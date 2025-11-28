using AspNetCoreHero.Results;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseDetails
{
    public class UpdateCaseHearingDatesCommand : IRequest<Result<Guid>>
    {
        public string UserId { get; set; }
        public List<CaseHearingDto> CasesHearingDt { get; set; }
    }
    public class CaseHearingDto
    {
        public Guid CaseId { get; set; }
        public DateTime HearingDt { get; set; }
        public string ProcDt { get; set; }
        public bool IsParent { get; set; }
    }
    public class UpdateCaseHearingDatesCommandHandler : IRequestHandler<UpdateCaseHearingDatesCommand, Result<Guid>>
    {
        private readonly IUserCaseRepository _Repository;
        private readonly ICaseProceedingRepository _procRepo;
        private IUnitOfWork _unitOfWork { get; set; }
        public UpdateCaseHearingDatesCommandHandler(IUserCaseRepository _Repository,
            IUnitOfWork _unitOfWork, ICaseProceedingRepository _procRepo)
        {
            this._Repository = _Repository;
            this._unitOfWork = _unitOfWork;
            this._procRepo = _procRepo;
        }
        public async Task<Result<Guid>> Handle(UpdateCaseHearingDatesCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Prepare case IDs (including child cases)
            var caseIdsToUpdate = new HashSet<Guid>(request.CasesHearingDt.Select(x => x.CaseId));

            foreach (var item in request.CasesHearingDt.Where(x => x.IsParent))
            {
                var childIds = _Repository.Entites
                    .Where(w => w.LinkedCaseId == item.CaseId)
                    .Select(w => w.Id)
                    .ToList();

                foreach (var childId in childIds)
                    caseIdsToUpdate.Add(childId);
            }

            // Step 2: Build hearing date lookup dictionary (Parent CaseId => Hearing Date)
            var hearingDateMap = request.CasesHearingDt
                .ToDictionary(x => x.CaseId, x => x.HearingDt);

            // Step 3: Fetch all proceedings for these cases
            var allProceedings = _procRepo.Entities
                .Where(w => caseIdsToUpdate.Contains(w.CaseId))
                .ToList();

            // Step 4: Get latest proceeding per case (safe from null issues)
            var latestProceedingsByCase = allProceedings
                .GroupBy(p => p.CaseId)
                .Select(g => g.OrderByDescending(p => p.ProceedingDate).FirstOrDefault())
                .Where(p => p != null)
                .ToList();

            // Step 5: Update proceedings with hearing dates
            if (latestProceedingsByCase.Any())
            {
                foreach (var proceeding in latestProceedingsByCase)
                {
                    // Get parent case ID if any, otherwise self
                    var parentId = _Repository.Entites
                        .Where(e => e.Id == proceeding.CaseId)
                        .Select(e => e.LinkedCaseId ?? e.Id)
                        .FirstOrDefault();

                    // Apply hearing date if matched
                    if (hearingDateMap.TryGetValue(parentId, out var hearingDate))
                    {
                        proceeding.NextDate = hearingDate;

                    }
                }

                await _procRepo.UpdateRangeAsync(latestProceedingsByCase);
            }

            // Step 6: Handle cases without any proceedings
            var updatedIds = latestProceedingsByCase.Select(p => p.CaseId).ToHashSet();
            var missingCaseIds = caseIdsToUpdate.Except(updatedIds).ToList();

            if (missingCaseIds.Any())
            {
                var casesToUpdateNextDate = _Repository.Entites
                    .Where(c => missingCaseIds.Contains(c.Id))
                    .ToList();

                foreach (var caseEntity in casesToUpdateNextDate)
                {
                    var parentId = caseEntity.LinkedCaseId ?? caseEntity.Id;

                    if (hearingDateMap.TryGetValue(parentId, out var hearingDate))
                    {
                        caseEntity.NextDate = hearingDate;
                    }
                }

                await _Repository.UpdateRangeAsync(casesToUpdateNextDate);
            }

            // Step 7: Save all changes
            int result = await _unitOfWork.Commit(cancellationToken);
            if (result == -1)
                return await Result<Guid>.FailAsync("There are issues to updated the selected record!");
            return await Result<Guid>.SuccessAsync("Selected records updated successfully!");

        }

    }
}
