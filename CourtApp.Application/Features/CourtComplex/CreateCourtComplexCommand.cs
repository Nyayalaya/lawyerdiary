using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
using CourtApp.Domain.Entities.Common;
using CourtApp.Domain.Entities.LawyerDiary;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CourtComplex
{
    public class CreateCourtComplexCommand : IRequest<Result<Guid>>
    {
        public int StateId { get; set; }
        public Guid CourtDistrictId { get; set; }
        //public string Name_En { get; set; }
        //public string Name_Hn { get; set; }
        //public string Abbreviation { get; set; }
        public List<DistrictComplex> Complexes { get; set; }
    }
    public class DistrictComplex
    {
        public string Name_En { get; set; }
        public string Name_Hn { get; set; }
    }
    public class CreateCourtComplexCommandHandler : IRequestHandler<CreateCourtComplexCommand, Result<Guid>>
    {
        private readonly ICourtComplexRepository repository;
        private readonly IMapper mapper;
        private readonly IMultiLangWordRepository _multiRepo;
        private IUnitOfWork _unitOfWork { get; set; }
        public CreateCourtComplexCommandHandler(ICourtComplexRepository repository, IMapper mapper, 
            IUnitOfWork _unitOfWork, IMultiLangWordRepository _multiRepo)
        {
            this.repository = repository;
            this.mapper = mapper;
            this._unitOfWork = _unitOfWork;
            this._multiRepo = _multiRepo;
        }
        public async Task<Result<Guid>> Handle(CreateCourtComplexCommand request, CancellationToken cancellationToken)
        {
            if (request.Complexes == null || request.Complexes.Count == 0)
                return Result<Guid>.Fail("Court complexes are not supplied!");

            var insertedId = Guid.Empty;

            foreach (var complex in request.Complexes)
            {
                // Normalize the name to avoid null reference and trim/ToLower
                var normalizedEnName = complex.Name_En?.Trim().ToLower();

                var exists = await repository.Entities
                    .AnyAsync(w =>
                        w.Name_En.Trim().ToLower() == normalizedEnName &&
                        w.CourtDistrictId == request.CourtDistrictId &&
                        w.StateId == request.StateId,
                        cancellationToken);

                if (exists)
                {
                    return Result<Guid>.Fail($"Record already exists: {complex.Name_En}");
                }

                var newEntity = new CourtComplexEntity
                {
                    Name_En = complex.Name_En?.Trim(),
                    Name_Hn = complex.Name_Hn?.Trim(),
                    StateId = request.StateId,
                    CourtDistrictId = request.CourtDistrictId
                };

                await repository.InsertAsync(newEntity);
                insertedId = newEntity.Id;
            }

            // Commit after all inserts (only once)
            await _unitOfWork.Commit(cancellationToken);


            var distCourts = request.Complexes.Select(s => s.Name_En).ToList();
            var keywords = new List<MultiLangDictEntity>();

            foreach (var cd in distCourts)
            {
                if (string.IsNullOrWhiteSpace(cd))
                    continue;

                var words = cd
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => new MultiLangDictEntity
                    {
                        KeyWord = s
                    });

                keywords.AddRange(words);
            }

            // OPTIONAL: Remove duplicate keywords (case-insensitive)
            keywords = keywords
                .GroupBy(k => k.KeyWord, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();

            if (keywords.Any())
            {
                await _multiRepo.BulkInsertAsync(keywords);
                await _unitOfWork.Commit(cancellationToken);
            }

            return Result<Guid>.Success(insertedId);
        }
    }
}
