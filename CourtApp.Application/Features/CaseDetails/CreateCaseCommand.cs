using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.CaseDetails;
using CourtApp.Application.Interfaces.CacheRepositories;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using KT3Core.Areas.Global.Classes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Case
{
    public class CreateCaseCommand : IRequest<Result<Guid>>
    {
        #region Common Properties Among all Court Type
        public List<string> LinkedIds { get; set; }
        public DateTime InstitutionDate { get; set; }
        public int StateId { get; set; }
        public Guid CourtTypeId { get; set; }
        public Guid CaseCategoryId { get; set; }
        public Guid CaseTypeId { get; set; }
        public string CaseNo { get; set; }
        public int? CaseYear { get; set; }
        public string FirstTitle { get; set; }
        public Guid FTitleId { get; set; }
        public string SecondTitle { get; set; }
        public Guid STitleId { get; set; }
        public string CisNumber { get; set; }
        public int CisYear { get; set; }
        public string CnrNumber { get; set; }
        public DateTime? NextDate { get; set; }
        public Guid? CaseStageId { get; set; }
        public Guid? LinkedCaseId { get; set; }
        public Guid? ClientId { get; set; }
        public Guid AppearenceID { get; set; }
        public Guid? LCaseId { get; set; }
        public List<UpseartAgainstCaseDto> AgainstCaseDetails { get; set; }
        #endregion

        #region Other than High Court Propeties
        public Guid? CourtDistrictId { get; set; }
        public Guid? ComplexId { get; set; }
        public Guid? CourtId { get; set; }

        #endregion

        #region HighCourt Properties        
        public int? StrengthId { get; set; }
        public Guid? BenchId { get; set; }
        #endregion

    }
    public class CaseAgainstEntityModel
    {
        public Guid Id { get; set; }
        public DateTime ImpugedOrderDate { get; set; }
        public Guid CourtTypeId { get; set; }
        public Guid? CourtBenchId { get; set; }
        public Guid CaseCategoryId { get; set; }
        public Guid CaseTypeId { get; set; }
        public int StateId { get; set; }
        public int? StrengthId { get; set; }
        public string CaseNo { get; set; }
        public int CaseYear { get; set; }
        public int CisYear { get; set; }
        public string OfficerName { get; set; }
        public Guid? Cadre { get; set; }
        public Guid? CourtDistrictId { get; set; }
        public Guid? AgainstBenchId { get; set; }
        public Guid? ComplexId { get; set; }
        public Guid? CourtId { get; set; }
        public string CisNumber { get; set; }
        public string CnrNumber { get; set; }
        public bool IsAgHighCourt { get; set; }
    }

    public class CreateCaseManagmentCommandHandler : IRequestHandler<CreateCaseCommand, Result<Guid>>
    {
        private readonly IUserCaseRepository _Repository;
        private readonly IMapper _mapper;
        private readonly ICaseNatureCacheRepository _CaseNatureCacheRepository;
        private readonly ICaseKindCacheRepository _CaseKindCacheRepository;
        private readonly IClientCacheRepository _ClientCacheRepository;
        private readonly ICaseStageCacheRepository _CaseStageCacheRepository;
        private readonly ICourtTypeCacheRepository _CourtTypeCacheRepository;
        private readonly ICourtMasterCacheRepository _CourtMasterCacheRepository;
        private readonly ITypeOfCasesCacheRepository _ITypeOfCasesCacheRepository;
        private readonly ICaseAgainstRepository _CaseAgainstRepo;

        private IUnitOfWork _unitOfWork { get; set; }
        public CreateCaseManagmentCommandHandler(IUserCaseRepository _Repository,
            IMapper _mapper, IUnitOfWork _unitOfWork,
            ICaseKindCacheRepository caseKindCacheRepository,
            IClientCacheRepository clientCacheRepository,
            ICaseStageCacheRepository caseStageCacheRepository,
            ICourtTypeCacheRepository _CourtTypeCacheRepository,
            ITypeOfCasesCacheRepository _ITypeOfCasesCacheRepository,
            ICourtMasterCacheRepository _CourtMasterCacheRepository,
            ICaseNatureCacheRepository _CaseNatureCacheRepository,
            ICaseAgainstRepository _CaseAgainstRepo
            )
        {
            this._mapper = _mapper;
            this._Repository = _Repository;
            this._unitOfWork = _unitOfWork;
            _CaseKindCacheRepository = caseKindCacheRepository;
            _ClientCacheRepository = clientCacheRepository;
            _CaseStageCacheRepository = caseStageCacheRepository;
            this._CourtTypeCacheRepository = _CourtTypeCacheRepository;
            this._ITypeOfCasesCacheRepository = _ITypeOfCasesCacheRepository;
            this._CourtMasterCacheRepository = _CourtMasterCacheRepository;
            this._CaseNatureCacheRepository = _CaseNatureCacheRepository;
            this._CaseAgainstRepo = _CaseAgainstRepo;
        }
        public async Task<Result<Guid>> Handle(CreateCaseCommand request, CancellationToken cancellationToken)
        {
            var predicate = PredicateBuilder.True<CaseDetailEntity>();
            if (predicate != null)
            {
                if (request.LinkedIds.Count > 0)
                    predicate = predicate.And(y => request.LinkedIds.Contains(y.CreatedBy));

                if (request.CourtTypeId != Guid.Empty)
                    predicate = predicate.And(y => y.CourtTypeId == request.CourtTypeId);

                if (request.ComplexId.HasValue && request.ComplexId.Value != Guid.Empty)
                    predicate = predicate.And(y => y.CourtBenchId == request.CourtId && y.ComplexId == request.ComplexId);

                if (request.BenchId.HasValue && request.BenchId.Value != Guid.Empty)
                    predicate = predicate.And(y => y.CourtBenchId == request.BenchId);

                if (request.CaseTypeId != Guid.Empty)
                    predicate = predicate.And(y => y.CaseTypeId == request.CaseTypeId);

                if (!string.IsNullOrWhiteSpace(request.CaseNo) && request.CaseYear != 0)
                    predicate = predicate.And(y => y.CaseNo == request.CaseNo && y.CaseYear == request.CaseYear);

                if (string.IsNullOrWhiteSpace(request.CaseNo) &&
                    !string.IsNullOrWhiteSpace(request.FirstTitle) &&
                    !string.IsNullOrWhiteSpace(request.SecondTitle))
                {
                    predicate = predicate.And(y => y.FirstTitle == request.FirstTitle && y.SecondTitle == request.SecondTitle);
                }
            }

            var existingCase = _Repository.Entites.Where(predicate).FirstOrDefault();
            if (existingCase != null)
                return Result<Guid>.Fail("Record is already already exist.");

            var entity = _mapper.Map<CaseDetailEntity>(request);

            // Ensure CourtBenchId fallback
            entity.CourtBenchId = request.BenchId ?? request.CourtId ?? Guid.Empty;

            // Nullify empty optional fields
            entity.CourtDistrictId = request.CourtDistrictId == Guid.Empty ? null : request.CourtDistrictId;
            entity.ComplexId = request.ComplexId == Guid.Empty ? null : request.ComplexId;

            // Add "Against Case" details
            if (request.AgainstCaseDetails?.Any(s => !string.IsNullOrWhiteSpace(s.CaseNo)) == true)
            {
                entity.CaseAgainstEntities = request.AgainstCaseDetails
                    .Where(s => !string.IsNullOrWhiteSpace(s.CaseNo))
                    .Select(item => new CaseDetailAgainstEntity
                    {
                        ImpugedOrderDate = item.ImpugedOrderDate ?? DateTime.MinValue,
                        CourtTypeId = item.CourtTypeId ?? Guid.Empty,
                        CourtBenchId = item.BenchId ?? item.CourtId ?? Guid.Empty,
                        StateId = item.StateId.Value,
                        CaseYear = item.CaseYear ?? 0,
                        CaseNo = item.CaseNo,
                        CaseCategoryId = item.CaseCategoryId ?? Guid.Empty,
                        CaseTypeId = item.CaseTypeId ?? Guid.Empty,
                        StrengthId = item.StrengthId ?? 0,
                        OfficerName = item.OfficerName,
                        CisYear = item.CisYear ?? 0,
                        CisNo = item.CisNo,
                        CadreId = item.CadreId ?? null,
                        CnrNo = item.CnrNo,
                        CourtDistrictId = item.CourtDistrictId != Guid.Empty ? item.CourtDistrictId : null,
                        ComplexId = item.ComplexId != Guid.Empty ? item.ComplexId : null
                    }).ToList();
            }

            var result = await _Repository.InsertAsync(entity);
            await _unitOfWork.Commit(cancellationToken);

            return Result<Guid>.Success(entity.Id);

        }
    }
}
