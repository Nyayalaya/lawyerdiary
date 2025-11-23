using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;

namespace CourtApp.Application.Features.Languages
{
    public class DeleteLangaugeCommand:IApplicationLayer, IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
    public class DeleteLangaugeCommandHandler : IRequestHandler<DeleteLangaugeCommand, Result<Guid>>
    {
        private readonly ILanguageRepository repository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public DeleteLangaugeCommandHandler(ILanguageRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(DeleteLangaugeCommand request, CancellationToken cancellationToken)
        {
           var langEntity=await repository.GetByIdAsync(request.Id);
            if(langEntity==null)
                return await Result<Guid>.FailAsync("Given language is not exist!");

            await repository.DeleteAsync(langEntity);
            await unitOfWork.Commit(cancellationToken);
            return await Result<Guid>
                   .SuccessAsync("State language model is deleted!");
        }
    }
}
