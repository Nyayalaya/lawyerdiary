using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.CacheKeys;
using CourtApp.Application.DTOs.Common;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
using CourtApp.Domain.Entities.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
            IUnitOfWork unitOfWork
           )
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            
        }
        public async Task<Result<List<Guid>>> Handle(UpdateMultilangCommand request, CancellationToken cancellationToken)
        {
            if (request?.MultiLangDictDtos == null || !request.MultiLangDictDtos.Any())
                return await Result<List<Guid>>
                    .FailAsync("No dictionary entries provided for update.");

            var entitiesToUpdate = _mapper.Map<List<MultiLangDictEntity>>(request.MultiLangDictDtos);

            // ✔ Use HashSet for faster lookup
            var ids = entitiesToUpdate.Select(x => x.Id).ToHashSet();

            var existingEntities = await _repository.Entities
                .Include(e => e.MultiLangs) // ✔ IMPORTANT
                .Where(e => ids.Contains(e.Id))
                .ToListAsync(cancellationToken);

            foreach (var existing in existingEntities)
            {
                var updated = entitiesToUpdate.FirstOrDefault(x => x.Id == existing.Id);
                if (updated == null)
                    continue;

                // ✔ Ensure collection is initialized
                existing.MultiLangs ??= new List<MultiLangDictItem>();

                // ✔ Update language values safely
                existing.MultiLangs = _mapper.Map<List<MultiLangDictItem>>(updated.MultiLangs);

                // ✔ Map remaining scalar properties (excluding collections via mapper config)
                _mapper.Map(updated, existing);
            }

            await _repository.UpdateRangeAsync(existingEntities);
            await _unitOfWork.Commit(cancellationToken);
           
            return Result<List<Guid>>.Success(existingEntities.Select(x => x.Id).ToList());

        }
    }
}
