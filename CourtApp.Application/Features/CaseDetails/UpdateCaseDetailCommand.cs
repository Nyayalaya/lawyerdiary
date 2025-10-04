using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.CaseDetails;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseDetails
{
    public class UpdateCaseDetailCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        #region Common Properties Among all Court Type
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
    public class UpdateCaseDetailCommandHandler : IRequestHandler<UpdateCaseDetailCommand, Result<Guid>>
    {
        private readonly IUserCaseRepository _repository;
        private readonly IMapper _mapper;
        private IUnitOfWork _uow { get; set; }
        public UpdateCaseDetailCommandHandler(IUserCaseRepository _repository, IUnitOfWork _uow, IMapper _mapper)
        {
            this._repository = _repository;
            this._uow = _uow;
            this._mapper = _mapper;
        }

        public async Task<Result<Guid>> Handle(UpdateCaseDetailCommand request, CancellationToken cancellationToken)
        {
            var detail = await _repository.GetByIdAsync(request.Id);
            if (detail == null)
                return Result<Guid>.Fail("Case detail not found.");

            // 🔁 if proceeding already done, next date of the case should be non editable. 
            // if after proceeding next date updated, and user want to change the next date from the case entry update section
            // next date of the case always be greated than the first time entered next date.
            var isProceeding = detail.CaseProcEntities.Count() > 0 ? true : false;
            if (isProceeding)
                return await Result<Guid>.FailAsync("Proceeding already done for this case, so next date cannot be edit from here!");


            // 🔁 Check for existing case with same CaseNo and CaseYear (but different ID)
            var duplicateCase = await _repository.Entites
                .Where(x => x.CaseNo == request.CaseNo
                         && x.CaseYear == request.CaseYear
                         && x.CourtTypeId == request.CourtTypeId
                         && x.Id != request.Id)
                .FirstOrDefaultAsync();

            if (duplicateCase != null)
                return Result<Guid>.Fail("This Case Number already exists for the given year. " +
                    "Case detail not updated.");

            // 🔧 Assign values
            detail.InstitutionDate = request.InstitutionDate;
            detail.StateId = request.StateId;
            detail.ClientId = request.ClientId ?? null;
            detail.AppearenceID = request.AppearenceID;
            detail.CourtTypeId = request.CourtTypeId;
            detail.CourtBenchId = request.BenchId ?? request.CourtId ?? Guid.Empty;
            detail.CourtDistrictId = request.CourtDistrictId != Guid.Empty ? request.CourtDistrictId : null;
            detail.ComplexId = request.ComplexId != Guid.Empty ? request.ComplexId : null;
            detail.CaseCategoryId = request.CaseCategoryId;
            detail.CaseStageId = request.CaseStageId ?? null;
            detail.CaseYear = request.CaseYear ?? 0;
            detail.NextDate = request.NextDate ?? DateTime.MinValue;
            detail.CnrNumber = request.CnrNumber;
            detail.CisNumber = request.CisNumber;
            detail.CisYear = request.CisYear;
            detail.CaseNo = request.CaseNo;
            detail.CaseTypeId = request.CaseTypeId;
            detail.STitleId = request.STitleId;
            detail.FTitleId = request.FTitleId;
            detail.FirstTitle = request.FirstTitle;
            detail.SecondTitle = request.SecondTitle;
            detail.StrengthId = request.StrengthId ?? 0;
            detail.LinkedCaseId = request.LinkedCaseId ?? null;

            // 👥 Handle Against Case Details
            if (request.AgainstCaseDetails?.Any(a => a.StateId != null) == true)
            {
                if (detail.CaseAgainstEntities.Any())
                {
                    var existing = detail.CaseAgainstEntities.First(); // update first one
                    var item = request.AgainstCaseDetails.First();

                    existing.ImpugedOrderDate = item.ImpugedOrderDate ?? DateTime.MinValue;
                    existing.StateId = item.StateId.Value;
                    existing.CourtTypeId = item.CourtTypeId.Value;
                    existing.CourtBenchId = item.BenchId ?? item.CourtId ?? Guid.Empty;
                    existing.CaseYear = item.CaseYear.Value;
                    existing.CaseNo = item.CaseNo;
                    existing.CaseCategoryId = item.CaseCategoryId.Value;
                    existing.CaseTypeId = item.CaseTypeId ?? Guid.Empty;
                    existing.StrengthId = item.StrengthId ?? 0;
                    existing.OfficerName = item.OfficerName;
                    existing.CisYear = item.CisYear ?? 0;
                    existing.CisNo = item.CisNo;
                    existing.CaseId = request.Id;
                    existing.CadreId = item.CadreId ?? Guid.Empty;
                    existing.CnrNo = item.CnrNo;
                    existing.CourtDistrictId = item.CourtDistrictId != Guid.Empty ? item.CourtDistrictId : null;
                    existing.ComplexId = item.ComplexId != Guid.Empty ? item.ComplexId : null;
                }
                else
                {
                    detail.CaseAgainstEntities = request.AgainstCaseDetails.Select(item => new CaseDetailAgainstEntity
                    {
                        ImpugedOrderDate = item.ImpugedOrderDate ?? DateTime.MinValue,
                        CourtBenchId = item.BenchId ?? item.CourtId ?? Guid.Empty,
                        StateId = item.StateId.Value,
                        CourtTypeId = item.CourtTypeId.Value,
                        CaseYear = item.CaseYear.Value,
                        CaseNo = item.CaseNo,
                        CaseCategoryId = item.CaseCategoryId.Value,
                        CaseTypeId = item.CaseTypeId ?? Guid.Empty,
                        StrengthId = item.StrengthId ?? 0,
                        OfficerName = item.OfficerName,
                        CisYear = item.CisYear ?? 0,
                        CisNo = item.CisNo,
                        CaseId = request.Id,
                        CadreId = item.CadreId ?? Guid.Empty,
                        CnrNo = item.CnrNo,
                        CourtDistrictId = item.CourtDistrictId != Guid.Empty ? item.CourtDistrictId : null,
                        ComplexId = item.ComplexId != Guid.Empty ? item.ComplexId : null
                    }).ToList();
                }
            }

            await _repository.UpdateAsync(detail);
            await _uow.Commit(cancellationToken);
            return Result<Guid>.Success(detail.Id);


            /* Older code before implement the case exist condition.
            var detail = await _repository.GetByIdAsync(request.Id);
            if (detail == null)
                return Result<Guid>.Fail($"Case detail Not Found.");
            else
            {
                detail.InstitutionDate = request.InstitutionDate;
                detail.StateId = request.StateId;
                detail.ClientId = request.ClientId != null ? request.ClientId.Value : null;
                detail.AppearenceID = request.AppearenceID;
                detail.CourtTypeId = request.CourtTypeId;
                detail.CourtBenchId = request.BenchId != null ? request.BenchId.Value : request.CourtId.Value;
                detail.CourtDistrictId = request.CourtDistrictId != Guid.Empty ? request.CourtDistrictId : null;
                detail.ComplexId = request.ComplexId != Guid.Empty ? request.ComplexId : null;
                detail.CaseCategoryId = request.CaseCategoryId;
                detail.CaseStageId = request.CaseStageId != null ? request.CaseStageId.Value : null;
                detail.CaseYear = request.CaseYear != null ? request.CaseYear.Value : 0;
                detail.NextDate = request.NextDate != null ? request.NextDate.Value : DateTime.MinValue;
                detail.CnrNumber = request.CnrNumber;
                detail.CisNumber = request.CisNumber;
                detail.CisYear = request.CisYear;
                detail.CaseYear = request.CaseYear != null ? request.CaseYear.Value : 0;
                detail.CaseNo = request.CaseNo;
                detail.CaseCategoryId = request.CaseCategoryId;
                detail.CaseTypeId = request.CaseTypeId;
                detail.STitleId = request.STitleId;
                detail.FTitleId = request.FTitleId;
                detail.FirstTitle = request.FirstTitle;
                detail.SecondTitle = request.SecondTitle;
                detail.StrengthId = request.StrengthId != null ? request.StrengthId.Value : 0;
                detail.CaseTypeId = request.CaseTypeId;
                detail.LinkedCaseId = request.LinkedCaseId != null ? request.LinkedCaseId.Value : null;

                var againstDt = detail.CaseAgainstEntities.FirstOrDefault();
                var IsAgstCases = request.AgainstCaseDetails
                    .Where(w => w.StateId != null)
                    .Select(s => s.StateId).Count();
                if (IsAgstCases > 0)
                    if (againstDt != null && request.AgainstCaseDetails.Count > 0)
                    {
                        foreach (var item in request.AgainstCaseDetails)
                        {
                            againstDt.ImpugedOrderDate = item.ImpugedOrderDate.Value;
                            againstDt.StateId = item.StateId.Value;
                            againstDt.CourtTypeId = item.CourtTypeId.Value;
                            againstDt.CourtBenchId = item.BenchId != null ? item.BenchId.Value : item.CourtId.Value;
                            againstDt.CaseYear = item.CaseYear.Value;
                            againstDt.CaseNo = item.CaseNo;
                            againstDt.CaseCategoryId = item.CaseCategoryId.Value;
                            againstDt.CaseTypeId = item.CaseTypeId.Value;
                            againstDt.StrengthId = item.StrengthId != null ? item.StrengthId.Value : 0;
                            againstDt.OfficerName = item.OfficerName;
                            againstDt.CisYear = item.CisYear != null ? item.CisYear.Value : 0;
                            againstDt.CisNo = item.CisNo;
                            againstDt.CaseId = request.Id;
                            againstDt.CadreId = item.CadreId != null ? item.CadreId.Value : Guid.Empty;
                            againstDt.CnrNo = item.CnrNo;
                            againstDt.CourtDistrictId = item.CourtDistrictId != Guid.Empty ? item.CourtDistrictId : null;
                            againstDt.ComplexId = item.ComplexId != Guid.Empty ? item.ComplexId : null;
                        }
                    }
                    else
                    {
                        List<CaseDetailAgainstEntity> ag = new List<CaseDetailAgainstEntity>();
                        foreach (var item in request.AgainstCaseDetails)
                        {
                            CaseDetailAgainstEntity agDt = new CaseDetailAgainstEntity();
                            agDt.ImpugedOrderDate = item.ImpugedOrderDate.Value;
                            agDt.CourtBenchId = item.BenchId != null ? item.BenchId.Value : item.CourtId.Value;
                            agDt.StateId = item.StateId.Value;
                            agDt.CourtTypeId = item.CourtTypeId.Value;
                            agDt.CaseYear = item.CaseYear.Value;
                            agDt.CaseNo = item.CaseNo;
                            agDt.CaseCategoryId = item.CaseCategoryId.Value;
                            agDt.CaseTypeId = item.CaseTypeId != null ? item.CaseTypeId.Value : Guid.Empty;
                            agDt.StrengthId = item.StrengthId != null ? item.StrengthId.Value : 0;
                            agDt.OfficerName = item.OfficerName;
                            agDt.CisYear = item.CisYear.Value;
                            agDt.CisNo = item.CisNo;
                            agDt.CaseId = request.Id;
                            agDt.CadreId = item.CadreId != null ? item.CadreId.Value : Guid.Empty;
                            agDt.CnrNo = item.CnrNo;
                            agDt.CourtDistrictId = item.CourtDistrictId != Guid.Empty ? item.CourtDistrictId : null;
                            agDt.ComplexId = item.ComplexId != Guid.Empty ? item.ComplexId : null;
                            ag.Add(agDt);
                        }
                        detail.CaseAgainstEntities = ag;
                    }
                await _repository.UpdateAsync(detail);
                await _uow.Commit(cancellationToken);
                return Result<Guid>.Success(detail.Id);
            }
            */
        }
    }
}
