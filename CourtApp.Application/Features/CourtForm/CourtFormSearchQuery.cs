using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.CourtForm;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.FormBuilder;
using KT3Core.Areas.Global.Classes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CourtApp.Application.Features.CourtForm
{
    public class CourtFormSearchQuery : IRequest<Result<List<CourtFormDto>>>
    {
        public Guid Id { get; set; }
        public int StateId { get; set; }
        public Guid CourtTypeId { get; set; }
        public Guid CaseCategoryId { get; set; }
        public Guid CaseTypeId { get; set; }
    }

    public class CourtFormSearchQueryHandler : IRequestHandler<CourtFormSearchQuery, Result<List<CourtFormDto>>>
    {
        private readonly ICourtFormTypeRepository repository;
        public CourtFormSearchQueryHandler(ICourtFormTypeRepository repository)
        {
            this.repository = repository;
        }
        public async Task<Result<List<CourtFormDto>>> Handle(CourtFormSearchQuery request, CancellationToken cancellationToken)
        {
            var predicate = PredicateBuilder.True<CourtFormTypeEntity>();
            if (request.Id != Guid.Empty)
                predicate = predicate.And(s => s.Id == request.Id);

            if (request.StateId != 0)
                predicate = predicate.And(s => s.StateId == request.StateId);

            if (request.CourtTypeId != Guid.Empty)
                predicate = predicate.And(s => s.CourtTypeId == request.CourtTypeId);

            if (request.CaseCategoryId != Guid.Empty)
            {
                predicate = predicate.And(s =>
                    s.CaseCategoryId == request.CaseCategoryId ||
                    s.CaseCategoryId== null);  // include "All" always
            }

            if (request.CaseTypeId != Guid.Empty)
                predicate = predicate.And(s => s.CaseTypeId == request.CaseTypeId);

            var forms = await repository
                .Entities
                .Where(predicate).Select(s => new CourtFormDto
                {
                    Id = s.Id,
                    CaseCategory = s.CaseCategory.Name_En,
                    FormName = s.FormName,
                    StateName = s.State.Name_En,
                    FormTemplate = s.FormTemplate,
                }).OrderBy(o=>o.FormName).ToListAsync(cancellationToken);

            if (!forms.Any())
                return await Result<List<CourtFormDto>>.FailAsync("Form is not avaiable for the selected criteria");

            return await Result<List<CourtFormDto>>.SuccessAsync(forms);

        }
    }
}
