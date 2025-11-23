using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseTitle
{
    public class CreateCaseTitleCommand : IRequest<Result<Guid>>
    {
        public Guid CaseId { get; set; }
        public int TypeId { get; set; }
        public List<CaseApplicantDetail> CaseApplicants { get; set; }
    }

    public class CreateCaseTitleCommandHandler : IRequestHandler<CreateCaseTitleCommand, Result<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly IUserCaseRepository _UserCaseRepo;
        private readonly ICaseTitleRepository _caseTitleRepository;
        private IUnitOfWork _unitOfWork { get; set; }
        public CreateCaseTitleCommandHandler(IUserCaseRepository _UserCaseRepo,
            IMapper _mapper, IUnitOfWork unitOfWork, ICaseTitleRepository caseTitleRepository)
        {
            this._mapper = _mapper;
            this._UserCaseRepo = _UserCaseRepo;
            _unitOfWork = unitOfWork;
            _caseTitleRepository = caseTitleRepository;

        }
        public async Task<Result<Guid>> Handle(CreateCaseTitleCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Map the incoming request
            var entity = _mapper.Map<CaseTitleEntity>(request);
            entity.CaseApplicants = _mapper.Map<List<CaseApplicantDetailEntity>>(request.CaseApplicants);

            // Step 2: Load all existing applicants from all titles
            var existingTitles = await _caseTitleRepository.Titles.Where(w=>w.CaseId==request.CaseId)
                .AsNoTracking()
                .Include(e => e.CaseApplicants) // Important: include navigation property                
                .ToListAsync(cancellationToken);

            // Step 2: Check if any existing record has same CaseId + TypeId AND any applicants
            bool hasDuplicate = existingTitles.Any(existing =>
                existing.CaseId == entity.CaseId &&
                existing.TypeId == entity.TypeId &&
                existing.CaseApplicants.Any() // Ensures it actually has applicants
            );

            // Step 4: Check for any duplicate CaseId-TypeId pair
            if (hasDuplicate)
            {
                return Result<Guid>.Fail("Selected case detail and it's title type already exist, please edit the record for add more applicant.");
            }

            // Step 5: Proceed with save
            await _caseTitleRepository.InsertAsync(entity);
            await _unitOfWork.Commit(cancellationToken);

            return Result<Guid>.Success(entity.Id);


            //var entity = _mapper.Map<CaseTitleEntity>(request);
            //entity.CaseApplicants = _mapper.Map<List<CaseApplicantDetailEntity>>(request.CaseApplicants);
            //await _CaseTitleRepository.InsertAsync(entity);
            //await _unitOfWork.Commit(cancellationToken);
            //return Result<Guid>.Success(entity.Id);
        }
    }
}
