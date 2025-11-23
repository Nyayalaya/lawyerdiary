using CourtApp.Domain.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Application.Interfaces.Repositories
{
    public interface ICourtFeeStructureRepository
    {
        IQueryable<CourtFeeStructureEntity> Entites { get; }

        Task<List<CourtFeeStructureEntity>> GetListAsync();

        Task<CourtFeeStructureEntity> GetByIdAsync(Guid CaseUid);

        Task<Guid> InsertAsync(CourtFeeStructureEntity objEntity);

        Task UpdateAsync(CourtFeeStructureEntity objEntity);

        Task DeleteAsync(CourtFeeStructureEntity objEntity);
    }
}
