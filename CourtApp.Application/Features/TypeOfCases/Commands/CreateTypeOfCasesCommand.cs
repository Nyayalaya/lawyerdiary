using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
using CourtApp.Domain.Entities.Common;
using CourtApp.Domain.Entities.LawyerDiary;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Typeofcasess.Commands
{
    public class CreateTypeOfCasesCommand : IRequest<Result<Guid>>
    {
        public Guid NatureId { get; set; }
        public Guid CourtTypeId { get; set; }
        public int StateId { get; set; }
        public List<TypeOfCase> CaseTypes { get; set; }

    }
    public class TypeOfCase
    {
        public string Name_En { get; set; }
        public string Name_Hn { get; set; }
        public string Abbreviation { get; set; }
    }

    public class CreateCaseKindCommandHandler : IRequestHandler<CreateTypeOfCasesCommand, Result<Guid>>
    {
        private readonly ITypeOfCasesRepository repository;
        private readonly IMapper mapper;
        private readonly ICaseNatureRepository caseNatureRepository;

        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IMultiLangWordRepository _multiRepo;

        public CreateCaseKindCommandHandler(ITypeOfCasesRepository repository,
            IMapper mapper, IUnitOfWork _unitOfWork, ICaseNatureRepository caseNatureRepository,
            IMultiLangWordRepository _multiRepo)
        {
            this.repository = repository;
            this.mapper = mapper;
            this._unitOfWork = _unitOfWork;
            this.caseNatureRepository = caseNatureRepository;
            this._multiRepo = _multiRepo;
        }
        public async Task<Result<Guid>> Handle(CreateTypeOfCasesCommand request, CancellationToken cancellationToken)
        {

            if (request.CaseTypes == null || !request.CaseTypes.Any())
                return Result<Guid>.Fail("Case type is not supplied!");

            Guid lastInsertedId = Guid.Empty;

            foreach (var c in request.CaseTypes)
            {
                bool isDuplicate = repository.QryEntities.Any(w =>
                    w.Name_En.ToLower() == c.Name_En.ToLower().Trim() &&
                    w.CourtTypeId == request.CourtTypeId &&
                    w.NatureId == request.NatureId
                );

                if (isDuplicate)
                    return Result<Guid>.Fail($"The name '{c.Name_En}' already exists for the given Court Type and Nature.");

                var entity = new TypeOfCasesEntity
                {
                    Name_En = c.Name_En.Trim(),
                    Name_Hn = c.Name_Hn?.Trim(),
                    CourtTypeId = request.CourtTypeId,
                    NatureId = request.NatureId,
                    Abbreviation = c.Abbreviation.Trim()
                };

                await repository.InsertAsync(entity);
                lastInsertedId = entity.Id;
            }

            // Commit once after loop for better performance
            await _unitOfWork.Commit(cancellationToken);

            var keywords = request.CaseTypes
        .SelectMany(c => c.Name_En.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        .Select(w => w.Trim())
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Select(w => new MultiLangDictEntity
        {
            KeyWord = w
        })
        .ToList();

            if (keywords.Any())
            {
                await _multiRepo.BulkInsertAsync(keywords);
                await _unitOfWork.Commit(cancellationToken);
            }
            return Result<Guid>.Success(lastInsertedId);

        }
    }
}
