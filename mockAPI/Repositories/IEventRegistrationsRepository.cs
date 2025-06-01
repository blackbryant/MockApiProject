using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mockAPI.Models;

namespace mockAPI.Repositories
{
    public interface IEventRegistrationsRepository : IGenericRepository<EventRegistration>
    {
        Task<(IReadOnlyCollection<EventRegistration> Items, bool HasNextPage)> GetEventRegistrationsAsync(int pageSize, int lastId);

       

       
    }
}