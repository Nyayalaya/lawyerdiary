using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.FormBuilder;
using CourtApp.Application.Interfaces.CacheRepositories.FormBuilder;
using CourtApp.Domain.Entities.FormBuilder;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace CourtApp.Application.Features.FormBuilder
{
    public class GetFormBuilderCachedByIdQuery : IRequest<Result<FormBuilderResponseByIdDto>>
    {
        public Guid Id { get; set; }
        public string AccessFrom { get; set; }
    }
    public class GetFormBuilderCachedByIdQueryHanlder : IRequestHandler<GetFormBuilderCachedByIdQuery, Result<FormBuilderResponseByIdDto>>
    {
        private readonly IMapper _mapper;
        private readonly IFormBuilderCacheRepository _repository;
        private readonly ITemplateInfoCacheRepository _templateInfoCacheRepository;
        public GetFormBuilderCachedByIdQueryHanlder(IFormBuilderCacheRepository _repository, IMapper _mapper, ITemplateInfoCacheRepository templateInfoCacheRepository)
        {
            this._repository = _repository;
            this._mapper = _mapper;
            _templateInfoCacheRepository = templateInfoCacheRepository;
        }
        public async Task<Result<FormBuilderResponseByIdDto>> Handle(GetFormBuilderCachedByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                FormBuilderEntity entity = null;
                if (request.AccessFrom == "MST") entity = await _repository.GetByIdAsync(request.Id);
                else
                {
                    var templatInfo = await _templateInfoCacheRepository.GetByIdAsync(request.Id);
                    entity = await _repository.GetByIdAsync(templatInfo.FormId);
                }
                var result = _mapper.Map<FormBuilderResponseByIdDto>(entity);
                if (entity != null && entity.FieldsDetails != null)
                {
                    var fields = entity.FieldsDetails.Fields;
                    var mappedDt = _mapper.Map<List<FieldDetailsDto>>(fields);
                    result.FieldDetails = mappedDt;
                    return Result<FormBuilderResponseByIdDto>.Success(result);
                }
                return Result<FormBuilderResponseByIdDto>.Fail("No Record found");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
