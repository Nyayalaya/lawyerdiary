using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.CaseTitle;
using CourtApp.Application.Extensions;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using KT3Core.Areas.Global.Classes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseTitle
{
    public class GetCaseTitleQuery : IRequest<PaginatedResult<CaseTitleResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TypeId { get; set; }
        public List<Guid> CaseIds { get; set; }
        public List<string> LinkedIds { get; set; }
    }

    public class GetCaseTitleQueryHandler : IRequestHandler<GetCaseTitleQuery, PaginatedResult<CaseTitleResponse>>
    {
        private readonly ICaseTitleRepository repository;
        private readonly IMapper _mapper;
        public GetCaseTitleQueryHandler(ICaseTitleRepository repository, IMapper _mapper)
        {
            this.repository = repository;
            this._mapper = _mapper;
        }
        public async Task<PaginatedResult<CaseTitleResponse>> Handle(GetCaseTitleQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<CaseTitleEntity, CaseTitleResponse>> expression = e => new CaseTitleResponse
            {
                Id = e.Id,
                No=e.Case.CaseNo,
                Year=e.Case.CaseYear.ToString(),
                CaseType=e.Case.CaseType.Name_En,
                Court=e.Case.CourtBench.CourtBench_En,
                CaseDetail = e.Case.FirstTitle + " V/S " + e.Case.SecondTitle ,
                Type = e.TypeId == 1 ? "First Title" : "Second Title",
                CaseApplicantDetails = e.CaseApplicants
                .Select(s => new ApplicantDetailDto
                {
                    ApplicantNo = s.ApplicantNo,
                    ApplicantDetail = s.ApplicantDetail,
                }).ToList()
            };
            var predicate = PredicateBuilder.True<CaseTitleEntity>();
            if (predicate != null)
            {
                if (request.LinkedIds!=null && request.LinkedIds.Any())
                    predicate = predicate.And(c => request.LinkedIds.Contains(c.CreatedBy));
                if (request.TypeId != 0)
                    predicate = predicate.And(y => y.TypeId == request.TypeId);
                if (request.CaseIds != null && request.CaseIds.Count() > 0)
                    predicate = predicate.And(y => request.CaseIds.Contains(y.CaseId));

            }
            var paginatedList = await repository
                .Titles                
                .Where(predicate)
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;

        }
    }
}
