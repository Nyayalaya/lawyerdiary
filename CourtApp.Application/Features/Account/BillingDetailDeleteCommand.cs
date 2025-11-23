using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;

namespace CourtApp.Application.Features.Account
{
    public class BillingDetailDeleteCommand : IApplicationLayer, IRequest<Result<Guid>>
    {
        public string LawyerId { get; set; }
    }
}
