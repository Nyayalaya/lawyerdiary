using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.Registers;
using CourtApp.Application.Extensions;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using KT3Core.Areas.Global.Classes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace CourtApp.Application.Features.Registers
{
    public class InstitutionRegisterQuery : IRequest<PaginatedResult<InstitutionResponse>>
    {
        public DateTime FromDt { get; set; }
        public DateTime ToDt { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        //public string UserId { get; set; }
        public List<string> LinkedIds { get; set; }
    }
    public class InstitutionRegisterQueryHandler : IRequestHandler<InstitutionRegisterQuery, PaginatedResult<InstitutionResponse>>
    {
        private readonly IUserCaseRepository _repository;
        private readonly ICaseAssignedRepository _assignRepo;
        public InstitutionRegisterQueryHandler(IUserCaseRepository _repository,
            ICaseAssignedRepository assignRepo)
        {
            this._repository = _repository;
            _assignRepo = assignRepo;
        }
        public async Task<PaginatedResult<InstitutionResponse>> Handle(InstitutionRegisterQuery request, CancellationToken cancellationToken)
        {
            var predicate = PredicateBuilder.True<CaseDetailEntity>();
            if (predicate != null)
            {
                if (request.LinkedIds.Count > 0)
                    predicate = predicate.And(c => request.LinkedIds.Contains(c.CreatedBy));

                predicate = predicate.And(i => i.InstitutionDate >= request.FromDt && i.InstitutionDate <= request.ToDt);
            }
            var caseDetails = (from c in _repository.Entites.Where(predicate)
                               join ac in _assignRepo.Entities
                                    on c.Id equals ac.CaseId into caseAssignments
                               from ac in caseAssignments.DefaultIfEmpty()
                               where request.LinkedIds.Contains(c.CreatedBy)
                               || request.LinkedIds.Contains(ac.LawyerId.ToString())
                               let asignedOrSelf = ac != null && request.LinkedIds.Contains(ac.LawyerId.ToString()) ? "Assigned" : "Self"
                               let isCaseAssigned = asignedOrSelf == "Self" && ac != null && ac.CaseId == c.Id
                               select new InstitutionResponse
                               {
                                   Id = c.Id,
                                   Reference = asignedOrSelf,
                                   IsCaseAssigned = isCaseAssigned,
                                   CaseType = c.CaseType.Name_En,
                                   No = c.CaseNo,
                                   Year = c.CaseYear.ToString(),
                                   Court = c.CourtBench.CourtBench_En,
                                   FirstTitle = c.FirstTitle,
                                   SecondTitle = c.SecondTitle,
                                   InsititutionDate = c.InstitutionDate != default(DateTime) ? c.InstitutionDate.ToString("dd/MM/yyyy") : "-",
                               }).ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return await caseDetails;
        }
    }
}
