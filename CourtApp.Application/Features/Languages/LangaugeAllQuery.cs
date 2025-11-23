using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.Common;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CourtApp.Application.Features.Languages
{
    public class LangaugeAllQuery : IRequest<Result<List<LangDto>>>
    {
        public int StateId { get; set; }
    }

    public class LangaugeAllQueryHandler : IRequestHandler<LangaugeAllQuery, Result<List<LangDto>>>
    {
        private readonly ILanguageRepository repository;
        private readonly IMapper mapper;
        public LangaugeAllQueryHandler(ILanguageRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<List<LangDto>>> Handle(LangaugeAllQuery request, CancellationToken cancellationToken)
        {
            var languages =await repository.Entities.AsNoTracking()
                .Where(e => e.StateId == request.StateId)
                .SelectMany(s => s.Languages).ToListAsync();

            if (!languages.Any())
                return await Result<List<LangDto>>.FailAsync("There is no lanague assigned for the district!");                           

            var result = mapper.Map<List<LangDto>>(languages.OrderBy(o=>o.Name).ToList());
            return Result<List<LangDto>>.Success(result);
        }

    }
}
