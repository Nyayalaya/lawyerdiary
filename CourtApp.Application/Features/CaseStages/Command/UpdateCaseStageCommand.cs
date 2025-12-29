using AspNetCoreHero.Results;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseStages.Command
{
    public class UpdateCaseStageCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public string CaseStage { get; set; }
    }

    public class UpdateCaseStageCommandHandler : IRequestHandler<UpdateCaseStageCommand, Result<Guid>>
    {
        private readonly ICaseStageRepository repository;
        private readonly IMultiLangWordRepository _multiRepo;
        private IUnitOfWork _unitOfWork { get; set; }
        public UpdateCaseStageCommandHandler(ICaseStageRepository repository, IUnitOfWork _unitOfWork)
        {
            this.repository = repository;
            this._unitOfWork = _unitOfWork;
        }

        public async Task<Result<Guid>> Handle(UpdateCaseStageCommand request, CancellationToken cancellationToken)
        {

            // Fetch the record to update
            var existingRecord = await repository.GetByIdAsync(request.Id);
            if (existingRecord == null)
                return Result<Guid>.Fail("Record does not exist.");


            // Check for name conflict with other records (excluding the current record)
            var duplicateRecord = await repository.QryEntities
                .Where(e => e.Id != request.Id && e.CaseStage.ToLower() == request.CaseStage.ToLower().Trim())
                .FirstOrDefaultAsync(cancellationToken);

            if (duplicateRecord != null)
                return Result<Guid>.Fail("Another record with the same name already exists.");

            // Update the entity fields
            existingRecord.CaseStage = request.CaseStage;


            await repository.UpdateAsync(existingRecord);
            await _unitOfWork.Commit(cancellationToken);

            return Result<Guid>.Success(existingRecord.Id);
        }
    }
}
