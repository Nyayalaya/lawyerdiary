using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.FormBuilder;
using CourtApp.Application.Interfaces.Repositories.FormBuilder;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.FormBuilder
{
    public class GetTemplateInfoByIdQuery : IRequest<Result<GetTemplateInfoByIdDto>>
    {
        public Guid Id { get; set; }
    }
    public class GetTemplateInfoByIdQueryHandler : IRequestHandler<GetTemplateInfoByIdQuery, Result<GetTemplateInfoByIdDto>>
    {
        private readonly ITemplateInfoRepository _Repository;
        private readonly IMapper _Mapper;
        public GetTemplateInfoByIdQueryHandler(ITemplateInfoRepository _Repository, IMapper _Mapper)
        {
            this._Repository = _Repository;
            this._Mapper = _Mapper;
        }
        public async Task<Result<GetTemplateInfoByIdDto>> Handle(GetTemplateInfoByIdQuery request, CancellationToken cancellationToken)
        {
            var Details =await _Repository.GetByIdAsync(request.Id);
            if (Details != null)
            {
                var dt = new GetTemplateInfoByIdDto();
                dt.FormId = Details.FormId;
                dt.TemplatePath=Details.TemplatePath;
                dt.TemplateName=Details.TemplateName;
                dt.TemplateBody = Details.TemplateBody;
                return Result<GetTemplateInfoByIdDto>.Success(dt);
            }
            return null;
        }
    }
}
