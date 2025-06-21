using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mockAPI.DataContext;
using mockAPI.Models;
using mockAPI.Repositories;

namespace mockAPI.Services
{
    public class BookService : GenericService<Book>, IBookService
    {

        private readonly IBooksRepository _booksRepository;

        public BookService(IBooksRepository booksRepository) : base(booksRepository)
        {
            this._booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
        }

         
    }
}