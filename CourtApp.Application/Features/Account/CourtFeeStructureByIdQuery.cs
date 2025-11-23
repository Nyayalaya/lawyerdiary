using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.Account;
using CourtApp.Application.Interfaces.CacheRepositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Account
{
    public class CourtFeeStructureByIdQuery : IRequest<Result<CourtFeeStructureByIdDto>>
    {
        public Guid Id { get; set; }
    }
    public class GetCourtFeeStructByIdQueryHandler : IRequestHandler<CourtFeeStructureByIdQuery, Result<CourtFeeStructureByIdDto>>
    {
        private readonly ICourtFeeStructureCacheRepository _repository;
        private readonly IMapper mapper;
        public GetCourtFeeStructByIdQueryHandler(ICourtFeeStructureCacheRepository _repository, IMapper mapper)
        {
            this.mapper = mapper;
            this._repository = _repository;
        }
        public async Task<Result<CourtFeeStructureByIdDto>> Handle(CourtFeeStructureByIdQuery request, CancellationToken cancellationToken)
        {
            var data = await _repository.GetCacheDataByIdAsync(request.Id);
            var mappeddata = mapper.Map<CourtFeeStructureByIdDto>(data);
            return Result<CourtFeeStructureByIdDto>.Success(mappeddata);
        }
        
    }
}
