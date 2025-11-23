using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.Common;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CourtApp.Application.Features.Languages
{
    public class CreateLanguageCommand : IApplicationLayer, IRequest<Result<Guid>>
    {
        public int StateId { get; set; }
        public List<LangDto> Languages { get; set; }
    }

    public class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, Result<Guid>>
    {
        private readonly ILanguageRepository repository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public CreateLanguageCommandHandler(ILanguageRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
        {
            if (request.Languages == null || request.Languages.Count == 0)
                return await Result<Guid>.FailAsync("State wise langaue is not provided!");

            var stateLanguages = await repository.Entities
                                .AnyAsync(l =>
                                    l.StateId == request.StateId &&
                                    l.Languages.Any(lang =>
                                        request.Languages
                                        .Any(reqLang => reqLang.Code == lang.Code)
                                    )
                                );

            if (stateLanguages)
                return await Result<Guid>
                    .FailAsync("Provide the language combination for state is already exist!, please edit more");

            var mappedEntity = mapper.Map<LanguageEntity>(request);
            await repository.InsertAsync(mappedEntity);
            await unitOfWork.Commit(cancellationToken);
            return await Result<Guid>
                    .SuccessAsync("State language model is updated!");
        }
    }
}
