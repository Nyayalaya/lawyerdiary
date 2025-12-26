using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.FormBuilder;
using CourtApp.Application.Interfaces.Repositories.FormBuilder;
using MediatR;
using System.Linq.Expressions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CourtApp.Application.Extensions;
using CourtApp.Domain.Entities.FormBuilder;

namespace CourtApp.Application.Features.FormBuilder
{
    public class GetCaseDarftingQuery : IRequest<PaginatedResult<CaseDarftingDtoResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetCaseDarftingQueryHanlder : IRequestHandler<GetCaseDarftingQuery,PaginatedResult<CaseDarftingDtoResponse>>
    {
        private readonly IMapper _mapper;
        private readonly ICaseDraftingRepository _repository;
        public GetCaseDarftingQueryHanlder(ICaseDraftingRepository _repository, IMapper _mapper)
        {
            this._repository = _repository;
            this._mapper = _mapper;
        }
        public async Task<PaginatedResult<CaseDarftingDtoResponse>> Handle(GetCaseDarftingQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<DraftingDetailEntity, CaseDarftingDtoResponse>> expression = e => new CaseDarftingDtoResponse
            {
                Id = e.Id,
                CaseTitle=e.Case.FirstTitle+" Vs "+e.Case.SecondTitle,
                TemplateName=e.Template.TemplateName,
                LangCode=e.LangCode
            };            
            try
            {
                var paginatedList = await _repository.Entities
                    .Include(c => c.Case)
                    .Include(c => c.Template)                    
                    .Select(expression)
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                paginatedList.TotalPages = _repository.Entities.Count();
                return paginatedList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }
    }
}
