using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.FormBuilder;
using CourtApp.Application.Interfaces.CacheRepositories.FormBuilder;
using CourtApp.Application.Interfaces.Repositories.FormBuilder;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace CourtApp.Application.Features.FormBuilder
{
    public class GetCaseDarftingCachedByIdQuery : IRequest<Result<CaseDarftingDetailDtoByIdResponse>>
    {
        public Guid Id { get; set; }
    }

    public class GetCaseDarftingCachedByIdQueryHandler : IRequestHandler<GetCaseDarftingCachedByIdQuery, Result<CaseDarftingDetailDtoByIdResponse>>
    {
        private readonly ICaseDraftingRepository _Repository;
        private readonly IFormBuilderCacheRepository _FrmRepository;
        private readonly IMapper _mapper;
        public GetCaseDarftingCachedByIdQueryHandler(ICaseDraftingRepository _Repository,
            IMapper _mapper,
            IFormBuilderCacheRepository formBuilderCacheRepository)
        {
            this._Repository = _Repository;
            this._mapper = _mapper;
            this._FrmRepository = formBuilderCacheRepository;
        }
        public async Task<Result<CaseDarftingDetailDtoByIdResponse>> Handle(GetCaseDarftingCachedByIdQuery request, CancellationToken cancellationToken)
        {
            CaseDarftingDetailDtoByIdResponse detail = new CaseDarftingDetailDtoByIdResponse();
            var dt = _Repository.Entities.Where(w => w.Id == request.Id).FirstOrDefault();
            detail.CaseId = dt.CaseId;
            detail.TemplateId = dt.TemplateId;
            detail.Id = request.Id;
            detail.LangCode = dt.LangCode;
            //var fmr = dt.FieldDetails.Select(s => new FormFieldDetailValue {Tag=s.Tag,Value=s.Value });

            if (dt.FieldDetails != null)
            {
                var fbr = await _FrmRepository.GetByIdAsync(dt.FormId);
                var frmDetails = fbr.FieldsDetails.Fields.ToList();
                List<FormFieldDetailValue> fmr = new List<FormFieldDetailValue>();
                foreach (var item in frmDetails)
                {
                    var fm = new FormFieldDetailValue();
                    fm.Key = item.Key;
                    fm.DispOrder = item.DispOrder;
                    fm.Name = item.Name;
                    fm.Type = item.Type;
                    fm.Tag = item.Tag;
                    fm.Value = dt.FieldDetails.Where(s => s.Tag == item.Tag)
                    .Select(s => s.Value).FirstOrDefault();
                    fm.DefaultVal = item.DefaultVal;
                    fmr.Add(fm);
                }
                //var frslt = from f in frmDetails
                //            join d in dt.FieldDetails on f.Key equals d.Key into td
                //            from t in td.DefaultIfEmpty()
                //            select new FormFieldDetailValue
                //            {
                //                Key = f.Key,
                //                DispOrder = f.DispOrder,
                //                IsRequire = f.IsRequire,
                //                Name = f.Name,
                //                Placeholder = f.Placeholder,
                //                Type = f.Type,
                //                FieldSize = _mapper.Map<FieldSizeDto>(f.FieldSize),
                //                Value = t == null ? "" : t.Value
                //            };
                detail.FieldDetails = fmr.ToList();
            }
            return await Result<CaseDarftingDetailDtoByIdResponse>.SuccessAsync(detail);
        }
    }
}
