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
        public List<CaseHearingDto> CasesHearingDt { get; set; }
    }
    public class CaseHearingDto
    {
        public Guid CaseId { get; set; }
        public DateTime HearingDt { get; set; }
        public string ProcDt { get; set; }
    }
    public class UpdateCaseHearingDatesCommandHandler : IRequestHandler<UpdateCaseHearingDatesCommand, Result<Guid>>
    {
        private readonly IUserCaseRepository _Repository;
        private readonly ICaseProceedingRepository _procRepo;
        private IUnitOfWork _unitOfWork { get; set; }
        public UpdateCaseHearingDatesCommandHandler(IUserCaseRepository _Repository, IUnitOfWork _unitOfWork, ICaseProceedingRepository _procRepo)
        {
            this._Repository = _Repository;
            this._unitOfWork = _unitOfWork;
            this._procRepo = _procRepo;
        }
        public async Task<Result<Guid>> Handle(UpdateCaseHearingDatesCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Get all case IDs from request
            var casesToUpdate = request.CasesHearingDt
                .Select(s => s.CaseId)
                .ToHashSet(); // Improves lookup performance

            // Step 2: Include child case IDs
            var childCases = await _Repository.Entites
                .Where(w => w.LinkedCaseId.HasValue && casesToUpdate.Contains(w.LinkedCaseId.Value))
                .Select(w => w.Id)
                .ToListAsync(cancellationToken);

            foreach (var childId in childCases)
                casesToUpdate.Add(childId); // HashSet ensures uniqueness

            // Step 3: Get latest proceedings per case
            var caseProceedings = await _procRepo.Entities
                .Where(w => casesToUpdate.Contains(w.CaseId))
                .GroupBy(w => w.CaseId)
                .Select(g => g.OrderByDescending(p => p.NextDate.HasValue)
                             .ThenByDescending(p => p.NextDate)
                             .FirstOrDefault())
                .ToListAsync(cancellationToken);

            // Step 4: Update NextDate for cases having proceedings
            var updatedCaseIds = new HashSet<Guid>();

            foreach (var proceeding in caseProceedings)
            {
                if (proceeding != null)
                {
                    var nextDate = request.CasesHearingDt
                        .FirstOrDefault(w => w.CaseId == proceeding.CaseId)?.HearingDt;

                    if (nextDate.HasValue)
                    {
                        proceeding.NextDate = nextDate;
                        updatedCaseIds.Add(proceeding.CaseId);
                    }
                }
            }
            if (caseProceedings.Any())
                await _procRepo.UpdateRangeAsync(caseProceedings);

            // Step 5: Update NextDate for remaining cases (those without proceedings)
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
                    var nextDate = request.CasesHearingDt
                        .FirstOrDefault(w => w.CaseId == entity.Id)?.HearingDt;

                    if (nextDate.HasValue)
                        entity.NextDate = nextDate;
                }

                await _Repository.UpdateRangeAsync(entities);
            }


            await _unitOfWork.Commit(cancellationToken);
            return Result<Guid>.Success(Guid.Empty);

        }
    }
}
