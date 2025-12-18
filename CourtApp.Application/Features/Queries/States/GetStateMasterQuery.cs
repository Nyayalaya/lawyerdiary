using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CourtApp.Application.Interfaces.CacheRepositories.Common;

namespace CourtApp.Application.Features.Queries.States
{
    public class GetStateMasterQuery : IRequest<Result<List<GetStateMasterResponse>>>
    {
        public GetStateMasterQuery()
        {

        }
    }
    public class GetStateMasterQueryHandler : IRequestHandler<GetStateMasterQuery, Result<List<GetStateMasterResponse>>>
    {
        private readonly IStateMasterCacheRepository _repositoryCache;
        private readonly IMapper _mapper;
        public GetStateMasterQueryHandler(IStateMasterCacheRepository _repositoryCache, IMapper _mapper)
        {
            this._repositoryCache = _repositoryCache;
            this._mapper = _mapper;
        }
        public async Task<Result<List<GetStateMasterResponse>>> Handle(GetStateMasterQuery request, CancellationToken cancellationToken)
        {
            var list = await _repositoryCache.GetStateListAsync();
            var mappeddata = _mapper.Map<List<GetStateMasterResponse>>(list.OrderBy(s => s.Name_En));
            var OrderDt = mappeddata.Select(s => new GetStateMasterResponse
            { Id = s.Id, name_en = s.name_en.ToUpper(), name_hn = s.name_hn })
                .OrderBy(o => o.name_en.ToUpper()).ToList();
            return Result<List<GetStateMasterResponse>>.Success(OrderDt);
        }
    }

}