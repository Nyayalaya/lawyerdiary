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
        private IUnitOfWork _unitOfWork { get; set; }
        public UpdateCopyingStatusCommandHandler(ICaseWorkRepository _Repository,
            IUnitOfWork _unitOfWork, ICaseProceedingRepository _ProcRepo)
        {
            this._Repository = _Repository;
            this._unitOfWork = _unitOfWork;
            this._ProcRepo = _ProcRepo;
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

                foreach (var workEntity in entity.ProcWork.Works)
                {
                    workEntity.ReceivedOn = DateTime.Now;
                    workEntity.Status = request.Status;
                }
                await _ProcRepo.UpdateAsync(entity);
            }
            await _unitOfWork.Commit(cancellationToken);
            return await Result<Guid>.SuccessAsync($"Case copying receive status update successfull!");
        }
    }
}
