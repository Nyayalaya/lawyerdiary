using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.Account;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Accounting;
using CourtApp.Domain.Entities.Account;
using MediatR;

namespace CourtApp.Application.Features.Account
{
    public class BillingDetailCreateCommand : IApplicationLayer, IRequest<Result<Guid>>
    {
        public BillingDetailDto BillingDetail { get; set; }
    }
    public class BillingDetailCreateCommandHandler : IRequestHandler<BillingDetailCreateCommand, Result<Guid>>
    {
        private readonly IBillingDetailRepository repository;
        private readonly IUnitOfWork unitOfWork;
        public BillingDetailCreateCommandHandler(IBillingDetailRepository repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(BillingDetailCreateCommand request, CancellationToken cancellationToken)
        {
            // Validate incoming billing detail request
            if (request.BillingDetail == null)
                return await Result<Guid>.FailAsync("No billing detail provided by client.");

            // Map request data to entity
            var billingDetailEntity = new BillingDetailEntity
            {
                AccountNo = request.BillingDetail.AccountNo?.Trim(),
                IfscCode = request.BillingDetail.IfscCode?.Trim(),
                BankName = request.BillingDetail.BankName?.Trim(),
                Branch = request.BillingDetail.Branch?.Trim(),
                GstNo = request.BillingDetail.GstNo?.Trim(),
                PanNumber = request.BillingDetail.PanNumber?.Trim(),
                LawyerId = request.BillingDetail.LawyerId
            };

            // Insert the entity into the database
            await repository.InsertAsync(billingDetailEntity);

            // Commit the transaction
            await unitOfWork.Commit(cancellationToken);

            // Return success response with newly created record's ID
            return await Result<Guid>.SuccessAsync(billingDetailEntity.Id, "User's billing detail added successfully.");

        }
    }
}
