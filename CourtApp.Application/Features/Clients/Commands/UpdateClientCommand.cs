using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Clients.Commands
{
    public class UpdateClientCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
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
    }
    public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;
        private readonly IStateMasterRepository _rStateMst;
        private readonly IDistrictMasterRepository _rDistrictMst;

        public UpdateClientCommandHandler(IClientRepository _clientRepository,
            IUnitOfWork unitOfWork, IStateMasterRepository rStateMst, IDistrictMasterRepository _rDistrictMst,
            IMapper _mapper)
        {
            this._clientRepository = _clientRepository;
            _unitOfWork = unitOfWork;
            this._rDistrictMst = _rDistrictMst;
            this._rStateMst = rStateMst;
            this._mapper = _mapper;

        }

        public async Task<Result<Guid>> Handle(UpdateClientCommand cmd, CancellationToken cancellationToken)
        {
            var detail = await _clientRepository.GetByIdAsync(cmd.Id);
            if (detail == null)
                return Result<Guid>.Fail($"Client Not Found.");
            else
            {
                detail.ClientType = cmd.ClientType;
                detail.Name = cmd.Name;
                detail.Email = cmd.Email;
                detail.OfficeEmail = cmd.OfficeEmail;
                detail.Phone = cmd.Phone;
                detail.Mobile = cmd.Mobile;
                detail.ReferalBy = cmd.ReferalBy;
                detail.Properiter = cmd.Properiter;
                detail.RegNo = cmd.RegNo;
                detail.Address = cmd.Address;
                await _clientRepository.UpdateAsync(detail);
                await _unitOfWork.Commit(cancellationToken);
                return Result<Guid>.Success(cmd.Id);
            }
        }
    }
}