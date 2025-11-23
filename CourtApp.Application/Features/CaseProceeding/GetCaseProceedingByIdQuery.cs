using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.CaseProceedings;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace CourtApp.Application.Features.CaseProceeding
{
    public class GetCaseProceedingByIdQuery : IRequest<Result<CaseProceedingDto>>
    {
        public Guid CaseId { get; set; }
        public DateTime SelectedDate { get; set; }
    }
    public class GetCaseProceedingByIdQueryHandler : IRequestHandler<GetCaseProceedingByIdQuery, Result<CaseProceedingDto>>
    {
        private readonly ICaseProceedingRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUserCaseRepository userCaseRepository;
        public GetCaseProceedingByIdQueryHandler(ICaseProceedingRepository _repository,
            IMapper _mapper, IUserCaseRepository userCaseRepository)
        {
            this._repository = _repository;
            this._mapper = _mapper;
            this.userCaseRepository = userCaseRepository;
        }

        public async Task<Result<CaseProceedingDto>> Handle(GetCaseProceedingByIdQuery request, CancellationToken cancellationToken)
        {

            var data = await _repository.GetByIdAsync(request.CaseId, request.SelectedDate);
            var cd = await userCaseRepository.GetDetailAsync(request.CaseId);

            if (cd == null)
                return await Result<CaseProceedingDto>.FailAsync("No case info available");

            // Start by mapping if data is available
            CaseProceedingDto md = data != null
                ? _mapper.Map<CaseProceedingDto>(data)
                : new CaseProceedingDto();

            // Assign court-related details from cd (case details)
            md.Court = cd.CourtBench?.CourtBench_En ?? string.Empty;
            md.FirstTitle = cd.FirstTitle ?? string.Empty;
            md.SecondTitle = cd.SecondTitle ?? string.Empty;
            md.Year = cd.CaseYear != 0 ? cd.CaseYear.ToString() : string.Empty;
            md.No = cd.CaseNo?.ToString() ?? string.Empty;
            md.CaseType = cd.CaseType?.Name_En ?? string.Empty;

            // Assign linked case IDs
            md.ParentChildCaseIds = await userCaseRepository
                .Entites
                .Where(w => w.LinkedCaseId == request.CaseId)
                .Select(s => s.Id)
                .ToListAsync();
            md.ParentChildCaseIds.Add(request.CaseId);
            return await Result<CaseProceedingDto>.SuccessAsync(md);



            //var data = await _repository.GetByIdAsync(request.CaseId, request.SelectedDate);
            //var cd = await userCaseRepository.GetDetailAsync(request.CaseId);

            //if (cd == null) return await Result<CaseProceedingDto>.FailAsync("No case info avaiable");

            //CaseProceedingDto md = new CaseProceedingDto();
            //var linkedCases = userCaseRepository
            //    .Entites
            //    .Where(w => w.LinkedCaseId == request.CaseId)
            //    .Select(s => s.Id).ToList();
            //md.LinkedCaseIds = linkedCases;
            //if (data != null) md = _mapper.Map<CaseProceedingDto>(data);
            //md.Court = cd?.CourtBench?.CourtBench_En ?? "";
            //md.FirstTitle = cd?.FirstTitle ?? "";
            //md.SecondTitle = cd?.SecondTitle ?? "";
            //md.Year = cd?.CaseYear != null ? cd.CaseYear.ToString() : "";
            //md.No = cd?.CaseNo?.ToString() ?? "";
            //md.CaseType = cd?.CaseType?.Name_En ?? "";


            //md.Court = cd != null && cd.CourtBench != null ? cd.CourtBench.CourtBench_En : "";
            //md.FirstTitle = cd != null && cd.CourtBench != null ? cd.FirstTitle : "";
            //md.SecondTitle = cd != null && cd.CourtBench != null ? cd.SecondTitle : "";
            //md.Year = cd != null && cd.CourtBench != null ? cd.CaseYear.ToString() : "";
            //md.No = cd != null && cd.CourtBench != null ? cd.CaseNo.ToString() : "";
            //md.CaseType = cd != null && cd.CourtBench != null ? cd.CaseType.Name_En : "";
           // return Result<CaseProceedingDto>.Success(md);
        }
    }

}
