using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
using CourtApp.Domain.Entities.Common;
using CourtApp.Domain.Entities.LawyerDiary;
using KT3Core.Areas.Global.Classes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseNatures.Command
{
    public class CreateCaseNatureCommand : IRequest<Result<Guid>>
    {
        public string Name_En { get; set; }
        public string Name_Hn { get; set; }
        public Guid CourtTypeId { get; set; }
    }
    public class CreateCaseNatureCommandHandler : IRequestHandler<CreateCaseNatureCommand, Result<Guid>>
    {
        private readonly ICaseNatureRepository repository;
        private readonly IMapper mapper;
        private readonly IMultiLangWordRepository _multiRepo;
        private IUnitOfWork _unitOfWork { get; set; }
        public CreateCaseNatureCommandHandler(ICaseNatureRepository repository, IMapper mapper, 
            IUnitOfWork _unitOfWork, IMultiLangWordRepository _multiRepo)
        {
            this.repository = repository;
            this.mapper = mapper;
            this._unitOfWork = _unitOfWork;
            this._multiRepo = _multiRepo;
        }
        public async Task<Result<Guid>> Handle(CreateCaseNatureCommand request, CancellationToken cancellationToken)
        {

            // Build dynamic predicate
            var predicate = PredicateBuilder.True<NatureEntity>();

            if (request.CourtTypeId != Guid.Empty)
                predicate = predicate.And(b => b.CourtTypeId.Equals(request.CourtTypeId));

            if (!string.IsNullOrWhiteSpace(request.Name_En))
                predicate = predicate.And(b => b.Name_En.ToLower().Trim() == request.Name_En.ToLower().Trim());

            // Check for duplicates
            var isExists = await repository.CaseNatures
                .Where(predicate)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (isExists != null)
                return Result<Guid>.Fail($"Record is already exists.");

            // Insert new record
            var entity = mapper.Map<NatureEntity>(request);
            await repository.InsertAsync(entity);
            await _unitOfWork.Commit(cancellationToken);

            var keywords = request.Name_En
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new MultiLangDictEntity
                {
                    KeyWord = s
                })
                .ToList();

            if (keywords.Count > 0)
            {
                await _multiRepo.BulkInsertAsync(keywords);
                await _unitOfWork.Commit(cancellationToken);
            }


            return Result<Guid>.Success(entity.Id);
        }
    }

}
