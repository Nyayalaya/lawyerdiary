using AspNetCoreHero.Results;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CourtApp.Application.Features.CaseDetails
{
    public class UpdateCaseNextDateCommand : IRequest<Result<Guid>>
    {
        public List<Guid> CaseIds { get; set; }
        public DateTime NextHearingDate { get; set; }
    }
    public class UpdateCaseNextDateCommandHandler : IRequestHandler<UpdateCaseNextDateCommand, Result<Guid>>
    {
        private readonly IUserCaseRepository _Repository;
        private readonly ICaseProceedingRepository caseProceeding;
        private IUnitOfWork _unitOfWork { get; set; }
        public UpdateCaseNextDateCommandHandler(IUserCaseRepository _Repository, IUnitOfWork _unitOfWork, ICaseProceedingRepository caseProceeding)
        {
            this._Repository = _Repository;
            this._unitOfWork = _unitOfWork;
            this.caseProceeding = caseProceeding;
        }
        public async Task<Result<Guid>> Handle(UpdateCaseNextDateCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Include child cases (linked)
            var childCases = await _Repository.Entites
                .Where(w => w.LinkedCaseId.HasValue && request.CaseIds.Contains(w.LinkedCaseId.Value))
                .ToListAsync(cancellationToken);

            if (childCases?.Any() == true)
            {
                var existing = new HashSet<Guid>(request.CaseIds);
                foreach (var child in childCases)
                {
                    if (existing.Add(child.Id))
                        request.CaseIds.Add(child.Id); // Only add if not already there
                }
            }

            // Step 2: Get latest proceedings per case
            var caseProceedings = await caseProceeding.Entities
               .Where(w => request.CaseIds.Contains(w.CaseId))
               .GroupBy(w => w.CaseId)
               .Select(g => g
                           .OrderByDescending(p => p.ProceedingDate)
                           .FirstOrDefault())
               .ToListAsync(cancellationToken);

            var updatedProceedingCaseIds = new HashSet<Guid>();

            // Step 3: Update NextDate for cases having proceedings
            if (caseProceedings.Any())
            {
                // Filter out valid proceedings (those that meet the date condition)
                var validProceedings = caseProceedings
                    .Where(p => request.NextHearingDate >= p.ProceedingDate)
                    .ToList();

                // Collect invalid ones (optional — if you want to log or return them)
                var invalidProceedings = caseProceedings
                    .Where(p => request.NextHearingDate < p.ProceedingDate)
                    .ToList();

                if (!validProceedings.Any())
                    return await Result<Guid>.FailAsync("Next hearing date must be greater than or equal to the last proceeding date for all cases.");

                // Update only valid proceedings
                foreach (var proceeding in validProceedings)
                {
                    proceeding.NextDate = request.NextHearingDate;
                    updatedProceedingCaseIds.Add(proceeding.CaseId);
                }

                await caseProceeding.UpdateRangeAsync(caseProceedings);
            }

            // Step 4: Find remaining cases without proceedings and update their NextDate
            var missingCaseIds = request.CaseIds.Except(updatedProceedingCaseIds).ToList();

            if (missingCaseIds.Any())
            {
                var entities = await _Repository.Entites
                    .Where(w => missingCaseIds.Contains(w.Id))
                    .ToListAsync(cancellationToken);

                if (entities.Count != missingCaseIds.Count)
                    return await Result<Guid>.FailAsync("Some cases were not found.");

                foreach (var entity in entities)
                {
                    entity.NextDate = request.NextHearingDate;
                }

                await _Repository.UpdateRangeAsync(entities);
            }

            // Step 5: Commit once
            await _unitOfWork.Commit(cancellationToken);
            return await Result<Guid>.SuccessAsync(Guid.Empty);

        }
    }
}
