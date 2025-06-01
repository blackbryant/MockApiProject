
using Microsoft.EntityFrameworkCore;
using mockAPI.DataContext;
using mockAPI.Models;

namespace mockAPI.Repositories
{
    public class BooksRepository : GenericRepository<Book> , IBooksRepository
    {

        public BooksRepository(AppDbContext context) : base(context)
        {
          
        }

        public async Task<IReadOnlyCollection<Book>> GetBooksAsync(int pageSize, int lastId)
        {
            return await _context.Books
                .Where(b => b.Id > lastId)
                .OrderBy(b => b.Id)
                .Take(pageSize)
                .ToListAsync();
        }
  
    }

    
}