using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mockAPI.Models;

namespace mockAPI.Repositories
{
    public interface IBooksRepository : IGenericRepository<Book>
    {
          Task<IReadOnlyCollection<Book>> GetBooksAsync(int pageSize, int lastId);
    }
}