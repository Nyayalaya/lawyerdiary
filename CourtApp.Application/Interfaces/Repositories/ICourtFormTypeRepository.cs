using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourtApp.Domain.Entities.FormBuilder;

namespace CourtApp.Application.Interfaces.Repositories
{
    public interface ICourtFormTypeRepository:IApplicationLayer
    {
        IQueryable<CourtFormTypeEntity> Entities { get; }

        Task<List<CourtFormTypeEntity>> GetListAsync();

        Task<CourtFormTypeEntity> GetByIdAsync(Guid Id);

        Task<Guid> InsertAsync(CourtFormTypeEntity entity);

        Task UpdateAsync(CourtFormTypeEntity entity);

        Task DeleteAsync(CourtFormTypeEntity entity);
    }
}
