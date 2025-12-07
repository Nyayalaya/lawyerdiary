using CourtApp.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.Interfaces.CacheRepositories.Common
{
    public interface IMultiLangWordCacheRepository
    {
        Task<List<MultiLangDictEntity>> GetListByLanCodeAsync(string langCode);
        Task<MultiLangDictEntity> GetByIdAsync(Guid Id);
    }
}
