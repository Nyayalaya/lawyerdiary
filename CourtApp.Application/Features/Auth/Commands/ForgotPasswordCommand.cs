using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Auth.Commands
{
    public class ForgotPasswordCommand : IRequest<Result<string>>
    {
        public string Email { get; set; }

        public string Origin { get; set; }
    }
}
