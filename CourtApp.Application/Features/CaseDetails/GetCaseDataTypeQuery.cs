using AspNetCoreHero.Results;
using CourtApp.Application.Enums;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseDetails
{
    public record GetCaseDataTypeQuery(Guid caseId, CaseDataType Type) : IRequest<Result<Guid>>;

    public class GetCaseTypeByCaseHandler : IRequestHandler<GetCaseDataTypeQuery, Result<Guid>>
    {
        private readonly IUserCaseRepository _repository;
        public GetCaseTypeByCaseHandler(IUserCaseRepository repository)
        {
            _repository = repository;
        }
        public async Task<Result<Guid>> Handle(GetCaseDataTypeQuery request, CancellationToken cancellationToken)
        {
            var caseInfo = await _repository.GetByIdAsync(request.caseId);
            if (caseInfo is null)
                return await Result<Guid>.FailAsync("Case info not found");

            Guid typeId = request.Type switch
            {
                CaseDataType.CaseType => caseInfo.CaseTypeId,
                CaseDataType.CaseCategory => caseInfo.CaseCategoryId,
                CaseDataType.CaseStage => caseInfo.CaseStageId.Value,
                CaseDataType.Appearance => caseInfo.AppearenceID,

                _ => Guid.Empty
            };

            return typeId == Guid.Empty
                ? await Result<Guid>.FailAsync("Invalid case data type.")
                : await Result<Guid>.SuccessAsync(typeId);

        }
    }
}
