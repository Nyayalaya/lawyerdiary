using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.CaseProceedings;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseProceeding
{
    public class CreateCaseProceedingCommand : IRequest<Result<Guid>>
    {
        public Guid CaseId { get; set; }
        public Guid HeadId { get; set; }
        public Guid SubHeadId { get; set; }
        public Guid? StageId { get; set; }
        public DateTime? NextDate { get; set; }
        public string Remark { get; set; }
        public ProceedingWorkDto ProcWork { get; set; }
        public DateTime? ProceedingDate { get; set; }
        public string UserId { get; set; }
        public List<Guid> MCasIds { get; set; }

    }

    public class CreateCaseProceedingCommandHandler : IRequestHandler<CreateCaseProceedingCommand, Result<Guid>>
    {
        private readonly ICaseProceedingRepository _Repository;
        private readonly IUserCaseRepository _CaseRepo;
        private readonly IProceedingHeadRepository _ProcRepo;
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork { get; set; }
        public CreateCaseProceedingCommandHandler(ICaseProceedingRepository _Repository,
            IMapper _mapper,
            IUnitOfWork _unitOfWork,
            IUserCaseRepository caseRepo,
            IProceedingHeadRepository procRepo)
        {
            this._Repository = _Repository;
            this._mapper = _mapper;
            this._unitOfWork = _unitOfWork;
            _CaseRepo = caseRepo;
            _ProcRepo = procRepo;
        }
        public async Task<Result<Guid>> Handle(CreateCaseProceedingCommand request, CancellationToken cancellationToken)
        {
            // ✅ Step 1: Fetch Head Details
            var procDetail = await _ProcRepo.GetByIdAsync(request.HeadId);
            if (procDetail != null && procDetail.Abbreviation == "DISP")
            {
                List<CaseDetailEntity> casesToUpdate = new List<CaseDetailEntity>();
                if (request.MCasIds != null && request.MCasIds.Any())
                {
                    // ✅ Fetch all cases in a single query
                    casesToUpdate = await _CaseRepo.Entites.Where(w => request.MCasIds.Contains(w.Id)).ToListAsync();
                }
                else
                {
                    var cd = await _CaseRepo.GetByIdAsync(request.CaseId);
                    if (cd != null)
                    {
                        casesToUpdate.Add(cd);
                    }
                }
                if (casesToUpdate.Any())
                {
                    foreach (var caseDetail in casesToUpdate)
                    {
                        caseDetail.DisposalDate = request.ProceedingDate;
                    }

                    // ✅ Batch update instead of multiple update calls
                    await _CaseRepo.UpdateRangeAsync(casesToUpdate);
                }
            }

            // ✅ Step 3: Create Main Entity
            var caseProcDetail = await _Repository.GetByIdAsync(request.CaseId, null);
            if (caseProcDetail.Case != null)
            {
                var nextDate = caseProcDetail.Case.NextDate;
                if (request.NextDate < nextDate)
                    return await Result<Guid>.FailAsync("Next proceeding date must be greater than the first next date!");
            }

            // ✅ Step 3: Create Main Entity
            var entity = _mapper.Map<CaseProcedingEntity>(request);
            entity.ProceedingDate = request.ProceedingDate;
            entity.CreatedOn = DateTime.UtcNow;

            List<CaseProcedingEntity> caseProceedings = new List<CaseProcedingEntity>();

            // ✅ Step 4: Process MCasIds (if available)
            if (request.MCasIds != null && request.MCasIds.Count > 0)
            {
                caseProceedings = request.MCasIds.Select(id => new CaseProcedingEntity
                {
                    CaseId = id,
                    HeadId = entity.HeadId,
                    SubHeadId = entity.SubHeadId,
                    StageId = entity.StageId,
                    NextDate = entity.NextDate,
                    ProceedingDate = entity.ProceedingDate,
                    CreatedBy = request.UserId,
                    CreatedOn = DateTime.UtcNow,
                    ProcWork = new ProceedingWorkEntity
                    {
                        LastWorkingDate = entity.ProcWork.LastWorkingDate,
                        Works = entity.ProcWork.Works
                    }
                }).ToList();
            }
            else
            {
                // ✅ Step 5: Fetch Child Cases if No MCasIds
                var childCases = await GetAllChildrenAsync(request.CaseId, request.UserId);
                if (childCases.Any())
                {
                    caseProceedings = childCases.Select(it => new CaseProcedingEntity
                    {
                        CaseId = it.Id,
                        HeadId = entity.HeadId,
                        SubHeadId = entity.SubHeadId,
                        StageId = entity.StageId,
                        NextDate = entity.NextDate,
                        ProceedingDate = entity.ProceedingDate,
                        CreatedBy = request.UserId,
                        CreatedOn = DateTime.UtcNow,
                        ProcWork = new ProceedingWorkEntity
                        {
                            LastWorkingDate = entity.ProcWork.LastWorkingDate,
                            Works = entity.ProcWork.Works
                        }
                    }).ToList();
                }
            }

            // ✅ Step 5: Insert All Entities Efficiently
            //if (request.CaseId != Guid.Empty)
              //  caseProceedings.Add(entity); // Always insert the main entity

            if (caseProceedings.Any())
            {
                await _Repository.AddAsyncRange(caseProceedings); // Make sure AddRangeAsync supports batch inserts
                await _unitOfWork.Commit(cancellationToken);
                return Result<Guid>.Success(entity.Id);
            }
            return Result<Guid>.Fail("No case proceedings added.");
        }

        // ✅ Convert to an async method for better performance
        public async Task<List<CaseDetailEntity>> GetAllChildrenAsync(Guid parentId, string userId)
        {
            var userCases = await _CaseRepo.Entites
                .Where(w => w.CreatedBy == userId)
                .ToListAsync();

            return GetChildrenIteratively(userCases, parentId);
        }

        // ✅ Use an iterative approach instead of recursion to prevent StackOverflow
        private List<CaseDetailEntity> GetChildrenIteratively(List<CaseDetailEntity> userCases, Guid parentId)
        {
            var result = new List<CaseDetailEntity>();
            var stack = new Stack<CaseDetailEntity>();
            var childCases = userCases.Where(c => c.LinkedCaseId == parentId);
            foreach (var childCase in childCases)
            {
                stack.Push(childCase); // Push each item separately
            }
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                result.Add(current);
                foreach (var child in userCases.Where(c => c.LinkedCaseId == current.Id))
                {
                    stack.Push(child);
                }
            }
            return result;
        }

    }
}
