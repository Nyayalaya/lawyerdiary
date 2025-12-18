using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
using CourtApp.Domain.Entities.LawyerDiary;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Clients.Commands
{
    public class CreateClientCommand : IRequest<Result<Guid>>
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string OfficeEmail { get; set; }
        public string Phone { get; set; }
        public string ReferalBy { get; set; }
        public string RegNo { get; set; }
        public string Properiter { get; set; }
        public string ClientType { get; set; }
        public string UserId { get; set; }
    }

    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Result<Guid>>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;
        private readonly IStateMasterRepository _rStateMst;
        private readonly IDistrictMasterRepository _rDistrictMst;
        private readonly IUserCaseRepository _repository;

        private IUnitOfWork _unitOfWork { get; set; }
        public CreateClientCommandHandler(IClientRepository _clientRepository, IUnitOfWork unitOfWork
            , IMapper mapper, IStateMasterRepository rStateMst, IDistrictMasterRepository _rDistrictMst
            , IUserCaseRepository repository)
        {
            this._clientRepository = _clientRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _rStateMst = rStateMst;
            this._rDistrictMst = _rDistrictMst;
            _repository = repository;
        }

        public async Task<Result<Guid>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var detail = _clientRepository.Clients
                .Where(w => w.CreatedBy.Equals(request.UserId)
                        && w.Name.Trim().ToLower().Equals(request.Name.Trim().ToLower())
                        && w.Mobile.Equals(request.Mobile)
                        && w.Address.Trim().ToLower().Equals(request.Address.Trim().ToLower())
                ).FirstOrDefault();
            if (detail == null)
            {
                var entity = _mapper.Map<ClientEntity>(request);
                await _clientRepository.InsertAsync(entity);
                await _unitOfWork.Commit(cancellationToken);
                return Result<Guid>.Success(entity.Id);
            }
            return Result<Guid>.Fail("Error! The client is already exist");
        }
    }
}