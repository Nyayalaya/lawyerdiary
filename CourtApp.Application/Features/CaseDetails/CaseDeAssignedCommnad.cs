using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;

namespace CourtApp.Application.Features.CaseDetails
{
    public class CaseDeAssignedCommnad : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public Guid LawyerId { get; set; }
        public string Remark { get; set; }
    }
    public class CaseDeAssignedCommnadHandler : IRequestHandler<CaseDeAssignedCommnad, Result<Guid>>
    {
        private readonly ICaseAssignedRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CaseDeAssignedCommnadHandler(ICaseAssignedRepository repository, IUnitOfWork unitOfWork)
        {
            this._repository = repository; ;
            this._unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CaseDeAssignedCommnad request, CancellationToken cancellationToken)
        {
            var assignedCaseDetails = _repository
                .Entities
                .Where(w => w.CaseId == request.Id
                && w.LawyerId == request.LawyerId).ToList();
            if (assignedCaseDetails == null)
                return await Result<Guid>.FailAsync("There is no case avaiable for de-assigning");

            var deAssigned = _repository.DeleteRangeAsync(assignedCaseDetails);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<Guid>.SuccessAsync();
        }
    }
}
