using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.CourtMaster;
using CourtApp.Application.Interfaces.CacheRepositories;
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

namespace CourtApp.Application.Features.CourtMasters.Command
{
    public class CreateCourtMasterCommand : IRequest<Result<Guid>>
    {
        public Guid CourtTypeId { get; set; }
        public string CourtName { get; set; }
        public int StateCode { get; set; }
        public Guid CourtDistrictId { get; set; }
        public Guid CourtComplexId { get; set; }
        public List<CourtBenchResponse> CourtBenches { get; set; }
    }

    public class CreateCourtMasterCommandHandler : IRequestHandler<CreateCourtMasterCommand, Result<Guid>>
    {
        private readonly ICourtMasterRepository repository;
        private readonly ICourtTypeCacheRepository _CourtTypeRepo;
        private readonly IStateMasterRepository _StateRepo;
        private readonly IDistrictMasterRepository _DistrictRepo;
        private readonly ICourtBenchRepository _CourtBenchRepo;
        private readonly IMapper mapper;
        private readonly IMultiLangWordRepository _multiRepo;
        private IUnitOfWork _unitOfWork { get; set; }
        public CreateCourtMasterCommandHandler(ICourtMasterRepository repository, IMapper mapper,
            IUnitOfWork _unitOfWork, ICourtTypeCacheRepository _CourtTypeRepo,
            IStateMasterRepository _StateRepo, IDistrictMasterRepository districtRepo, 
            ICourtBenchRepository courtBenchRepository, IMultiLangWordRepository multiRepo)
        {
            this.repository = repository;
            this.mapper = mapper;
            this._unitOfWork = _unitOfWork;
            this._CourtTypeRepo = _CourtTypeRepo;
            this._StateRepo = _StateRepo;
            _DistrictRepo = districtRepo;
            this._CourtBenchRepo = courtBenchRepository;
            _multiRepo = multiRepo;
        }
        public async Task<Result<Guid>> Handle(CreateCourtMasterCommand request, CancellationToken cancellationToken)
        {
            if (request.CourtBenches == null || request.CourtBenches.Count == 0)
                return Result<Guid>.Fail("Court benches are not supplied!");

            var courtExists = await repository.Entities.AnyAsync(w =>
    w.StateId == request.StateCode && // Mandatory field
    w.CourtTypeId == request.CourtTypeId && // Mandatory field
    (request.CourtDistrictId == Guid.Empty || w.CourtDistrictId == request.CourtDistrictId) && // Optional field
    (request.CourtComplexId == Guid.Empty || w.CourtComplexId == request.CourtComplexId), // Optional field
    cancellationToken);

            if (courtExists)
                return Result<Guid>.Fail($"Court relation is already exists, please edit the same for adding more court");

            // Save CourtMasterEntity (parent)
            var courtMaster = new CourtMasterEntity
            {
                StateId = request.StateCode,
                CourtDistrictId = request.CourtDistrictId,
                CourtComplexId = request.CourtComplexId,
                CourtTypeId = request.CourtTypeId
            };

            await repository.InsertAsync(courtMaster);
            await _unitOfWork.Commit(cancellationToken); // Ensure parent ID is generated

            // Save each CourtBenchEntity (child)
            foreach (var bench in request.CourtBenches)
            {
                var benchEntity = new CourtBenchEntity
                {
                    CourtMasterId = courtMaster.Id,
                    CourtBench_En = bench.CourtBench_En?.Trim(),
                    CourtBench_Hn = bench.CourtBench_Hn?.Trim()
                };

                await _CourtBenchRepo.AddBenchAsync(benchEntity);
            }

            await _unitOfWork.Commit(cancellationToken);

            var distCourts = request.CourtBenches.Select(s => s.CourtBench_En).ToList();
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
            return Result<Guid>.Success(courtMaster.Id);

        }
    }
}
