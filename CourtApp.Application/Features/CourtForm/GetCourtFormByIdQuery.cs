using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.CourtForm;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;

namespace CourtApp.Application.Features.CourtForm
{
    public class GetCourtFormByIdQuery : IRequest<Result<CourtFormByIdDto>>
    {
        public Guid Id { get; set; }
    }
    public class GetCourtFormByIdQueryHandler : IRequestHandler<GetCourtFormByIdQuery, Result<CourtFormByIdDto>>
    {
        private readonly ICourtFormTypeRepository repository;
        private readonly IMapper mapper;
        public GetCourtFormByIdQueryHandler(ICourtFormTypeRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<CourtFormByIdDto>> Handle(GetCourtFormByIdQuery request, CancellationToken cancellationToken)
        {
            var courtFormDetail = await repository.GetByIdAsync(request.Id);

            if (courtFormDetail == null)
                return await Result<CourtFormByIdDto>.FailAsync("There was a problem retrieving the information.");

            var courtFormDto = mapper.Map<CourtFormByIdDto>(courtFormDetail);

            return await Result<CourtFormByIdDto>.SuccessAsync(courtFormDto);

        }
    }
}
