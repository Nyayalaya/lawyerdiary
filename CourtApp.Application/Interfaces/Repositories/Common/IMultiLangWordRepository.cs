using CourtApp.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.Interfaces.Repositories.Common
{
    public interface IMultiLangWordRepository
    {
        IQueryable<MultiLangDictEntity> Entities { get; }
        Task<List<MultiLangDictEntity>> GetListByLanCodeAsync(string langCode);
        Task<MultiLangDictEntity> GetByIdAsync(Guid Id);
        Task<List<Guid>> BulkInsertAsync(List<MultiLangDictEntity> dictEntities);
        Task UpdateAsync(MultiLangDictEntity dictEntity);
        Task<List<Guid>> UpdateRangeAsync(List<MultiLangDictEntity> entities);
        Task DeleteAsync(MultiLangDictEntity dictEntity);
    }
}
