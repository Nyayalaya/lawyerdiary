using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.Common;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;

namespace CourtApp.Application.Features.Languages
{
    public class UpdateLangaugeCommand : IApplicationLayer, IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public int StateId { get; set; }
        public List<LangDto> Languages { get; set; }
    }
    public class UpdateLanguageCommandHandler : IRequestHandler<UpdateLangaugeCommand, Result<Guid>>
    {
        private readonly ILanguageRepository repository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public UpdateLanguageCommandHandler(ILanguageRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(UpdateLangaugeCommand request, CancellationToken cancellationToken)
        {
            var lngDetail = await repository.GetByIdAsync(request.Id);
            if (lngDetail == null)
                return await Result<Guid>.FailAsync("Detail not found!");

            // Update StateId
            lngDetail.StateId = request.StateId;

            // Clear and repopulate Languages
            lngDetail.Languages = request.Languages?.Select(lng => new Domain.Entities.Common.LangEntity
            {
                Code = lng.Code,
                Name = lng.Name
            }).ToList() ?? new List<Domain.Entities.Common.LangEntity>();

            // Persist changes
            await repository.UpdateAsync(lngDetail);
            await unitOfWork.Commit(cancellationToken);

            return await Result<Guid>.SuccessAsync("State language model updated successfully!");

        }
    }
}
