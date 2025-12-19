using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.Common;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
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
        private readonly IMultiLangWordRepository _multiRepo;
        private IUnitOfWork _unitOfWork { get; set; }
        public CreateCourtTypeCommandHandler(ICourtTypeRepository _Repository, IMapper _mapper,
            IUnitOfWork _unitOfWork, IMultiLangWordRepository _multiRepo)
        {
            this._unitOfWork = _unitOfWork;
            this._mapper = _mapper;
            this._Repository = _Repository;
            this._multiRepo = _multiRepo;
        }
        public async Task<Result<Guid>> Handle(CreateCourtTypeCommand request, CancellationToken cancellationToken)
        {
            // Normalize once
            var abbreviation = request.Abbreviation?.Trim();
            var courtType = request.CourtType?.Trim();

            // Duplicate CourtType check
            var isExists = await _Repository.CourtTypeEntities
                .AsNoTracking()
                .AnyAsync(x =>
                    (abbreviation == null || x.Abbreviation == abbreviation) &&
                    (courtType == null || x.CourtType == courtType),
                    cancellationToken);

            if (isExists)
                return Result<Guid>.Fail($"{courtType} already exists.");

            // Insert CourtType
            var entity = _mapper.Map<CourtTypeEntity>(request);
            entity.Languages = _mapper.Map<List<LangEntity>>(request.Language);

            await _Repository.InsertAsync(entity);
            await _unitOfWork.Commit(cancellationToken);
            // -------------------------------
            // Dictionary keyword handling
            // (delegated to BulkInsertAsync)
            // -------------------------------

            var keywords = request.CourtType
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new MultiLangDictEntity
                {
                    KeyWord = s
                })
                .ToList();

            if (keywords.Count > 0)
            {
                await _multiRepo.BulkInsertAsync(keywords);
                await _unitOfWork.Commit(cancellationToken);
            }

            return Result<Guid>.Success(entity.Id);



            //// Build dynamic predicate
            //var predicate = PredicateBuilder.True<CourtTypeEntity>();

            //if (!string.IsNullOrWhiteSpace(request.Abbreviation))
            //    predicate = predicate.And(b => b.Abbreviation.ToLower().Trim() == request.Abbreviation.ToLower().Trim());

            //if (!string.IsNullOrWhiteSpace(request.CourtType))
            //    predicate = predicate.And(b => b.CourtType.ToLower().Trim() == request.CourtType.ToLower().Trim());

            //// Check for duplicates
            //var isExists = await _Repository.CourtTypeEntities
            //    .Where(predicate)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync();

            //if (isExists != null)
            //    return Result<Guid>.Fail($"{request.CourtType} already exists.");

            //// Insert new record
            //var entity = _mapper.Map<CourtTypeEntity>(request);
            //entity.Languages = _mapper.Map<List<LangEntity>>(request.Language);
            //await _Repository.InsertAsync(entity);
            //await _unitOfWork.Commit(cancellationToken);

            //var CourtDatas = request.CourtType.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            //var dt = CourtDatas.Select(s => new MultiLangDictEntity
            //{
            //    KeyWord = s
            //}).ToList();
            //var dictData = _multiRepo.BulkInsertAsync(dt);

            //return Result<Guid>.Success(entity.Id);

        }
    }
}
