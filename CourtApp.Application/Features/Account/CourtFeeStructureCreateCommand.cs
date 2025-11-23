using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.Account;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Account
{
    public class CourtFeeStructureCreateCommand : IApplicationLayer, IRequest<Result<Guid>>
    {
        public string StateCode { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double Rate { get; set; }
        public double FixAmount { get; set; }
    }

    public class CreateCourtFeeStructureCommandHandler : IRequestHandler<CourtFeeStructureCreateCommand, Result<Guid>>
    {
        private readonly ICourtFeeStructureRepository repository;
        private readonly IMapper mapper;
        private IUnitOfWork _unitOfWork { get; set; }
        public CreateCourtFeeStructureCommandHandler(ICourtFeeStructureRepository repository, IMapper mapper, IUnitOfWork _unitOfWork)
        {
            this.repository = repository;
            this.mapper = mapper;
            this._unitOfWork = _unitOfWork;

        }
        public async Task<Result<Guid>> Handle(CourtFeeStructureCreateCommand request, CancellationToken cancellationToken)
        {
            var mappeddata = mapper.Map<CourtFeeStructureEntity>(request);
            await repository.InsertAsync(mappeddata);
            await _unitOfWork.Commit(cancellationToken);
            return Result<Guid>.Success(mappeddata.Id);
        }
    }
}
