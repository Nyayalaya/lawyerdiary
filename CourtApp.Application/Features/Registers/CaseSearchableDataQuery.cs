using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.Registers;
using CourtApp.Application.Extensions;
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

namespace CourtApp.Application.Features.Registers
{
    public class CaseSearchableDataQuery : IRequest<PaginatedResult<InstitutionResponse>>
    {
        //public string UserId { get; set; }
        public List<string> LinkedIds { get; set; }
        public Guid ClientId { get; set; }
        public string ReferalBy { get; set; }
        public string Status { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class CaseSearchableDataQueryHandler : IRequestHandler<CaseSearchableDataQuery, PaginatedResult<InstitutionResponse>>
    {
        private readonly IUserCaseRepository _caseRepo;
        private readonly IClientCacheRepository client;
        private readonly ICaseAssignedRepository _assignRepo;
        public CaseSearchableDataQueryHandler(IUserCaseRepository _caseRepo,
            IClientCacheRepository client,
            ICaseAssignedRepository assignRepo)
        {
            this._caseRepo = _caseRepo;
            this.client = client;
            _assignRepo = assignRepo;
        }
        public async Task<PaginatedResult<InstitutionResponse>> Handle(CaseSearchableDataQuery request, CancellationToken cancellationToken)
        {
            var predicate = PredicateBuilder.True<CaseDetailEntity>();
            if (predicate != null)
            {
                if (request.LinkedIds.Count > 0)
                    predicate = predicate.And(c => request.LinkedIds.Contains(c.CreatedBy));
                if (request.ClientId != Guid.Empty)
                    predicate = predicate.And(c => c.ClientId.Equals(request.ClientId));
                if (!string.IsNullOrEmpty(request.ReferalBy))
                {
                    var clientIds = (await client.GetCachedListAsync())
                        .Where(r => r.ReferalBy.Trim().ToLower().Equals(request.ReferalBy.Trim().ToLower()))
                        .Select(s => s.Id).ToList();
                    predicate = predicate.And(c => clientIds.Contains(c.ClientId.Value));
                }
                if (request.Status == "Pending")
                    predicate = predicate.And(p => p.DisposalDate == null);

                if (request.Status == "Disposal")
                    predicate = predicate.And(p => p.DisposalDate != null);

            }
            var caseDetails = (from c in _caseRepo.Entites.Where(predicate)
                               join ac in _assignRepo.Entities
                                    on c.Id equals ac.CaseId into caseAssignments
                               from ac in caseAssignments.DefaultIfEmpty()
                               where request.LinkedIds.Contains(c.CreatedBy)
                              || request.LinkedIds.Contains(ac.LawyerId.ToString())
                               let asignedOrSelf = ac != null && request.LinkedIds.Contains(ac.LawyerId.ToString()) ? "Assigned" : "Self"
                               let isCaseAssigned = asignedOrSelf == "Self" && ac != null && ac.CaseId == c.Id
                               let AssignedLawyerId = asignedOrSelf == "Self" && ac != null ? ac.LawyerId : Guid.Empty
                               select new InstitutionResponse
                               {
                                   Id = c.Id,
                                   Reference = asignedOrSelf,
                                   IsCaseAssigned = isCaseAssigned,
                                   LawyerId=AssignedLawyerId,
                                   CaseType = c.CaseType.Name_En,
                                   No = c.CaseNo,
                                   Year = c.CaseYear == 0 ? "" : c.CaseYear.ToString(),
                                   Court = c.CourtBench.CourtBench_En,
                                   FirstTitle = c.FirstTitle,
                                   SecondTitle = c.SecondTitle,
                                   InsititutionDate = c.InstitutionDate != default(DateTime) ? c.InstitutionDate.ToString("dd/MM/yyyy") : "-",
                               }).ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return await caseDetails;
        }
    }
}
