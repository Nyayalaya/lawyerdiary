using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.CaseDetails;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using KT3Core.Areas.Global.Classes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseDetails
{
    public class GetCasesByUserQuery : IRequest<Result<List<GetCaseInfoDto>>>
    {
        public string CallingFrom { get; set; }
        public List<string> LinkIds { get; set; }
    }
    public class GetCasesByUserQueryHandler : IRequestHandler<GetCasesByUserQuery, Result<List<GetCaseInfoDto>>>
    {

        private readonly IUserCaseRepository _repository;
        public GetCasesByUserQueryHandler(IUserCaseRepository _repository)
        {
            this._repository = _repository;
        }
        public async Task<Result<List<GetCaseInfoDto>>> Handle(GetCasesByUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var predicate = PredicateBuilder.True<CaseDetailEntity>();
                if (predicate != null)
                {
                    if (request.CallingFrom == "Bring")
                        predicate = predicate.And(c => request.LinkIds.Contains(c.CreatedBy) && c.DisposalDate != null);
                    else
                        predicate = predicate.And(c => request.LinkIds.Contains(c.CreatedBy));
                }
                var userCases = await _repository.Entites
                    .Where(predicate)
                    .Select(c => new GetCaseInfoDto
                    {
                        Id = c.Id,
                        No = c.CaseNo,
                        Year = c.CaseYear.ToString(),
                        CourtType = c.CourtType.CourtType.ToString(),
                        CaseType = c.CaseType.Name_En,
                        Court = c.CourtBench.CourtBench_En,
                        CaseStage = c.CaseStage.CaseStage,
                        DisposalDate = c.DisposalDate,
                        CaseDetail = (c.FirstTitle + " V/S " + c.SecondTitle + " [" +
                                            (string.IsNullOrEmpty(c.CaseNo) ? c.CaseYear.ToString() : c.CaseNo + "/" + c.CaseYear.ToString()) +
                                            "]").ToUpperInvariant(),
                        NextDate = c.CaseProcEntities
                                              .OrderByDescending(o => o.NextDate.Value) // Order by latest date
                                              .Select(s => s.NextDate.Value.ToString("dd-MM-yyyy"))
                                              .FirstOrDefault() ?? (c.NextDate.HasValue ? c.NextDate.Value.ToString("dd-MM-yyyy") : "")
                    }).ToListAsync();

                if (userCases.Any())
                    return await Result<List<GetCaseInfoDto>>.SuccessAsync(userCases);
                else
                    return await Result<List<GetCaseInfoDto>>.FailAsync("There is some issue on fetching data");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return await Result<List<GetCaseInfoDto>>.FailAsync("SOme exception raised");
            }

        }
    }
}
