using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Service.Models;
using Service.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController(IGenreService genreService) : ControllerBase
    {
        private readonly IGenreService _genreService = genreService;

        [HttpGet]
        [SwaggerOperation(OperationId = "GetGenres")]
        public async Task<IEnumerable<Genre>> GetGenres()
        {
            return await _genreService.GetAllGenres();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "GetGenre")]
        public async Task<IActionResult> GetGenre(int id)
        {
            var book = await _genreService.GetGenreById(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpGet("/api/Genres/Search")]
        [SwaggerOperation(OperationId = "SearchGenres")]

        public async Task<IEnumerable<Genre>> SearchGenres(string query)
        {
            return await _genreService.SearchGenres(query);
        }

        [HttpPost]
        [SwaggerOperation(OperationId = "AddGenre")]
        public async Task<IActionResult> AddGenre([FromBody] GenreRequestModel genre)
        {
            try
            {
                bool success = await _genreService.AddGenre(genre);

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
        [SwaggerOperation(OperationId = "UpdateGenre")]
        public async Task<IActionResult> UpdateGenre([FromBody] GenreRequestModel genre)
        {
            bool success = await _genreService.UpdateGenre(genre);

            if (!success)
            {
                return StatusCode(500, "No se pudo actualizar el autor");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "DeleteGenre")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _genreService.GetGenreById(id);

            if (genre == null)
            {
                return NotFound();
            }

            bool success = await _genreService.DeleteGenre(genre);

            if (!success)
            {
                return StatusCode(500, "No se pudo eliminar el autor");
            }

            return Ok();
        }
    }
}
