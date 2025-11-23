using AspNetCoreHero.Results;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.Account;
using KT3Core.Areas.Global.Classes;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Queries.Tools
{
    public class AdvalremCourtFeeCalQuery : IRequest<Result<string>>
    {
        public int StateCode { get; set; }
        public string FeeKind { get; set; }
        public double CalculativeAmount { get; set; }
    }
    public class AdvalremCourtFeeCalQueryHandler : IRequestHandler<AdvalremCourtFeeCalQuery, Result<string>>
    {
        private readonly ICourtFeeStructureRepository _repository;
        public AdvalremCourtFeeCalQueryHandler(ICourtFeeStructureRepository _repository)
        {
            this._repository = _repository;
        }
        public async Task<Result<string>> Handle(AdvalremCourtFeeCalQuery request, CancellationToken cancellationToken)
        {
            var predicate = PredicateBuilder.True<CourtFeeStructureEntity>();
            double Rate = 0, FixedAmount = 0, CalculativeAmount = 0, MinAmt = 0, MaxAmt = 0;
            CalculativeAmount = request.CalculativeAmount;
            if (request.StateCode != 0)
                predicate = predicate.And(b => b.State.Id == request.StateCode);

            if (request.CalculativeAmount != 0)
                predicate = predicate.And(a => CalculativeAmount >= a.MinValue && CalculativeAmount <= a.MaxValue);

            var datalist = _repository.Entites;
            string result = string.Empty;
            var dt = datalist.Where(predicate).Select(s => new { s.Rate, FixedAmount = s.FixAmount, MinAmt = s.MinValue, MaxAmt = s.MaxValue }).FirstOrDefault();
            if (dt != null)
            {
                Rate = dt.Rate;
                FixedAmount = dt.FixedAmount;
                MinAmt = dt.MinAmt;
                MaxAmt = dt.MaxAmt;
                if (Rate == 0 && FixedAmount == 0)
                {
                    datalist.Where(a => a.MinValue > request.CalculativeAmount && a.MaxValue == 0)
                        .Select(s => new { s.Rate, FixedAmount = s.FixAmount, MinAmt = s.MinValue }).FirstOrDefault(); ;
                }
                CalculativeAmount -= MinAmt;
                result = "Rs. " + Convert.ToString(request.FeeKind == "FULL" ? CalculativeAmount * Rate + FixedAmount : CalculativeAmount * Rate + FixedAmount / 2);
            }
            else
                result = "Given amount is not exist the court fee structure!";
            return await Result<string>.SuccessAsync(result);
        }
    }

}
