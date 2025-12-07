using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.Common;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
using CourtApp.Domain.Entities.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Common
{
    public class UpdateMultilangCommand : IRequest<Result<List<Guid>>>
    {
        public List<MultiLangDictDto> MultiLangDictDtos { get; set; }
    }
    public class UpdateMultilangCommandHandler : IRequestHandler<UpdateMultilangCommand, Result<List<Guid>>>
    {
        private readonly IMultiLangWordRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMultilangCommandHandler(
            IMultiLangWordRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<Result<List<Guid>>> Handle(UpdateMultilangCommand request, CancellationToken cancellationToken)
        {
            if (request?.MultiLangDictDtos == null || !request.MultiLangDictDtos.Any())
                return await Result<List<Guid>>.FailAsync("No dictionary entries provided for update.");
            var entitiesToUpdate = _mapper.Map<List<MultiLangDictEntity>>(request.MultiLangDictDtos);
            var ids = entitiesToUpdate.Select(x => x.Id).ToList();
            var existingEntities = await _repository.Entities
                .Where(e => ids.Contains(e.Id))
                .ToListAsync(cancellationToken);
            foreach (var existing in existingEntities)
            {
                var updated = entitiesToUpdate.First(x => x.Id == existing.Id);
                _mapper.Map(updated, existing);
            }
            await _repository.UpdateRangeAsync(existingEntities);
            await _unitOfWork.Commit(cancellationToken);
            return Result<List<Guid>>.Success(existingEntities.Select(x => x.Id).ToList());
        }
    }
}
