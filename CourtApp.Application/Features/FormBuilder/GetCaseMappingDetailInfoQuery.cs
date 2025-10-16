using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.FormBuilder;
using CourtApp.Application.Interfaces.CacheRepositories;
using CourtApp.Application.Interfaces.CacheRepositories.FormBuilder;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.FormBuilder;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.FormBuilder
{
    public class GetCaseMappingDetailInfoQuery : IRequest<Result<CaseMappingDetailInfoDto>>
    {
        public Guid Id { get; set; }
    }
    public class GetCaseMappingDetailInfoQueryHandler : IRequestHandler<GetCaseMappingDetailInfoQuery, Result<CaseMappingDetailInfoDto>>
    {
        private readonly ICaseNatureCacheRepository _RepoNature;
        private readonly IUserCaseRepository _RepoCase;
        private readonly ICaseDraftingRepository _RepoDrafting;
        private readonly ITemplateInfoCacheRepository _RepoTemplate;
        private readonly IFormTempMappingRepository _RepoMapping;
        public GetCaseMappingDetailInfoQueryHandler(IUserCaseRepository _RepoCase,
            ICaseNatureCacheRepository _RepoNature,
            ICaseStageCacheRepository _RepoStage,
            ICaseDraftingRepository _RepoDrafting,
            ITemplateInfoCacheRepository _RepoTemplate,
            IFormTempMappingRepository _RepoMapping
            )
        {
            this._RepoCase = _RepoCase;
            this._RepoNature = _RepoNature;
            this._RepoNature = _RepoNature;
            this._RepoTemplate = _RepoTemplate;
            this._RepoDrafting = _RepoDrafting;
            this._RepoTemplate = _RepoTemplate;
            this._RepoMapping = _RepoMapping;
        }
        public async Task<Result<CaseMappingDetailInfoDto>> Handle(GetCaseMappingDetailInfoQuery request, CancellationToken cancellationToken)
        {
            var draftingDetail = await _RepoDrafting.GetByIdAsync(request.Id);
            if (draftingDetail == null) return await Result<CaseMappingDetailInfoDto>.FailAsync("No drafting found!");

            var templateDetail = await _RepoTemplate.GetByIdAsync(draftingDetail.TemplateId);
            if (templateDetail == null) return await Result<CaseMappingDetailInfoDto>.FailAsync("No template found!");

            CaseMappingDetailInfoDto infoDto = new CaseMappingDetailInfoDto();
            infoDto.TemplateBody = templateDetail.TemplateBody;
            infoDto.CaseId = draftingDetail.CaseId;
            var tagsData = draftingDetail.FieldDetails.Select(s => new MappingDetails { Tag = s.Tag, Value = s.Value }).ToList();
            infoDto.TagValues = tagsData;
            return await Result<CaseMappingDetailInfoDto>.SuccessAsync(infoDto);
        }
    }
}
