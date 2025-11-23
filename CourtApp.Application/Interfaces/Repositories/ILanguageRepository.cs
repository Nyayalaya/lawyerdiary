using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourtApp.Domain.Entities.Common;
using CourtApp.Domain.Entities.LawyerDiary;

namespace CourtApp.Application.Interfaces.Repositories
{
    public interface ILanguageRepository:IApplicationLayer
    {
        IQueryable<LanguageEntity> Entities { get; }

        Task<List<LanguageEntity>> GetListAsync();

        Task<LanguageEntity> GetByIdAsync(Guid Id);

        Task<Guid> InsertAsync(LanguageEntity entity);

        Task UpdateAsync(LanguageEntity entity);

        Task DeleteAsync(LanguageEntity entity);
    }
}
