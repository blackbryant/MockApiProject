using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mockAPI.DataContext;
using mockAPI.Models;
using mockAPI.Repositories;
using mockAPI.Services;

namespace mockAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {

            _bookService = bookService;
        }
   
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            IReadOnlyList<Book> books = await _bookService.GetAllAsync();
            if (books == null || books.Count == 0)
                return NotFound("No books found.");
            var bookDtos = books.Select(b => new BookDTO
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Genre = b.Genre,
                ISBN = b.ISBN,
                PublicationDate = b.PublicationDate,
                Summary = b.Summary

            }).ToList();


            return Ok(books);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(BookDTO))]
        [ProducesResponseType(statusCode: 400, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 401, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 404, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<BookDTO>> Get(int id)
        {
            Book? book = await _bookService.GetByIdAsync(id);

            if (book == null)
                return NotFound($"Book with ID {id} not found.");

            var bookDto = new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                ISBN = book.ISBN,
                PublicationDate = book.PublicationDate,
                Summary = book.Summary
            };

            return Ok(bookDto);
        }

        [HttpPost]
        public async Task<ActionResult<BookDTO>> Post([FromBody] BookDTO book)
        {
            if (book == null)
                return BadRequest("Book data is null.");
            var newBook = new Book()
            {
                Id = 0,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                ISBN = book.ISBN,
                PublicationDate = book.PublicationDate,
                Summary = book.Summary
            };



            Book addedBook = await _bookService.AddAsync(newBook);
            var bookDto = new BookDTO
            {
                Id = addedBook.Id,
                Title = addedBook.Title,
                Author = addedBook.Author,
                Genre = addedBook.Genre,
                ISBN = addedBook.ISBN,
                PublicationDate = addedBook.PublicationDate,
                Summary = addedBook.Summary
            };


            return Ok(bookDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] BookDTO bookDto)
        {
            if (bookDto == null)
                return BadRequest("Book data is null.");
            if (id < 0 )
                return NotFound($"Book with ID {id} not found.");

            try
            {
                var existingBook = await _bookService.GetByIdAsync(id);
                if (existingBook == null)
                    return NotFound($"Book with ID {id} not found.");

                Book book = new Book
                {
                    Id = id,
                    Title = bookDto.Title,
                    Author = bookDto.Author,
                    Genre = bookDto.Genre,
                    ISBN = bookDto.ISBN,
                    PublicationDate = bookDto.PublicationDate,
                    Summary = bookDto.Summary
                };

                _bookService.Update(book);
                var updatedBookDto = new BookDTO
                {
                    Id = existingBook.Id,
                    Title = existingBook.Title,
                    Author = existingBook.Author,
                    Genre = existingBook.Genre,
                    ISBN = existingBook.ISBN,
                    PublicationDate = existingBook.PublicationDate,
                    Summary = existingBook.Summary
                };
                return Ok(updatedBookDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(int id)
        {
            if (id < 0)
                return Task.FromResult<IActionResult>(NotFound($"Book with ID {id} not found."));

            try
            {
                var book = _bookService.GetByIdAsync(id);
                if (book == null)
                    return Task.FromResult<IActionResult>(NotFound($"Book with ID {id} not found."));

                var deleteBook = new Book
                {
                    Id = id
                };  
                _bookService.Delete(deleteBook);
                return Task.FromResult<IActionResult>(NoContent());
            }
            catch (Exception ex)
            {
                return Task.FromResult<IActionResult>(StatusCode(500, $"Internal server error: {ex.Message}"));
            }
        }


    }
}