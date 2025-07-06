using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.Registers;
using CourtApp.Application.Extensions;
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
    public class DisposalRegisterQuery : IRequest<PaginatedResult<DisposalRegisterResponse>>
    {
        public DateTime FromDt { get; set; }
        public DateTime ToDt { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<string> LinkedIds { get; set; }
        //public string UserId { get; set; }
    }

    public class RegistDisposalQueryHandler : IRequestHandler<DisposalRegisterQuery, PaginatedResult<DisposalRegisterResponse>>
    {


        private readonly ICaseProceedingRepository _wRepo;
        private readonly IUserCaseRepository _caseRepo;
        private readonly ICaseAssignedRepository _AssignedRepo;
        public RegistDisposalQueryHandler(ICaseProceedingRepository _wRepo, IUserCaseRepository _caseRepo, ICaseAssignedRepository assignedRepo)
        {
            this._caseRepo = _caseRepo;
            this._wRepo = _wRepo;
            _AssignedRepo = assignedRepo;
        }
        public Task<PaginatedResult<DisposalRegisterResponse>> Handle(DisposalRegisterQuery request, CancellationToken cancellationToken)
        {

            var caseDetails = from c in _caseRepo.Entites.Where(w => w.DisposalDate != null).AsNoTracking()
                              join ac in _AssignedRepo.Entities.AsNoTracking()
                                on c.Id equals ac.CaseId into caseAssignments
                              from ac in caseAssignments.DefaultIfEmpty()
                              where request.LinkedIds.Contains(c.CreatedBy)
                                 || (ac != null && request.LinkedIds.Contains(ac.LawyerId.ToString()))
                              let assignedOrSelf = (ac != null && request.LinkedIds.Contains(ac.LawyerId.ToString())) ? "Assigned" : "Self"
                              let isCaseAssigned = assignedOrSelf == "Self" && ac != null && ac.CaseId == c.Id
                              let Proceedings = c.CaseProcEntities.Select(proc => new
                              {
                                  ProcId = proc.Id,
                                  SubHead = proc.SubHead
                              }).ToList()
                              select new DisposalRegisterResponse
                              {
                                  DisposalDate = c.DisposalDate.HasValue ? c.DisposalDate.Value.ToString("dd-MM-yyyy") : "",
                                  Id = c.Id,
                                  FirstTitle = c.FirstTitle,
                                  SecondTitle = c.SecondTitle,
                                  No = c.CaseNo,
                                  Year = c.CaseYear.ToString(),
                                  Court = c.CourtBench.CourtBench_En,
                                  CaseType = c.CaseType.Name_En,
                                  Reason = Proceedings.Select(s => s.SubHead.Name_En).FirstOrDefault() ?? "",
                                  Reference= assignedOrSelf,
                                  InsititutionDate=c.InstitutionDate.ToString("dd/MM/yyyy")
                              };


            return caseDetails.ToPaginatedListAsync(request.PageNumber, request.PageSize); ;


            /* after verification this code will be removed
            var predicate = PredicateBuilder.True<CaseProcedingEntity>();
            if (request.LinkedIds.Count > 0)
                predicate = predicate.And(c => request.LinkedIds.Contains(c.CreatedBy));

            DateTime to = DateTime.Now;
            predicate = predicate.And(w => w.Head.Abbreviation == "DISP");
            var fndt = _wRepo
                .Entities
                .Include(h => h.Head)
                .Where(predicate)
                .Select(e => new DisposalRegisterResponse
                {
                    DisposalDate = e.ProceedingDate != null ? e.ProceedingDate.Value.ToString("dd-MM-yyyy") : "",
                    Id = e.CaseId,
                    FirstTitle = e.Case.FirstTitle,
                    SecondTitle = e.Case.SecondTitle,
                    No = e.Case.CaseNo,
                    Year = e.Case.CaseYear.ToString(),
                    Court = e.Case.CourtBench.CourtBench_En,
                    CaseType = e.Case.CaseType.Name_En,
                    Reason = e.SubHead.Name_En
                })
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return fndt;
            */
        }
    }
}
