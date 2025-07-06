using AspNetCoreHero.Results;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
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
            Guid Id = Guid.Empty;
            foreach (var item in request.CaseIds)
            {
                var entity = await _Repository.GetByIdAsync(item);
                if (entity == null)
                    return Result<Guid>.Fail($"Case is not found.");
                else
                {
                    entity.NextDate = request.NextHearingDate;
                    await _Repository.UpdateAsync(entity);
                    Id = entity.Id;
                }
            }
            await _unitOfWork.Commit(cancellationToken);
            return Result<Guid>.Success(Id);
        }
    }
}
