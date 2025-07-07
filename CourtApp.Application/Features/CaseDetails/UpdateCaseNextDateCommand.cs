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
        private IUnitOfWork _unitOfWork { get; set; }
        public UpdateCaseNextDateCommandHandler(IUserCaseRepository _Repository, IUnitOfWork _unitOfWork)
        {
            this._Repository = _Repository;
            this._unitOfWork = _unitOfWork;
        }
        public async Task<Result<Guid>> Handle(UpdateCaseNextDateCommand request, CancellationToken cancellationToken)
        {
            var entities = await _Repository
                .Entites.AsNoTracking()
                .Where(w => request.CaseIds.Contains(w.Id))
                .ToListAsync();

            if (entities.Count != request.CaseIds.Count)
                return Result<Guid>.Fail("Some cases were not found.");

            foreach (var entity in entities)
            {
                entity.NextDate = request.NextHearingDate;
            }

            await _Repository.UpdateRangeAsync(entities);
            await _unitOfWork.Commit(cancellationToken);

            return Result<Guid>.Success(Guid.Empty);
        }
    }
}
