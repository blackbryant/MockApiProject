using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mockAPI.DataContext;
using mockAPI.Models;

namespace mockAPI.Repositories
{
    public class EventRegistrationsRepository : GenericRepository<EventRegistration>, IEventRegistrationsRepository
      
    {


        public EventRegistrationsRepository(AppDbContext context) : base(context)
        {
        }

        public Task<(IReadOnlyCollection<EventRegistration> Items, bool HasNextPage)> GetEventRegistrationsAsync(int pageSize, int lastId)
        {
            throw new NotImplementedException();
        }
        

    }

     
}