using AspNetCoreHero.Results;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Account
{
    public class CourtFeeStructureUpdateCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public int StateCode { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double Rate { get; set; }
        public double FixAmount { get; set; }
    }

    public class UpdateCourtFeeStructureCommandHandler : IRequestHandler<CourtFeeStructureUpdateCommand, Result<Guid>>
    {
        private readonly ICourtFeeStructureRepository repository;
       private IUnitOfWork _unitOfWork { get; set; }
        public UpdateCourtFeeStructureCommandHandler(ICourtFeeStructureRepository repository,IUnitOfWork _unitOfWork)
        {
            this.repository = repository;          
            this._unitOfWork = _unitOfWork;

        }
        public async Task<Result<Guid>> Handle(CourtFeeStructureUpdateCommand request, CancellationToken cancellationToken)
        {
            var detail = await repository.GetByIdAsync(request.Id);
            if (detail == null)
                return Result<Guid>.Fail($"Fee structure detail Not Found.");
            else
            {               
                detail.State.Id = request.StateCode;
                detail.MaxValue = request.MaxValue;
                detail.MinValue = request.MinValue;
                detail.Rate = request.Rate;
                detail.FixAmount = request.FixAmount;
                detail.LastModifiedOn = DateTime.Now;
                await repository.UpdateAsync(detail);
                await _unitOfWork.Commit(cancellationToken);
                return Result<Guid>.Success(detail.Id);
            }
        }
    }
}
