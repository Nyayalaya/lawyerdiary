using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.CaseWorking;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseWork
{
    public class GetAssignedWorkQuery : IRequest<Result<List<AssignedWorkToCaseResponse>>>
    {
        public Guid CaseId { get; set; }
        public DateTime WorkingDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string UserId { get; set; }
    }
    public class GetAssignedWorkQueryHandler : IRequestHandler<GetAssignedWorkQuery, Result<List<AssignedWorkToCaseResponse>>>
    {
        private readonly ICaseWorkRepository _Repository;
        private readonly ICaseProceedingRepository _ProcRepo;
        private readonly IWorkMasterSubRepository _SWRepo;
        public GetAssignedWorkQueryHandler(ICaseWorkRepository _Repository,
            ICaseProceedingRepository _ProcRepo,
            IWorkMasterRepository wRepo,
            IWorkMasterSubRepository sWRepo)
        {
            this._Repository = _Repository;
            this._ProcRepo = _ProcRepo;
            _SWRepo = sWRepo;
        }
        public async Task<Result<List<AssignedWorkToCaseResponse>>> Handle(GetAssignedWorkQuery request, CancellationToken cancellationToken)
        {
            var CaseWorkDetail = await _ProcRepo.GetListAsync();
            var UserPendingWork = CaseWorkDetail.Where(w => w.CreatedBy.Equals(request.UserId));
            if (UserPendingWork.Any())
            {
                List<AssignedWorkToCaseResponse> awc = new List<AssignedWorkToCaseResponse>();
                foreach (var c in UserPendingWork.Distinct())
                {
                    AssignedWorkToCaseResponse a = new AssignedWorkToCaseResponse();
                    a.CaseId = c.CaseId;
                    a.CaseDetail = " (" + c.Case.CaseNo + "/" + c.Case.CaseYear + "/" + c.Case.CaseType.Name_En + "/" + c.Case.CourtBench.CourtBench_En + ") /" + c.Case.FirstTitle + " " + c.Case.SecondTitle;
                    a.AWorks = new List<AssignedWork>();
                    a.ProceedingDate = c.ProceedingDate.HasValue? c.ProceedingDate.Value:default(DateTime);
                    a.LastWorkingDate = c.ProcWork != null && c.ProcWork.LastWorkingDate != null ? c.ProcWork.LastWorkingDate.Value : default(DateTime);
                    if (c.ProcWork != null)
                    {
                        foreach (var w in c.ProcWork.Works.Where(w => w.WorkId != Guid.Empty).Where(s => s.Status == 0))
                        {
                            AssignedWork aw = new AssignedWork();
                            aw.Id = c.Id;
                            aw.WorkId = w.WorkId;
                            var swork = await _SWRepo.GetByIdAsync(w.WorkId);
                            aw.WorkDetail = swork != null ? swork.Work.Work_En + " - " + swork.Name_En : "";
                            a.AWorks.Add(aw);
                        }
                        awc.Add(a);
                    }
                }
                var workc = awc.Where(w => w.AWorks.Count > 0).ToList();
                var works = workc.SelectMany(s => s.AWorks).Where(w => w.WorkId != Guid.Empty);
                if (works.Count() > 0)
                    return Result<List<AssignedWorkToCaseResponse>>.Success(workc.OrderByDescending(o => o.LastWorkingDate).ToList());
                else
                    return Result<List<AssignedWorkToCaseResponse>>.Success("There is no work allocated");
            }
            return Result<List<AssignedWorkToCaseResponse>>.Fail("There is no record");
        }
    }
}
