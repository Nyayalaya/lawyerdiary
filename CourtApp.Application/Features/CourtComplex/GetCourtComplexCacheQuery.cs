using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.CourtComplex;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CourtComplex
{
    public class GetCourtComplexCacheQuery : IRequest<Result<List<CourtComplexResponse>>>
    {
        public int DistrictId { get; set; }
        public Guid CourtDistrictId { get; set; }
    }
    public class GetCourtComplexCacheQueryHandler : IRequestHandler<GetCourtComplexCacheQuery, Result<List<CourtComplexResponse>>>
    {
        //private readonly ICourtComplexCacheRepository _cacheRepository;
        private readonly ICourtComplexRepository courComplextRepository;
        private readonly IMapper _mapper;
        public GetCourtComplexCacheQueryHandler(ICourtComplexRepository courComplextRepository, IMapper _mapper)
        {
            this.courComplextRepository = courComplextRepository;
            this._mapper = _mapper;

        }
        public async Task<Result<List<CourtComplexResponse>>> Handle(GetCourtComplexCacheQuery request, CancellationToken cancellationToken)
        {
            var complexes = await courComplextRepository
                .Entities
                .Where(w => w.CourtDistrictId == request.CourtDistrictId)
                .OrderBy(o => o.Name_En)
                .ToListAsync();

            if (!complexes.Any())
                return await Result<List<CourtComplexResponse>>
                    .FailAsync("Complexes is not found for the selected district!");

            var mappedDt = _mapper.Map<List<CourtComplexResponse>>(complexes);
            var mdt = mappedDt.Select(s => new CourtComplexResponse
            {
                Id = s.Id,
                DistrictName = s.DistrictName,
                CDistrictName = s.CDistrictName,
                Name_En = s.Name_En.ToUpper(),
                StateName = s.StateName
            }).OrderBy(o => o.Name_En.ToUpper());
            return Result<List<CourtComplexResponse>>.Success(mdt.ToList());
        }
    }
}
