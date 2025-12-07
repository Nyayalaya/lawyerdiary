using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.Common;
using CourtApp.Application.Interfaces.CacheRepositories.Common;
using CourtApp.Application.Interfaces.Repositories.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Common
{
    public class GetMultiLangDictQuery : IRequest<Result<List<MultiLangDictDto>>>
    {
        public string LangCode { get; set; }
    }
    public class GetMultiLangDictQueryHandler : IRequestHandler<GetMultiLangDictQuery, Result<List<MultiLangDictDto>>>
    {
        private readonly IMultiLangWordCacheRepository _repository;
        private readonly IMapper _mapper;
        public GetMultiLangDictQueryHandler(IMultiLangWordCacheRepository repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;   
        }
        public async Task<Result<List<MultiLangDictDto>>> Handle(GetMultiLangDictQuery request, CancellationToken cancellationToken)
        {
            // Fetch all entities that have translations for the requested language
            var entities = await _repository.GetListByLanCodeAsync(request.LangCode);

            // If no entities found, return empty list
            if (entities == null || !entities.Any())            
                return await Result<List<MultiLangDictDto>>.FailAsync("No data found!");

            var multiLangDict = _mapper.Map<List<MultiLangDictDto>>(entities);
            return Result<List<MultiLangDictDto>>.Success(multiLangDict);
        }
    }   
}
