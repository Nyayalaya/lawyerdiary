using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.Common;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.Common;
using CourtApp.Domain.Entities.LawyerDiary;
using KT3Core.Areas.Global.Classes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CourtApp.Application.Features.CourtType.Command
{
    public class CreateCourtTypeCommand : IRequest<Result<Guid>>
    {
        public string CourtType { get; set; }
        public string Abbreviation { get; set; }
        public List<LangDto> Language { get; set; }
    }
    public class CreateCourtTypeCommandHandler : IRequestHandler<CreateCourtTypeCommand, Result<Guid>>
    {
        private readonly ICourtTypeRepository _Repository;
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork { get; set; }
        public CreateCourtTypeCommandHandler(ICourtTypeRepository _Repository, IMapper _mapper, IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
            this._mapper = _mapper;
            this._Repository = _Repository;
        }
        public async Task<Result<Guid>> Handle(CreateCourtTypeCommand request, CancellationToken cancellationToken)
        {
            // Build dynamic predicate
            var predicate = PredicateBuilder.True<CourtTypeEntity>();

            if (!string.IsNullOrWhiteSpace(request.Abbreviation))
                predicate = predicate.And(b => b.Abbreviation.ToLower().Trim() == request.Abbreviation.ToLower().Trim());

            if (!string.IsNullOrWhiteSpace(request.CourtType))
                predicate = predicate.And(b => b.CourtType.ToLower().Trim() == request.CourtType.ToLower().Trim());

            // Check for duplicates
            var isExists = await _Repository.CourtTypeEntities
                .Where(predicate)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (isExists != null)
                return Result<Guid>.Fail($"{request.CourtType} already exists.");

            // Insert new record
            var entity = _mapper.Map<CourtTypeEntity>(request);
            entity.Languages = _mapper.Map<List<LangEntity>>(request.Language);
            await _Repository.InsertAsync(entity);
            await _unitOfWork.Commit(cancellationToken);

            return Result<Guid>.Success(entity.Id);

        }
    }
}
