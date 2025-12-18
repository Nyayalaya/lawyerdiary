using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Clients.Commands
{
    public class CreateClientByCaseIdCommand : IRequest<Result<Guid>>
    {
        public Guid ClientId { get; set; }
        public string UserId { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }
    }


    public class CreateClientByCaseIdCommandHandler : IRequestHandler<CreateClientByCaseIdCommand, Result<Guid>>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;
        private readonly IUserCaseRepository _repository;

        private IUnitOfWork _unitOfWork { get; set; }
        public CreateClientByCaseIdCommandHandler(IClientRepository _clientRepository, IUnitOfWork unitOfWork
            , IMapper mapper, IStateMasterRepository rStateMst, IDistrictMasterRepository _rDistrictMst
            , IUserCaseRepository repository)
        {
            this._clientRepository = _clientRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<Result<Guid>> Handle(CreateClientByCaseIdCommand request, CancellationToken cancellationToken)
        {
            var detail = _clientRepository
                .Clients
                .Where(w => w.Name.Trim().ToLower().Equals(request.Name.Trim().ToLower())
                        && w.Mobile.Equals(request.Mobile) && w.CreatedBy.Equals(request.UserId)
                ).FirstOrDefault();
            if (detail != null)
                return Result<Guid>.Fail("The client is already exist for the logged in user!");
            else
            {
                var cld = await _clientRepository.GetByIdAsync(request.ClientId);
                cld.Id = Guid.NewGuid();
                cld.ReferalBy = "";
                await _clientRepository.InsertAsync(cld);
                await _unitOfWork.Commit(cancellationToken);
                return Result<Guid>.Success(cld.Id);
            }
            //return Result<Guid>.Fail("There is some problem to process the request");
        }
    }
}