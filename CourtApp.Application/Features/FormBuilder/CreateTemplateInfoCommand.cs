using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.FormBuilder;
using CourtApp.Domain.Entities.FormBuilder;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.FormBuilder
{
    public class CreateTemplateInfoCommand : IRequest<Result<Guid>>
    {
        public string TemplateName { get; set; }
        public string TemplatePath { get; set; }
        public string TemplateBody { get; set; }
        public List<TemplateTags> Tags { get; set; }
    }
    public class TemplateTags
    {
        public string Tag { get; set; }
    }
    public class CreateTemplateInfoCommandHandler : IRequestHandler<CreateTemplateInfoCommand, Result<Guid>>
    {
        private readonly ITemplateInfoRepository _repository;
        private readonly IMapper _mapper;
        private IUnitOfWork _UoW { get; set; }
        public CreateTemplateInfoCommandHandler(ITemplateInfoRepository _repository,
            IMapper _mapper, IUnitOfWork _UoW)
        {
            this._repository = _repository;
            this._mapper = _mapper;
            this._UoW = _UoW;
        }
        public async Task<Result<Guid>> Handle(CreateTemplateInfoCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.TemplateName))
                return await Result<Guid>.FailAsync("Template Name is not given");

            bool templateExists = await _repository.Entities
                .AnyAsync(e => e.TemplateName == request.TemplateName);

            if (templateExists)
                return await Result<Guid>.FailAsync("The given template info already exists.");

            // Map request to entity
            var entity = new TemplateInfoEntity
            {
                TemplateName = request.TemplateName,
                TemplatePath = request.TemplatePath,
                TemplateBody = request.TemplateBody,
                Tags = request.Tags?.Select(tag => new TemplateTagsEntity
                {
                    Tag = tag.Tag
                }).ToList() ?? new List<TemplateTagsEntity>()
            };

            await _repository.InsertAsync(entity);
            await _UoW.Commit(cancellationToken);

            return await Result<Guid>.SuccessAsync(entity.Id);

        }
    }
}
