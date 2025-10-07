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
            // Step 1: Create HashSet of case IDs (for fast lookup)
            var casesToUpdate = request.CasesHearingDt
                .Select(s => s.CaseId)
                .ToHashSet();

            // Step 2: Fetch all child cases linked to these case IDs
            var childCases = await _Repository.Entites
                .Where(w => w.LinkedCaseId.HasValue && casesToUpdate.Contains(w.LinkedCaseId.Value))
                .Select(w => w.Id)
                .ToListAsync(cancellationToken);

            //  Add child case IDs to the update list
            foreach (var childId in childCases)
                casesToUpdate.Add(childId); // HashSet avoids duplicates

            // Step 3: Build a dictionary for quick hearing date lookup (parent case ID → Hearing Date)
            var hearingDateMap = request.CasesHearingDt
                .ToDictionary(x => x.CaseId, x => x.HearingDt);

            // Step 4: Fetch all proceedings for the given case IDs (once)
            var updatedCaseIds = new HashSet<Guid>();
            try
            {

                var allProceedings = await _procRepo.Entities
                    .Where(w => casesToUpdate.Contains(w.CaseId))
                    .ToListAsync(cancellationToken);
                // Step 4.1: If no proceedings, skip to next step
                if (allProceedings.Any())
                {
                    // Step 4.2: Group by CaseId and pick latest proceeding (by date or next hearing date)
                    var caseProceedings = await _procRepo.Entities
                            .Where(w => casesToUpdate.Contains(w.CaseId))
                            .GroupBy(w => w.CaseId)
                            .Select(g => g
                                .OrderByDescending(p => p.ProceedingDate)
                                .FirstOrDefault())
                            .ToListAsync(cancellationToken);

                    // Step 4.3:Track which case IDs were updated through proceedings
                    foreach (var proceeding in caseProceedings)
                    {
                        if (proceeding != null)
                        {
                            // Get parent case ID
                            var parentId = await _Repository.Entites
                                .Where(e => e.Id == proceeding.CaseId)
                                .Select(e => e.LinkedCaseId ?? e.Id)
                                .FirstOrDefaultAsync(cancellationToken);

                            // Set NextDate if available
                            if (hearingDateMap.TryGetValue(parentId, out var nextDate))
                            {
                                proceeding.NextDate = nextDate;
                                proceeding.LastModifiedOn = DateTime.Now;
                                proceeding.LastModifiedBy = request.UserId;
                                updatedCaseIds.Add(proceeding.CaseId);
                            }
                        }
                    }
                    // Step 4.4: Save updated proceedings
                    if (caseProceedings.Any())
                        await _procRepo.UpdateRangeAsync(caseProceedings);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Step 5: Update `NextDate` in case table for cases not updated in proceedings
            var missingCaseIds = casesToUpdate.Except(updatedCaseIds).ToList();
            if (missingCaseIds.Any())
            {
                var entities = await _Repository.Entites
                    .Where(w => missingCaseIds.Contains(w.Id))
                    .ToListAsync(cancellationToken);

                if (entities.Count != missingCaseIds.Count)
                    return await Result<Guid>.FailAsync("Some cases were not found.");

                foreach (var entity in entities)
                {
                    var parentId = entity.LinkedCaseId ?? entity.Id;

                    if (hearingDateMap.TryGetValue(parentId, out var nextDate))
                        entity.NextDate = nextDate;
                }

                await _Repository.UpdateRangeAsync(entities);
            }

            // Step 6: Save changes
            await _unitOfWork.Commit(cancellationToken);
            return Result<Guid>.Success(Guid.Empty);
        }


        //public async Task<Result<Guid>> Handle(UpdateCaseHearingDatesCommand request, CancellationToken cancellationToken)
        //{
        //    // Step 1: Create HashSet of case IDs (for fast lookup)
        //    var casesToUpdate = request.CasesHearingDt
        //        .Select(s => s.CaseId)
        //        .ToHashSet();

        //    // Step 2: Fetch all child cases linked to these case IDs
        //    var childCases = await _Repository.Entites
        //        .Where(w => w.LinkedCaseId.HasValue && casesToUpdate.Contains(w.LinkedCaseId.Value))
        //        .Select(w => w.Id)
        //        .ToListAsync(cancellationToken);

        //    // Add child case IDs to the update list
        //    foreach (var childId in childCases)
        //        casesToUpdate.Add(childId); // HashSet avoids duplicates

        //    // Step 3: Build a dictionary for quick hearing date lookup (parent case ID → Hearing Date)
        //    var hearingDateMap = request.CasesHearingDt
        //        .ToDictionary(x => x.CaseId, x => x.HearingDt);

        //    // Step 4: Get latest proceedings for all cases
        //    var proceedings = await _procRepo.Entities
        //        .Where(w => casesToUpdate.Contains(w.CaseId))
        //        .ToListAsync(cancellationToken);

        //    if (!proceedings.Any())
        //    {

        //    }


        //        var caseProceedings = await _procRepo.Entities
        //            .Where(w => casesToUpdate.Contains(w.CaseId))
        //            .GroupBy(w => w.CaseId)
        //            .Select(g => g
        //                .OrderByDescending(p => p.ProceedingDate)
        //                .FirstOrDefault())
        //            .ToListAsync(cancellationToken);

        //    // Track which case IDs were updated through proceedings
        //    var updatedCaseIds = new HashSet<Guid>();

        //    foreach (var proceeding in caseProceedings)
        //    {
        //        if (proceeding != null)
        //        {
        //            // Get parent case ID
        //            var parentId = await _Repository.Entites
        //                .Where(e => e.Id == proceeding.CaseId)
        //                .Select(e => e.LinkedCaseId ?? e.Id)
        //                .FirstOrDefaultAsync(cancellationToken);

        //            // Set NextDate if available
        //            if (hearingDateMap.TryGetValue(parentId, out var nextDate))
        //            {
        //                proceeding.NextDate = nextDate;
        //                proceeding.LastModifiedOn = DateTime.Now;
        //                proceeding.LastModifiedBy = request.UserId;
        //                updatedCaseIds.Add(proceeding.CaseId);
        //            }
        //        }
        //    }

        //    if (caseProceedings.Any())
        //        await _procRepo.UpdateRangeAsync(caseProceedings);

        //    // Step 5: Update `NextDate` in case table for cases without proceedings
        //    var missingCaseIds = casesToUpdate.Except(updatedCaseIds).ToList();

        //    if (missingCaseIds.Any())
        //    {
        //        var entities = await _Repository.Entites
        //            .Where(w => missingCaseIds.Contains(w.Id))
        //            .ToListAsync(cancellationToken);

        //        if (entities.Count != missingCaseIds.Count)
        //            return await Result<Guid>.FailAsync("Some cases were not found.");

        //        foreach (var entity in entities)
        //        {
        //            var parentId = entity.LinkedCaseId ?? entity.Id;

        //            if (hearingDateMap.TryGetValue(parentId, out var nextDate))
        //                entity.NextDate = nextDate;
        //        }

        //        await _Repository.UpdateRangeAsync(entities);
        //    }

        //    // Step 6: Save changes
        //    await _unitOfWork.Commit(cancellationToken);
        //    return Result<Guid>.Success(Guid.Empty);
        //}
    }
}
