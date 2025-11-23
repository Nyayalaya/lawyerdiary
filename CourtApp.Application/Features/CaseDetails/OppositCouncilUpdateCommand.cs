using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseDetails
{
    public class OppositCouncilUpdateCommand : IRequest<Result<Guid>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid CaseId { get; set; }
    }
}
