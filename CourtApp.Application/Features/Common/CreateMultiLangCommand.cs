using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.Common;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
using CourtApp.Domain.Entities.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Common
{
    public class CreateMultiLangCommand : IRequest<Result<List<Guid>>>
    {
        public List<MultiLangDictDto> MultiLangDictDtos { get; set; }
    }
    public class CreateMultiLangCommandHandler : IRequestHandler<CreateMultiLangCommand, Result<List<Guid>>>
    {
        private readonly IMultiLangWordRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CreateMultiLangCommandHandler(
            IMultiLangWordRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<List<Guid>>> Handle(CreateMultiLangCommand request, CancellationToken cancellationToken)
        {
            if (request?.MultiLangDictDtos == null || !request.MultiLangDictDtos.Any())
                return Result<List<Guid>>.Fail("No dictionary entries provided.");
           
            var allEntities = _mapper.Map<List<MultiLangDictEntity>>(request.MultiLangDictDtos)
                              ?? new List<MultiLangDictEntity>();
            
            foreach (var entity in allEntities)
            {
                if (entity == null)
                    continue;

                if (entity.Id == Guid.Empty)
                    entity.Id = Guid.NewGuid();
            }
                var insertedIds = await _repository.BulkInsertAsync(allEntities);
                await _unitOfWork.Commit(cancellationToken);
            return await Result<List<Guid>>.SuccessAsync(insertedIds);

        }
    }
}
