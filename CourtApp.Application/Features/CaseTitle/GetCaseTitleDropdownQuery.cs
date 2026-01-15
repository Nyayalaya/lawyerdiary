using AutoMapper;
using CourtApp.Application.DTOs.CaseTitle;
using CourtApp.Application.DTOs.DropDowns;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using KT3Core.Areas.Global.Classes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseTitle
{
    public record GetCaseTitleDropdownQuery(List<Guid> CaseIds,List<string>? LinkedIds) : IRequest<CaseTitleDropdownResultDto>;

    public class GetCaseTitleDropdownQueryHandler : IRequestHandler<GetCaseTitleDropdownQuery, CaseTitleDropdownResultDto>
    {
        private readonly ICaseTitleRepository repository;

        public GetCaseTitleDropdownQueryHandler(ICaseTitleRepository repository)
        {
            this.repository = repository;
        }
        public async Task<CaseTitleDropdownResultDto> Handle(GetCaseTitleDropdownQuery request, CancellationToken cancellationToken)
        {
            var predicate = PredicateBuilder.True<CaseTitleEntity>();

            if (request.LinkedIds != null && request.LinkedIds.Any())
                predicate = predicate.And(x => request.LinkedIds.Contains(x.CreatedBy));

            if (request.CaseIds != null && request.CaseIds.Any())
                predicate = predicate.And(x => request.CaseIds.Contains(x.CaseId));

            var data = await repository.Titles
                .AsNoTracking()
                .Where(predicate)
                .Select(e => new
                {
                    e.TypeId,
                    Applicants = e.CaseApplicants
                        .Select(a => new
                        {
                            a.ApplicantNo,
                            a.ApplicantDetail
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            var result = new CaseTitleDropdownResultDto
            {
                FirstTitles = data
                            .Where(x => x.TypeId == 1)
                            .SelectMany(x => x.Applicants)
                            .Where(a => !string.IsNullOrWhiteSpace(a.ApplicantDetail))
                            .Select(a => new DdlStringStringDto
                            {
                                Id = a.ApplicantNo,
                                Name = a.ApplicantDetail
                            })
                            .DistinctBy(x => x.Id)
                            .OrderBy(x => x.Name)
                            .ToList(),

                SecondTitles = data
                            .Where(x => x.TypeId == 2)
                            .SelectMany(x => x.Applicants)
                            .Where(a => !string.IsNullOrWhiteSpace(a.ApplicantDetail))
                            .Select(a => new DdlStringStringDto
                            {
                                Id = a.ApplicantNo,
                                Name = a.ApplicantDetail
                            })
                            .DistinctBy(x => x.Id)
                            .OrderBy(x => x.Name)
                            .ToList()
                                };

            return result;


        }
    }
}
