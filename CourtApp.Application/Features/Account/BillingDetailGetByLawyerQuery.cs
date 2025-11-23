using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.Account;
using CourtApp.Application.DTOs.Cadre;
using CourtApp.Application.Interfaces.Repositories.Accounting;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CourtApp.Application.Features.Account
{
    public class BillingDetailGetByLawyerQuery : IApplicationLayer, IRequest<Result<BillingDetailDto>>
    {
        public string LawyerId { get; set; }
    }
    public class BillingDetailGetByLawyerQueryHandler : IRequestHandler<BillingDetailGetByLawyerQuery, Result<BillingDetailDto>>
    {
        private readonly IBillingDetailRepository repository;        
        public BillingDetailGetByLawyerQueryHandler(IBillingDetailRepository repository)
        {
            this.repository = repository;            
        }
        public async Task<Result<BillingDetailDto>> Handle(BillingDetailGetByLawyerQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.LawyerId))
                return await Result<BillingDetailDto>.FailAsync("Invalid lawyer ID.");

            var billingDetailDto = await repository.Entities
                .Where(w => w.LawyerId == request.LawyerId)
                .Select(w => new BillingDetailDto
                {
                    Id = w.Id,
                    LawyerId = w.LawyerId,
                    AccountNo = w.AccountNo,
                    BankName = w.BankName,
                    Branch = w.Branch,
                    GstNo = w.GstNo,
                    IfscCode = w.IfscCode,
                    PanNumber = w.PanNumber,
                })
                .FirstOrDefaultAsync();

            return billingDetailDto is null
                ? await Result<BillingDetailDto>.FailAsync("No billing detail available.")
                : await Result<BillingDetailDto>.SuccessAsync(billingDetailDto);

        }
    }
}
