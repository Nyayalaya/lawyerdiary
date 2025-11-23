using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.Common;
using CourtApp.Application.Interfaces.CacheRepositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CourtType.Query
{
    public class GetCourtTypeByIdQuery : IRequest<Result<GetCourtTypeResponse>>
    {
        public Guid Id { get; set; }
    }
    public class GetCourtTypeByIdQueryHandler : IRequestHandler<GetCourtTypeByIdQuery, Result<GetCourtTypeResponse>>
    {
        private readonly ICourtTypeCacheRepository _CourtType;
        private readonly IMapper _mapper;

        public GetCourtTypeByIdQueryHandler(ICourtTypeCacheRepository _CourtType, IMapper mapper)
        {
            this._CourtType = _CourtType;
            _mapper = mapper;
        }

        public async Task<Result<GetCourtTypeResponse>> Handle(GetCourtTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var courtTypeList = await _CourtType.GetByIdAsync(request.Id);
            var mappedCourtTpe = _mapper.Map<GetCourtTypeResponse>(courtTypeList);
            mappedCourtTpe.Language = _mapper.Map<List<LangDto>>(courtTypeList.Languages);
            return Result<GetCourtTypeResponse>.Success(mappedCourtTpe);
        }
    }
}
