using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.FormBuilder;
using CourtApp.Application.Extensions;
using CourtApp.Application.Interfaces.Repositories.FormBuilder;
using CourtApp.Domain.Entities.FormBuilder;
using MediatR;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.FormBuilder
{
    public class GetTemplateInfoQuery : IRequest<PaginatedResult<GetTemplateInfoDtoResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public class GetTemplateCachedQueryHandler : IRequestHandler<GetTemplateInfoQuery, PaginatedResult<GetTemplateInfoDtoResponse>>
        {
            private readonly ITemplateInfoRepository _Repository;
            public GetTemplateCachedQueryHandler(ITemplateInfoRepository _Repository)
            {
                this._Repository = _Repository;

            }
            public async Task<PaginatedResult<GetTemplateInfoDtoResponse>> Handle(GetTemplateInfoQuery request, CancellationToken cancellationToken)
            {
                Expression<Func<TemplateInfoEntity, GetTemplateInfoDtoResponse>> expression = e => new GetTemplateInfoDtoResponse
                {
                    Id = e.Id,
                    TemplatePath = e.TemplatePath,
                    TemplateName = e.TemplateName,
                    Tags = e.Tags.Select(s => new TemplateTagInfo() { Tag = s.Tag }).ToList()
                };
                var dt =  _Repository.Entities.ToList();
                var paginatedList = await _Repository.Entities
                    .Select(expression)
                    .ToPaginatedListAsync(1, 10000);
                paginatedList.TotalPages = _Repository.Entities.Count();
                return paginatedList;
            }
        }
    }
}
