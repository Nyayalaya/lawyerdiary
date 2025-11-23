using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.Account;
using CourtApp.Application.Extensions;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.Account;
using KT3Core.Areas.Global.Classes;
using MediatR;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Account
{
    public class CourtFeeStructureGetQuery : IRequest<PaginatedResult<CourtFeeStructureDto>>
    {
        public int StateCode { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetCourtFeeStructAllQueryHandler : IRequestHandler<CourtFeeStructureGetQuery, PaginatedResult<CourtFeeStructureDto>>
    {
        private readonly ICourtFeeStructureRepository _repository;
        private readonly IMapper mapper;
        public GetCourtFeeStructAllQueryHandler(ICourtFeeStructureRepository _repository, IMapper mapper)
        {
            this.mapper = mapper;
            this._repository = _repository;
        }
        public async Task<PaginatedResult<CourtFeeStructureDto>>Handle(CourtFeeStructureGetQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<CourtFeeStructureEntity, CourtFeeStructureDto>> expression = e => new CourtFeeStructureDto
            {
                Id = e.Id,              
                StateName = e.State.Name_En,
                MaxValue=e.MaxValue,
                MinValue=e.MinValue,
                Rate=e.Rate,
                FixAmount=e.FixAmount

            };
            var predicate = PredicateBuilder.True<CourtFeeStructureEntity>();
            if (request.StateCode != 0)
                predicate = predicate.And(b => b.State.Id == request.StateCode);

            var datalist =  _repository.Entites;
            var dtlist =await datalist.Where(predicate)
               .Select(expression)
               .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return dtlist;
        }
    }
}
