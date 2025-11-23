using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseDetails
{
    public class CreateCaseAssignedCommand : IRequest<Result<Guid>>
    {
        public Guid CaseId { get; set; }
        public Guid LawyerId { get; set; }
        public Guid UserId { get; set; }
    }
    public class CreateCaseAssignedCommandHandler : IRequestHandler<CreateCaseAssignedCommand, Result<Guid>>
    {
        private readonly ICaseAssignedRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CreateCaseAssignedCommandHandler(ICaseAssignedRepository _repository, IMapper _mapper, IUnitOfWork _unitOfWork)
        {
            this._repository = _repository;
            this._mapper = _mapper;
            this._unitOfWork = _unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CreateCaseAssignedCommand request, CancellationToken cancellationToken)
        {
            if (request != null)
            {
                var entity = _mapper.Map<AssignCaseEntity>(request);
                await _repository.InsertAsync(entity);
                await _unitOfWork.Commit(cancellationToken);
                return Result<Guid>.Success(entity.Id);
            }
            await _unitOfWork.Commit(cancellationToken);
            return Result<Guid>.Fail("There is an issue in request model");
        }
    }
}
