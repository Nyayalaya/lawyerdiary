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
    public class UpdateCaseProceedingCommand : IRequest<Result<Guid>>
    {
        public Guid CaseId { get; set; }
        public Guid HeadId { get; set; }
        public Guid SubHeadId { get; set; }
        public Guid? StageId { get; set; }
        public DateTime? NextDate { get; set; }
        public DateTime ProceedingDate { get; set; }
        public string Remark { get; set; }
        public ProceedingWorkDto ProcWork { get; set; }
        public string UserId { get; set; }
        public List<Guid> MCasIds { get; set; }
    }
    public class UpdateCaseProceedingCommandHandler : IRequestHandler<UpdateCaseProceedingCommand, Result<Guid>>
    {
        private readonly ICaseProceedingRepository _Repository;
        private readonly IUserCaseRepository _CaseRepo;
        private readonly IProceedingHeadRepository _ProcRepo;
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork { get; set; }
        public UpdateCaseProceedingCommandHandler(ICaseProceedingRepository _Repository,
            IMapper _mapper,
            IUnitOfWork _unitOfWork,
            IUserCaseRepository _CaseRepo,
            IProceedingHeadRepository _ProcRepo
            )
        {
            this._Repository = _Repository;
            this._mapper = _mapper;
            this._unitOfWork = _unitOfWork;
            this._ProcRepo = _ProcRepo;
            this._CaseRepo = _CaseRepo;
        }
        public async Task<Result<Guid>> Handle(UpdateCaseProceedingCommand request, CancellationToken cancellationToken)
        {
            // ✅ Step 1: Fetch Head Details
            var procDetail = await _ProcRepo.GetByIdAsync(request.HeadId);
            if (procDetail != null && procDetail.Abbreviation == "DISP")
            {
                List<CaseDetailEntity> casesToUpdate = new List<CaseDetailEntity>();
                if (request.MCasIds != null && request.MCasIds.Any())
                {
                    // ✅ Fetch all cases in a single query
                    casesToUpdate = await _CaseRepo.Entites
                        .Where(w => request.MCasIds.Contains(w.Id)).ToListAsync();
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
            // ✅ Step 2: Get Proceedings and Work detail
            var proceedings = request.ProcWork;
            var procWorks = proceedings.Workdt.Select(s => new ProcWorkEntity
            {
                WorkId = s.WorkId,
                WorkTypeId = s.WorkTypeId
            }).ToList();


            // ✅ Step 3: Create Main Entity
            var entity = await _Repository.GetByIdAsync(request.CaseId, null);
            entity.NextDate = request.NextDate;
            entity.ProceedingDate = request.ProceedingDate;
            entity.HeadId = request.HeadId;
            entity.SubHeadId = request.SubHeadId;
            entity.StageId = request.StageId;
            entity.ProcWork = new ProceedingWorkEntity
            {
                LastWorkingDate = proceedings?.WorkingDate,
                Works = procWorks
            };

            await _Repository.UpdateAsync(entity);
            List<CaseProcedingEntity> caseProceedings = new List<CaseProcedingEntity>();

            var childCases = await GetAllChildrenAsync(request.CaseId, request.UserId);
            if (childCases.Any())
            {
                var childCaseIds = childCases.Select(s => s.Id).ToList();

                // Optional: only needed if you plan to use childProcs later
                var childProcs = await _Repository
                    .Entities
                    .Where(w => childCaseIds.Contains(w.CaseId))
                    .ToListAsync();

                foreach (var ch in childProcs)
                {
                    ch.HeadId = request.HeadId;
                    ch.SubHeadId = request.SubHeadId;
                    ch.StageId = request.StageId;
                    ch.NextDate = request.NextDate;
                    ch.ProceedingDate = request.ProceedingDate;
                    ch.ProcWork = new ProceedingWorkEntity()
                    {
                        LastWorkingDate = proceedings?.WorkingDate,
                        Works = procWorks
                    };
                    await _Repository.UpdateAsync(ch);
                }

                //caseProceedings = childProcs.Select(it => new CaseProcedingEntity
                //{
                //    CaseId = it.CaseId,
                //    HeadId = request.HeadId,
                //    SubHeadId = request.SubHeadId,
                //    StageId = request.StageId,
                //    NextDate = request.NextDate,
                //    ProceedingDate = request.ProceedingDate,
                //    ProcWork = new ProceedingWorkEntity
                //    {
                //        LastWorkingDate = proceedings?.WorkingDate,
                //        Works = procWorks
                //    }
                //}).ToList();
            }
            //caseProceedings.Add(entity);
            await _Repository.UpdateRangeAsync(caseProceedings);
            await _unitOfWork.Commit(cancellationToken);
            return Result<Guid>.Success(Guid.Empty);

            //var ProcDetail = await _ProcRepo.GetByIdAsync(request.HeadId);
            //if (ProcDetail != null && ProcDetail.Abbreviation == "DISP")
            //{
            //    var CaseDetail = await _CaseRepo.GetByIdAsync(request.CaseId);
            //    CaseDetail.DisposalDate = request.ProceedingDate;
            //    await _CaseRepo.UpdateAsync(CaseDetail);
            //}
            //var entity = await _Repository.GetByIdAsync(request.CaseId, null);
            ////var childCases = await GetAllChildrenAsync(request.CaseId, request.UserId);
            //if (entity != null)
            //{
            //    entity.NextDate = request.NextDate;
            //    entity.HeadId = request.HeadId;
            //    entity.SubHeadId = request.SubHeadId;
            //    entity.StageId = request.StageId;
            //    entity.ProceedingDate = request.ProceedingDate;
            //    entity.ProcWork = _mapper.Map<ProceedingWorkEntity>(request.ProcWork);
            //    await _Repository.UpdateAsync(entity);
            //    await _unitOfWork.Commit(cancellationToken);
            //    return Result<Guid>.Success(entity.Id);
            //}
            //else
            //{
            //    var obj = _mapper.Map<CaseProcedingEntity>(request);
            //    obj.ProceedingDate = entity.NextDate != null ? entity.NextDate.Value : null;
            //    await _Repository.AddAsync(obj);
            //    await _unitOfWork.Commit(cancellationToken);
            //    return Result<Guid>.Success(obj.Id); ;
            //}
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
