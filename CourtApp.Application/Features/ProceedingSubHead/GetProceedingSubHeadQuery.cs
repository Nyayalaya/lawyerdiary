using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.ProcSubHead;
using CourtApp.Application.Extensions;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.LawyerDiary;
using KT3Core.Areas.Global.Classes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.ProceedingSubHead
{
    public class GetProceedingSubHeadQuery : IRequest<PaginatedResult<GetProcSubHeadResponse>>
    {
        public Guid HeadId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
    }
    public class GetProceedingSubHeadQueryHandler : IRequestHandler<GetProceedingSubHeadQuery, PaginatedResult<GetProcSubHeadResponse>>
    {
        private readonly IProceedingSubHeadRepository repository;
        private readonly IMapper mapper;
        public GetProceedingSubHeadQueryHandler(IProceedingSubHeadRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<PaginatedResult<GetProcSubHeadResponse>> Handle(GetProceedingSubHeadQuery request, CancellationToken cancellationToken)
        {
            // Projection: maps directly to response DTO
            Expression<Func<ProceedingSubHeadEntity, GetProcSubHeadResponse>> expression = e => new GetProcSubHeadResponse
            {
                Id = e.Id,
                Name_En = e.Name_En,
                Name_Hn = e.Name_Hn,
                Head = e.Head.Name_En
            };

            // Build predicate for filtering
            var predicate = PredicateBuilder.True<ProceedingSubHeadEntity>();

            if (request.HeadId != Guid.Empty)
                predicate = predicate.And(e => e.HeadId == request.HeadId);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                predicate = predicate.And(e =>
                    e.Name_En.ToLower().Contains(request.Search.ToLower()) ||
                    e.Name_Hn.Contains(request.Search) ||
                    e.Head.Name_En.ToLower().Contains(request.Search.ToLower()));
            }

            try
            {
                var query = repository.Entities
                    .Include(e => e.Head)
                    .Where(predicate);

                // Apply sorting
                if (!string.IsNullOrEmpty(request.SortColumn))
                {
                    query = request.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescendingDynamic(request.SortColumn)
                        : query.OrderByDynamic(request.SortColumn);
                }
                else
                {
                    // Default sorting (optional)
                    query = query.OrderBy(e => e.Name_En);
                }

                // Project and paginate
                var paginatedList = await query
                    .Select(expression)
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);

                return paginatedList;
            }
            catch (Exception ex)
            {
                // Log this in production system
                Console.WriteLine($"[ProceedingSubHeadQueryHandler] Error: {ex.Message}");
                return PaginatedResult<GetProcSubHeadResponse>.Failure(new List<string>());
            }
        }

    }
}
