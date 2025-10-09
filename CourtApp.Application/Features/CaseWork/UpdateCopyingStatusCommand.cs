using AspNetCoreHero.Results;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using CourtApp.Domain.Entities.LawyerDiary;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseWork
{
    public class UpdateCopyingStatusCommand : IRequest<Result<Guid>>
    {
        public List<Guid> CaseId { get; set; }
        public int Status { get; set; }
    }
    public class UpdateCopyingStatusCommandHandler : IRequestHandler<UpdateCopyingStatusCommand, Result<Guid>>
    {
        private readonly ICaseWorkRepository _Repository;
        private readonly ICaseProceedingRepository _ProcRepo;
        private readonly IWorkMasterRepository _WorkMasterRepo;
        private IUnitOfWork _unitOfWork { get; set; }
        public UpdateCopyingStatusCommandHandler(ICaseWorkRepository _Repository,
            IUnitOfWork _unitOfWork, ICaseProceedingRepository _ProcRepo,
            IWorkMasterRepository _WorkMasterRepo)
        {
            this._Repository = _Repository;
            this._unitOfWork = _unitOfWork;
            this._ProcRepo = _ProcRepo;
            this._WorkMasterRepo = _WorkMasterRepo;
        }
        public async Task<Result<Guid>> Handle(UpdateCopyingStatusCommand request, CancellationToken cancellationToken)
        {
            var caseIds = request.CaseId.Distinct().ToList();
            // Get latest entities in a single query (instead of per ID)
            var latestEntities = await _ProcRepo.Entities
                .Where(e => caseIds.Contains(e.CaseId))
                .GroupBy(e => e.CaseId)
                .Select(g => g
                    .OrderByDescending(e => e.ProceedingDate)
                    .FirstOrDefault())
                .ToListAsync(cancellationToken);

            if(latestEntities.Count==0)
                return await Result<Guid>.FailAsync($"The proceedings of cases is not found!");

            foreach (var entity in latestEntities)
            {
                if (entity?.ProcWork?.Works == null)
                    continue;

                // Get the IDs of the WorkTypes applied to the entity
                var appliedWorkTypeIds = entity.ProcWork.Works
                    .Select(w => w.WorkTypeId)
                    .ToHashSet(); // Use HashSet for faster lookup

                // Get the ID of the "copying" work type, case-insensitive match
                var copyingWorkTypeId = _WorkMasterRepo.Entities
                    .Where(w => appliedWorkTypeIds.Contains(w.Id) && w.Work_En.ToLower()== "coping")
                    .Select(w => w.Id)
                    .FirstOrDefault();

                // If a matching "copying" work type was found, update relevant work entities
                if (copyingWorkTypeId != Guid.Empty)
                {
                    foreach (var work in entity.ProcWork.Works.Where(w => w.WorkTypeId == copyingWorkTypeId))
                    {
                        work.ReceivedOn = DateTime.Now;
                        work.Status = request.Status;
                    }
                }
                await _ProcRepo.UpdateAsync(entity);
            }
            await _unitOfWork.Commit(cancellationToken);
            return await Result<Guid>.SuccessAsync($"Case copying receive status update successfull!");
        }
    }
}
