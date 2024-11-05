using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Service.Models;
using Service.Services;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController(IBookService bookService) : ControllerBase
    {
        private readonly IBookService _bookService = bookService;

        // GET: api/<BooksController>
        [HttpGet]
        [SwaggerOperation(OperationId = "GetBooks")]
        public async Task<IEnumerable<Book>> GetBooks()
        {
            return await _bookService.GetAllBooks();
        }

        // GET api/<BooksController>/5
        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "GetBook")]
        public async Task<Book?> GetBook(int id)
        {
            return await _bookService.GetBookById(id);
        }

        [HttpGet("/api/Books/Search")]
        [SwaggerOperation(OperationId = "SearchBooks")]
        public async Task<IEnumerable<Book>> SearchBooks(string query)
        {
            return await _bookService.SearchBooks(query);
        }

        // POST api/<BooksController>
        [HttpPost]
        [SwaggerOperation(OperationId = "AddBook")]
        public async Task<IActionResult> AddBook([FromBody] BookRequestModel value)
        {
            try
            {
                bool success = await _bookService.AddBook(value);

                if (!success)
                {
                    return StatusCode(500, "No se pudo crear el libro");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        // PUT api/<BooksController>/5
        [HttpPut]
        [SwaggerOperation(OperationId = "UpdateBook")]
        public async Task<IActionResult> UpdateBook([FromBody] BookRequestModel value)
        {
            bool success = await _bookService.UpdateBook(value);

            if (!success)
            {
                return StatusCode(500, "No se pudo actualizar el libro");
            }

            return Ok();
        }

        // DELETE api/<BooksController>/5
        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "DeleteBook")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _bookService.GetBookById(id);

            if (book == null)
            {
                return NotFound();
            }

            bool success = await _bookService.DeleteBook(book);

            if (!success)
            {
                return StatusCode(500, "No se pudo eliminar el libro");
            }

            return Ok();
        }
    }
}
