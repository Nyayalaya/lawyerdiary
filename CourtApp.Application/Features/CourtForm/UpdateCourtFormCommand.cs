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
    public class UpdateCourtFormCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public int StateId { get; set; }
        public string LanguageCode { get; set; }
        public Guid CourtTypeId { get; set; }
        public Guid? CaseCategoryId { get; set; }
        public string FormName { get; set; }
        public string FormTemplate { get; set; }
        public Guid? CaseTypeId { get; set; }
    }
    public class UpdateCourtFormCommandHandler : IRequestHandler<UpdateCourtFormCommand, Result<Guid>>
    {
        private readonly ICourtFormTypeRepository repository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public UpdateCourtFormCommandHandler(IUnitOfWork unitOfWork,
            ICourtFormTypeRepository repository, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<Guid>> Handle(UpdateCourtFormCommand request, CancellationToken cancellationToken)
        {
            // Check if the record exists
            var formDetail = await repository.Entities
                .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            if (formDetail == null)
                return await Result<Guid>.FailAsync("The record does not exist!");

            // Check for duplicate form name (excluding the current Id)
            bool isDuplicateName = await repository.Entities
                .AnyAsync(f => f.FormName == request.FormName && f.Id != request.Id, cancellationToken);

            if (isDuplicateName)
                return await Result<Guid>.FailAsync("A record with the same form name already exists.");

            // Use AutoMapper to map updated fields to the existing entity
            var mappedEntity = mapper.Map(request, formDetail);

            // Persist changes
            await repository.UpdateAsync(mappedEntity);
            await unitOfWork.Commit(cancellationToken);

            return await Result<Guid>.SuccessAsync(formDetail.Id, "The record was updated successfully!");


        }
    }
}
