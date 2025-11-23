using AspNetCoreHero.Results;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseDetails
{
    public class DeleteCaseDetailCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
    public class DeleteCaseDetailCommandHandler : IRequestHandler<DeleteCaseDetailCommand, Result<Guid>>
    {
        private readonly IUserCaseRepository _Repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICaseDocsRepository _caseDocRepo;
        private readonly ICaseProceedingRepository caseProceeding;

        public DeleteCaseDetailCommandHandler(IUserCaseRepository _Repository, IUnitOfWork unitOfWork,
            ICaseDocsRepository caseDocRepo, ICaseProceedingRepository caseProceeding)
        {
            this._Repository = _Repository;
            _unitOfWork = unitOfWork;
            _caseDocRepo = caseDocRepo;
            this.caseProceeding = caseProceeding;
        }

        public async Task<Result<Guid>> Handle(DeleteCaseDetailCommand command, CancellationToken cancellationToken)
        {
            // 1️⃣ Get the parent record
            var detail = await _Repository.GetByIdAsync(command.Id);
            if (detail == null)
                return Result<Guid>.Fail("Record not found for deletion!");

            // 2️⃣ Get and delete related Case Documents
            var caseDocs = await _caseDocRepo.Entities
                .Where(w => w.CaseId == command.Id)
                .ToListAsync(cancellationToken);

            if (caseDocs.Any())
                await _caseDocRepo.DeleteRangeAsync(caseDocs);

            // 3️⃣ Get and delete related Proceedings
            var procData = await caseProceeding.GetProceedingByCaseIdAsync(command.Id);
            if (procData != null && procData.Any())
                await caseProceeding.DeleteRangeAsync(procData);

            // 4️⃣ Delete the main record
            await _Repository.DeleteAsync(detail);

            // 5️⃣ Commit all changes atomically
            await _unitOfWork.Commit(cancellationToken);

            return Result<Guid>.Success(detail.Id);
        }
    }
}
