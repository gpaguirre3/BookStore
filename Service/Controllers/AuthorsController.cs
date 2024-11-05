using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Service.Models;
using Service.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController(IAuthorService bookService) : ControllerBase
    {
        private readonly IAuthorService _authorService = bookService;

        [HttpGet]
        [SwaggerOperation(OperationId = "GetAuthors")]
        public async Task<IEnumerable<Author>> GetAuthors()
        {
            return await _authorService.GetAllAuthors();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "GetAuthor")]
        public async Task<IActionResult> GetAuthor(int id)
        {
            var book = await _authorService.GetAuthorById(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpGet("/api/Authors/Search")]
        [SwaggerOperation(OperationId = "SearchAuthors")]

        public async Task<IEnumerable<Author>> SearchAuthors(string query)
        {
            return await _authorService.SearchAuthors(query);
        }

        [HttpPost]
        [SwaggerOperation(OperationId = "AddAuthor")]
        public async Task<IActionResult> AddAuthor([FromBody] AuthorRequestModel author)
        {
            try
            {
                bool success = await _authorService.AddAuthor(author);

                if (!success)
                {
                    return StatusCode(500, "No se pudo crear el autor");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPut]
        [SwaggerOperation(OperationId = "UpdateAuthor")]
        public async Task<IActionResult> UpdateAuthor([FromBody] AuthorRequestModel author)
        {
            bool success = await _authorService.UpdateAuthor(author);

            if (!success)
            {
                return StatusCode(500, "No se pudo actualizar el autor");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "DeleteAuthor")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _authorService.GetAuthorById(id);

            if (author == null)
            {
                return NotFound();
            }

            bool success = await _authorService.DeleteAuthor(author);

            if (!success)
            {
                return StatusCode(500, "No se pudo eliminar el autor");
            }

            return Ok();
        }
    }
}
