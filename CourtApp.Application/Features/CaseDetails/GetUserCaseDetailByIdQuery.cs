using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.CaseDetails;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.UserCase
{
    public class GetUserCaseDetailByIdQuery : IRequest<Result<UserCaseDetailResponse>>
    {
        public Guid CaseId { get; set; }
        public List<string> LinkedIds { get; set; }
    }

    public class GetUserCaseDetailByIdQueryHandler : IRequestHandler<GetUserCaseDetailByIdQuery, Result<UserCaseDetailResponse>>
    {
        private readonly IUserCaseRepository _CaseRepo;
        private readonly IMapper _mapper;
        private readonly IClientRepository _clientRepository;
        public GetUserCaseDetailByIdQueryHandler(IUserCaseRepository _CaseRepo, IMapper _mapper, IClientRepository clientRepository)
        {
            this._CaseRepo = _CaseRepo;
            this._mapper = _mapper;
            _clientRepository = clientRepository;
        }
        public async Task<Result<UserCaseDetailResponse>> Handle(GetUserCaseDetailByIdQuery request, CancellationToken cancellationToken)
        {
            // Fetch case detail based on condition
            var detail = (request.LinkedIds.Any() && request.CaseId == Guid.Empty)
                ? await _CaseRepo.GetMostRecentCaseInfo(request.LinkedIds)
                : await _CaseRepo.GetByIdAsync(request.CaseId);

            if (detail == null)
                return await Result<UserCaseDetailResponse>.FailAsync("Case information does not exists!");


            var mappedData = _mapper.Map<UserCaseDetailResponse>(detail);
            mappedData.IsProceeding = detail.CaseProcEntities?.Any() == true;

            bool isHighCourt = detail.CourtType?.Abbreviation == "HICT";
            mappedData.IsHighCourt = isHighCourt;

            if (isHighCourt)
                mappedData.BenchId = detail.CourtBenchId;
            else
                mappedData.CourtId = detail.CourtBenchId;

            // Map against case details
            if (detail.CaseAgainstEntities?.Any() == true)
            {
                mappedData.AgainstCaseDetails = detail.CaseAgainstEntities.Select(item => new UpseartAgainstCaseDto
                {
                    ImpugedOrderDate = item.ImpugedOrderDate,
                    StateId = item.StateId,
                    CourtTypeId = item.CourtTypeId,
                    IsAgHighCourt = item.CourtType?.Abbreviation == "HICT",
                    BenchId = item.CourtType?.Abbreviation == "HICT" ? item.CourtBenchId : Guid.Empty,
                    CourtId = item.CourtType?.Abbreviation != "HICT" ? item.CourtBenchId : Guid.Empty,
                    CourtDistrictId = item.CourtDistrictId ?? Guid.Empty,
                    ComplexId = item.ComplexId ?? Guid.Empty,
                    CadreId = item.CadreId,
                    CaseNo = item.CaseNo,
                    CaseCategoryId = item.CaseCategoryId,
                    CaseTypeId = item.CaseTypeId,
                    CaseYear = item.CaseYear,
                    CisNo = item.CisNo,
                    CisYear = item.CisYear,
                    CnrNo = item.CnrNo,
                    OfficerName = item.OfficerName,
                    StrengthId = item.StrengthId
                }).ToList();
            }

            return Result<UserCaseDetailResponse>.Success(mappedData);

            //CaseDetailEntity detail = new CaseDetailEntity();
            ////this condition is applicable for getting most
            ////recent case for repeat the case.
            //if (request.LinkedIds.Any() && request.CaseId == Guid.Empty)
            //    detail = await _CaseRepo.GetMostRecentCaseInfo(request.LinkedIds);
            //else
            //    detail = await _CaseRepo.GetByIdAsync(request.CaseId);

            //if (detail.CourtType.Abbreviation == "HICT")
            //{
            //    mappeddata.IsHighCourt = true;
            //    mappeddata.BenchId = detail.CourtBenchId;
            //}
            //else
            //    mappeddata.CourtId = detail.CourtBenchId;

            //if (detail.CaseAgainstEntities != null && detail.CaseAgainstEntities.Count > 0)
            //{
            //    var agl = new List<UpseartAgainstCaseDto>();
            //    foreach (var item in detail.CaseAgainstEntities)
            //    {
            //        var agc = new UpseartAgainstCaseDto();
            //        agc.ImpugedOrderDate = item.ImpugedOrderDate;
            //        agc.StateId = item.StateId;
            //        agc.CourtTypeId = item.CourtTypeId;
            //        if (item.CourtType.Abbreviation == "HICT")
            //        {
            //            agc.IsAgHighCourt = true;
            //            agc.BenchId = item.CourtBenchId;
            //        }
            //        else
            //            agc.CourtId = item.CourtBenchId;
            //        agc.CourtDistrictId = item.CourtDistrictId != null ? item.CourtDistrictId : Guid.Empty;
            //        agc.ComplexId = item.ComplexId != null ? item.ComplexId : Guid.Empty;
            //        agc.CadreId = item.CadreId;
            //        agc.CaseNo = item.CaseNo;
            //        agc.CaseCategoryId = item.CaseCategoryId;
            //        agc.CaseTypeId = item.CaseTypeId;
            //        agc.CaseYear = item.CaseYear;
            //        agc.CisNo = item.CisNo;
            //        agc.CisYear = item.CisYear;
            //        agc.CnrNo = item.CnrNo;
            //        agc.OfficerName = item.OfficerName;
            //        agc.StrengthId = item.StrengthId;
            //        agl.Add(agc);
            //    }
            //    mappedData.AgainstCaseDetails = agl;
            //}

            //return Result<UserCaseDetailResponse>.Success(mappedData);


        }
    }
}
