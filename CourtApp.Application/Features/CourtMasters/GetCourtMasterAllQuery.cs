using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.CourtMaster;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.LawyerDiary;
using KT3Core.Areas.Global.Classes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CourtMasters
{
    public class GetCourtMasterAllQuery : IRequest<Result<List<GetCourtMasterDataAllResponse>>>
    {
        public Guid CourtTypeId { get; set; }
        public GetCourtMasterAllQuery()
        {

        }
    }

    internal class GetCourtMasterAllQueryHandler : IRequestHandler<GetCourtMasterAllQuery, Result<List<GetCourtMasterDataAllResponse>>>
    {
        private readonly ICourtMasterRepository _repository;
        private readonly ICourtBenchRepository _repoBench;
        public GetCourtMasterAllQueryHandler(ICourtMasterRepository _repository,
            ICourtBenchRepository _repoBench)
        {
            this._repository = _repository;
            this._repoBench = _repoBench;
        }
        public async Task<Result<List<GetCourtMasterDataAllResponse>>> Handle(GetCourtMasterAllQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.Entities
                        .Include(e => e.CourtType)
                        .Include(e => e.State)
                        .Include(e => e.CourtComplex)
                        .Include(e => e.CourtDistrict)
                        .Include(e => e.CourtBenches) // To avoid N+1 issue when using .Count()
                        .AsQueryable();

            if (request.CourtTypeId != Guid.Empty)
            {
                query = query.Where(e => e.CourtType.Id == request.CourtTypeId);
            }

            var dtlist = await query.Select(e => new GetCourtMasterDataAllResponse
            {
                Id = e.Id,
                CourtType = e.CourtType.CourtType,
                CourtName = e.Name_En,
                CourtFullName = e.Name_En,
                State = e.State.Name_En,
                District = e.CourtDistrict != null ? e.CourtDistrict.Name_En : null,
                Complex = e.CourtComplex != null ? e.CourtComplex.Name_En : null,
                Total = e.CourtBenches.Count
            }).ToListAsync();

            return Result<List<GetCourtMasterDataAllResponse>>.Success(dtlist);


            //var predicate = PredicateBuilder.True<CourtMasterEntity>();
            //if (request.CourtTypeId != Guid.Empty)
            //    predicate = predicate.And(b => b.CourtType.Id == request.CourtTypeId);


            //var dtlist = _repository.Entities
            //    .Include(ct => ct.CourtType)
            //    .Include(st => st.State)
            //    .Include(d => d.CourtComplex)
            //    .Include(d => d.CourtDistrict)
            //   .Where(predicate)
            //   .Select(e => new GetCourtMasterDataAllResponse
            //   {
            //       Id = e.Id,
            //       CourtType = e.CourtType.CourtType,
            //       CourtName = e.Name_En,
            //       CourtFullName = e.Name_En,
            //       State = e.State.Name_En,
            //       District = e.CourtDistrict != null ? e.CourtDistrict.Name_En : null,
            //       Complex = e.CourtComplex != null ? e.CourtComplex.Name_En : null,
            //       Total = e.CourtBenches.Count()
            //   })
            //   .ToList();

            //return Task.FromResult(Result<List<GetCourtMasterDataAllResponse>>
            //    .Success(dtlist));
        }
    }
}
