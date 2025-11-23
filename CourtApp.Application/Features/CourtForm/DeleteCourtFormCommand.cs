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
using Microsoft.EntityFrameworkCore;

namespace CourtApp.Application.Features.CourtForm
{
    public class DeleteCourtFormCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
    public class DeleteCourtFormCommandHandler : IRequestHandler<DeleteCourtFormCommand, Result<Guid>>
    {
        private readonly ICourtFormTypeRepository repository;
        private readonly IUnitOfWork unitOfWork;
        public DeleteCourtFormCommandHandler(IUnitOfWork unitOfWork,
            ICourtFormTypeRepository repository)
        {
            this.unitOfWork = unitOfWork;
            this.repository = repository;
        }
        public async Task<Result<Guid>> Handle(DeleteCourtFormCommand request, CancellationToken cancellationToken)
        {
            var formDetail = await repository.Entities
                .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            if (formDetail == null)
                return await Result<Guid>.FailAsync("The record does not exist!");

            // Proceed with deletion
            await repository.DeleteAsync(formDetail);
            await unitOfWork.Commit(cancellationToken);

            return await Result<Guid>.SuccessAsync(formDetail.Id, "The record was deleted successfully!");

        }
    }
}
