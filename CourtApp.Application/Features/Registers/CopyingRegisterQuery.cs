using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.Registers;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Registers
{
    public class CopyingRegisterQuery : IRequest<Result<List<CopyDisposalResponse>>>
    {
        public DateTime FromDt { get; set; }
        public DateTime ToDt { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int SearchType { get; set; }
        public List<string> LinkedIds { get; set; }
        //public string UserId { get; set; }
    }
    public class CopyingRegisterQueryHandler : IRequestHandler<CopyingRegisterQuery, Result<List<CopyDisposalResponse>>>
    {
        private readonly IUserCaseRepository _caseRepo;
        private readonly ICaseWorkRepository _wRepo;
        private readonly ICaseProceedingRepository _ProcRepo;
        private readonly IWorkMasterRepository _WorkRepo;
        private readonly ICaseAssignedRepository _AssignedRepo;
        public CopyingRegisterQueryHandler(IUserCaseRepository _caseRepo
            , ICaseWorkRepository _wRepo,
ICaseProceedingRepository procRepo,
IWorkMasterRepository _WorkRepo,
ICaseAssignedRepository assignedRepo)
        {
            this._caseRepo = _caseRepo;
            this._wRepo = _wRepo;
            _ProcRepo = procRepo;
            this._WorkRepo = _WorkRepo;
            _AssignedRepo = assignedRepo;
        }
        public async Task<Result<List<CopyDisposalResponse>>> Handle(CopyingRegisterQuery request, CancellationToken cancellationToken)
        {

            //var caseDetails = (from c in _caseRepo.Entites.AsNoTracking()
            //                   join ac in _AssignedRepo.Entities.AsNoTracking()
            //                     on c.Id equals ac.CaseId into caseAssignments
            //                   from ac in caseAssignments.DefaultIfEmpty()
            //                   where request.LinkedIds.Contains(c.CreatedBy)
            //                      || (ac != null && request.LinkedIds.Contains(ac.LawyerId.ToString()))
            //                   let assignedOrSelf = (ac != null && request.LinkedIds.Contains(ac.LawyerId.ToString())) ? "Assigned" : "Self"
            //                   let isCaseAssigned = assignedOrSelf == "Self" && ac != null && ac.CaseId == c.Id
            //                   let Proceedings = c.CaseProcEntities.Select(proc => new
            //                   {
            //                       ProcId = proc.Id,
            //                       ProcWork = new
            //                       {
            //                           proc.ProcWork.LastWorkingDate,
            //                           Works = (from w in proc.ProcWork.Works
            //                                    join wm in _WorkRepo.Entities.AsNoTracking()
            //                                        on w.WorkTypeId equals wm.Id
            //                                    where wm.Abbreviation.ToUpper() == "COPY"
            //                                    select new
            //                                    {
            //                                        w.WorkId,
            //                                        w.Status,
            //                                        AppliedOn = w.AppliedOn, // Raw DateTime here
            //                                        ReceivedOn = w.ReceivedOn, // Raw DateTime here
            //                                        WorkTypeName = wm.Work_En
            //                                    }).ToList()
            //                       }
            //                   }).ToList()
            //                   select new CopyDisposalResponse
            //                   {
            //                       Id = c.Id,
            //                       Court = c.CourtBench.CourtBench_En.ToString(),
            //                       No = c.CaseNo,
            //                       Year = c.CaseYear.ToString(),
            //                       FirstTitle = c.FirstTitle,
            //                       SecondTitle = c.SecondTitle,
            //                       CaseType = c.CaseType.Name_En,
            //                       // Apply formatting client-side after data retrieval
            //                       AppliedOn = Proceedings.SelectMany(s => s.ProcWork.Works)
            //                                              .Where(w => w.AppliedOn != default(DateTime))
            //                                              .Select(w => w.AppliedOn.ToString("dd/MM/yyyy"))
            //                                              .FirstOrDefault() ?? "",  // Extract the first non-empty AppliedOn date
            //                       ReceivedOn = Proceedings.SelectMany(s => s.ProcWork.Works)
            //                                                .Where(w => w.ReceivedOn != default(DateTime))
            //                                                .Select(w => w.ReceivedOn.ToString("dd/MM/yyyy"))
            //                                                .FirstOrDefault() ?? "", // Extract the first non-empty ReceivedOn date
            //                       Reason = Proceedings.Select(s => s.ProcWork.Works.Select(w => w.WorkTypeName).FirstOrDefault())
            //                                            .FirstOrDefault() ?? ""  // Take the first work reason
            //                   }).ToList();

            var caseDetails = (
                from c in _caseRepo.Entites.AsNoTracking()

                join ac in _AssignedRepo.Entities.AsNoTracking()
                    on c.Id equals ac.CaseId into caseAssignments
                from ac in caseAssignments.DefaultIfEmpty()

                where request.LinkedIds.Contains(c.CreatedBy)
                    || (ac != null && request.LinkedIds.Contains(ac.LawyerId.ToString()))

                from proc in c.CaseProcEntities
                from w in proc.ProcWork.Works

                join wm in _WorkRepo.Entities.AsNoTracking()
                    on w.WorkTypeId equals wm.Id

                where wm.Abbreviation.ToUpper() == "COPY"
                        && w.AppliedOn != default(DateTime)  

                select new CopyDisposalResponse
                {
                    Id = c.Id,
                    Court = c.CourtBench.CourtBench_En,
                    No = c.CaseNo,
                    Year = c.CaseYear.ToString(),
                    FirstTitle = c.FirstTitle,
                    SecondTitle = c.SecondTitle,
                    CaseType = c.CaseType.Name_En,

                    AppliedOn = w.AppliedOn.ToString("dd/MM/yyyy"),

                    ReceivedOn = (w.ReceivedOn != default(DateTime)
                                && w.ReceivedOn != DateTime.MaxValue)
                                    ? w.ReceivedOn.ToString("dd/MM/yyyy")
                                    : "",

                    Reason = wm.Work_En
                }
            ).ToList();


            // Check if caseDetails is empty
            if (caseDetails == null || !caseDetails.Any())
                return await Result<List<CopyDisposalResponse>>.FailAsync("There are no proceedings available");

            // Filter based on search type
            List<CopyDisposalResponse> awc = caseDetails;

            if (request.SearchType == 3)
                awc = awc.Where(w => !string.IsNullOrEmpty(w.ReceivedOn)).ToList();
            if (request.SearchType == 2)
                awc = awc.Where(w => string.IsNullOrEmpty(w.ReceivedOn)).ToList();

            // Further filtering to ensure AppliedOn is not empty
            var dt = awc.Where(w => !string.IsNullOrEmpty(w.AppliedOn)).ToList();

            // Return the result
            return Result<List<CopyDisposalResponse>>.Success(dt);

        }
    }

}
