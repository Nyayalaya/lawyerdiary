using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.FormBuilder;
using CourtApp.Domain.Entities.LawyerDiary;
using KT3Core.Areas.Global.Classes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CourtApp.Application.Features.CourtForm
{
    public class CreateCourtFormCommand : IRequest<Result<Guid>>
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

    public class CreateCourtFormCommandHandler : IRequestHandler<CreateCourtFormCommand, Result<Guid>>
    {
        private readonly ICourtFormTypeRepository repository;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        public CreateCourtFormCommandHandler(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICourtFormTypeRepository repository)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.repository = repository;
        }
        public async Task<Result<Guid>> Handle(CreateCourtFormCommand request, CancellationToken cancellationToken)
        {
            var predicate = PredicateBuilder.True<CourtFormTypeEntity>();

            if (request.CourtTypeId != Guid.Empty)
                predicate = predicate.And(crt => crt.CourtTypeId == request.CourtTypeId);

            if (!string.IsNullOrWhiteSpace(request.LanguageCode))
                predicate = predicate.And(crt => crt.LanguageCode.Equals(request.LanguageCode));

            if (request.CaseCategoryId != Guid.Empty)
                predicate = predicate.And(crt => crt.CaseCategoryId == request.CaseCategoryId);

            if (!string.IsNullOrWhiteSpace(request.FormName))
                predicate = predicate.And(crt => crt.FormName == request.FormName);

            if (request.CaseTypeId!=Guid.Empty)
                predicate = predicate.And(crt => crt.CaseTypeId == request.CaseTypeId);

            // Check for existence before inserting
            bool formExists = await repository.Entities.AnyAsync(predicate, cancellationToken);
            if (formExists)
                return await Result<Guid>.FailAsync("The provided details already exist");

            var formEntity = mapper.Map<CourtFormTypeEntity>(request);
            await repository.InsertAsync(formEntity);
            await unitOfWork.Commit(cancellationToken);

            return await Result<Guid>.SuccessAsync("Record created successfull!");

        }
    }
}
