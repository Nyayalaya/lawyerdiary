
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
using CourtApp.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace CourtApp.Infrastructure.Repositories
{
    public class StateMasterRepository:IStateMasterRepository
    {
        private readonly IRepositoryAsync<StateEntity> _repository;
        private readonly IDistributedCache _distributedCache;
        public StateMasterRepository(IRepositoryAsync<StateEntity> _repository,IDistributedCache _distributedCache)
        {
         this._repository=_repository;
         this._distributedCache=_distributedCache;   
        }

        public IQueryable<StateEntity> Entities => _repository.Entities;

        public StateEntity GetStateById(int Id)
        {
            return _repository.GetByIdAsync(Id).Result;
        }

        public async Task<List<StateEntity>> GetStateListAsync()
        {
            return await _repository.Entities.ToListAsync();
        }
    }
}