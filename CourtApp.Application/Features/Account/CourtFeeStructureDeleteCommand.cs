using AspNetCoreHero.Results;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Account
{
    public class CourtFeeStructureDeleteCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }       
    }

    public class DeleteCourtFeeStructureCommandHandler : IRequestHandler<CourtFeeStructureDeleteCommand, Result<Guid>>
    {
        private readonly ICourtFeeStructureRepository repository;
        private IUnitOfWork _unitOfWork { get; set; }
        public DeleteCourtFeeStructureCommandHandler(ICourtFeeStructureRepository repository, IUnitOfWork _unitOfWork)
        {
            this.repository = repository;
            this._unitOfWork = _unitOfWork;

        }
        public async Task<Result<Guid>> Handle(CourtFeeStructureDeleteCommand request, CancellationToken cancellationToken)
        {
            var detail = await repository.GetByIdAsync(request.Id);
            if (detail == null)
                return Result<Guid>.Fail($"Fee structure detail Not Found.");
            else
            {
                await repository.DeleteAsync(detail);
                await _unitOfWork.Commit(cancellationToken);
                return Result<Guid>.Success(detail.Id);
            }
        }
    }
}
