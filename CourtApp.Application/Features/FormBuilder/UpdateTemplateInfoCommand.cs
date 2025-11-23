using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.FormBuilder;
using CourtApp.Domain.Entities.FormBuilder;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.FormBuilder
{
    public class UpdateTemplateInfoCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }
        public string TemplateName { get; set; }
        public string TemplatePath { get; set; }
        public string TemplateBody { get; set; }
        public List<TemplateTags> Tags { get; set; }
    }
    public class UpdateTemplateInfoCommandHandler : IRequestHandler<UpdateTemplateInfoCommand, Result<Guid>>
    {
        private readonly ITemplateInfoRepository _repository;
        private readonly IMapper _mapper;
        private IUnitOfWork _UoW { get; set; }
        public UpdateTemplateInfoCommandHandler(ITemplateInfoRepository _repository,
            IMapper _mapper, IUnitOfWork _UoW)
        {
            this._repository = _repository;
            this._mapper = _mapper;
            this._UoW = _UoW;
        }
        public async Task<Result<Guid>> Handle(UpdateTemplateInfoCommand request, CancellationToken cancellationToken)
        {
            var TempDetail = await _repository.GetByIdAsync(request.Id);
            if (TempDetail == null) Result<Guid>.Fail("Template is not found!");
            TempDetail.TemplateName = request.TemplateName;
            TempDetail.TemplatePath = request.TemplatePath;
            TempDetail.TemplateBody = request.TemplateBody;
            TempDetail.FormId = request.FormId;
            TempDetail.Tags=_mapper.Map<List<TemplateTagsEntity>>(request.Tags);            
            await _repository.UpdateAsync(TempDetail);
            await _UoW.Commit(cancellationToken);
            return Result<Guid>.Success(TempDetail.Id);
        }
    }
}
