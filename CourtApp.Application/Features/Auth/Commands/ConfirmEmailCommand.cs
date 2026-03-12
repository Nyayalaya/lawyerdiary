using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Auth.Commands
{
    public class ConfirmEmailCommand : IRequest<Result<string>>
    {
        public string UserId { get; set; }

        public string Code { get; set; }
    }
}
