using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.FormBuilder;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.FormBuilder;
using CourtApp.Domain.Entities.FormBuilder;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.FormBuilder
{
    public class CreateFormBuilderCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public string FormName { get; set; }       
        public FormFieldsDto Form { get; set; }
    }
    public class CreateFormBuilderCommandHandler : IRequestHandler<CreateFormBuilderCommand, Result<Guid>>
    {
        private readonly IFormBuilderRepository repository;
        private IUnitOfWork _UoW { get; set; }
        private readonly IMapper _mapper;

        public CreateFormBuilderCommandHandler(IFormBuilderRepository repository, IUnitOfWork _UoW, IMapper _mapper)
        {
            this.repository = repository;
            this._mapper = _mapper;
            this._UoW = _UoW;
        }
        public async Task<Result<Guid>> Handle(CreateFormBuilderCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.FormName))
                return await Result<Guid>.FailAsync("form name should not be empty!");

            bool formExists = await repository.Entities
                .AnyAsync(w => w.FormName.Trim().ToLower().Equals(request.FormName.Trim().ToLower()), cancellationToken);

            if (formExists)
                return await Result<Guid>.FailAsync($"{request.FormName} already exists.");

            var entity = _mapper.Map<FormBuilderEntity>(request);
            await repository.InsertAsync(entity);
            await _UoW.Commit(cancellationToken);

            return await Result<Guid>.SuccessAsync(entity.Id);

        }
    }
}
