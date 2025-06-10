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

        private readonly BooksRepository _booksRepository;

        public BookService(BooksRepository booksRepository) : base(booksRepository)
        {
            _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
        }

         
    }
}