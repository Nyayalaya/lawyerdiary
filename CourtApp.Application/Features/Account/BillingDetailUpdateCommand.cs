using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.Account;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Accounting;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CourtApp.Application.Features.Account
{
    public class BillingDetailUpdateCommand : IApplicationLayer,IRequest<Result<Guid>>
    {
        public BillingDetailDto BillingDetail { get; set; }
    }
    public class BillingDetailUpdateCommandHandler : IRequestHandler<BillingDetailUpdateCommand, Result<Guid>>
    {
        private readonly IBillingDetailRepository repository;
        private readonly IUnitOfWork unitOfWork;
        public BillingDetailUpdateCommandHandler(IBillingDetailRepository repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(BillingDetailUpdateCommand request, CancellationToken cancellationToken)
        {
            // Validate input
            if (request.BillingDetail == null)
                return await Result<Guid>.FailAsync("No billing detail provided by client.");

            // Fetch the existing billing detail from DB
            var existingDetail = await repository.Entities
                .FirstOrDefaultAsync(x => x.LawyerId == request.BillingDetail.LawyerId);

            if (existingDetail == null)
                return await Result<Guid>.FailAsync("No existing billing detail found for the given lawyer.");

            // Update the entity with new values
            existingDetail.AccountNo = request.BillingDetail.AccountNo?.Trim();
            existingDetail.IfscCode = request.BillingDetail.IfscCode?.Trim();
            existingDetail.BankName = request.BillingDetail.BankName?.Trim();
            existingDetail.Branch = request.BillingDetail.Branch?.Trim();
            existingDetail.GstNo = request.BillingDetail.GstNo?.Trim();
            existingDetail.PanNumber = request.BillingDetail.PanNumber?.Trim();

            // Mark the entity as modified
            await repository.UpdateAsync(existingDetail);

            // Commit the transaction
            await unitOfWork.Commit(cancellationToken);

            // Return success with the updated entity's ID
            return await Result<Guid>.SuccessAsync(existingDetail.Id, "User's billing detail updated successfully.");


        }
    }
}
