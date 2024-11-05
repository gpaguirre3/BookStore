using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PresentationWeb.ApiService;
using PresentationWeb.Models;
using WebClient.Controllers;

namespace PresentationWeb.Controllers
{
	public class AuthorsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookstoreApiService bookstoreApiService;

        public AuthorsController(ILogger<HomeController> logger)
		{
            _logger = logger;
            bookstoreApiService = new BookstoreApiService("http://localhost:5277", new HttpClient());
        }

		// GET: AuthorController
		public async Task<ActionResult> Index()
		{
			var authors = await bookstoreApiService.GetAuthorsAsync();

            return View("Index", new AuthorsIndexViewModel()
			{
				Authors = authors.ToList(),
			});
		}

		public IActionResult AddAuthor()
		{
			return View("AuthorForm", new BookFormViewModel());
		}

		public async Task<ActionResult> EditAuthor(int id)
		{
			Author? result;
			ICollection<Author> authors = [];
			ICollection<Genre> genres = [];

			try
			{
				result = await bookstoreApiService.GetAuthorAsync(id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				result = null;
			}

			return View("AuthorForm", new AuthorFormViewModel()
			{
				Author = result,
			});
		}

		public async Task<IActionResult> SaveAuthor([FromForm] AuthorFormModel data)
		{
			var errors = new List<string>();

			if (data.Firstname == null || data.Firstname.Trim().Length == 0)
			{
				errors.Add("El campo nombre(s) es necesario");
			}

			if (data.Lastname == null || data.Lastname.Trim().Length == 0)
			{
				errors.Add("El campo apellido(s) es necesario");
			}

			if (errors.Count > 0)
			{
				return View("AuthorForm", new AuthorFormViewModel()
				{
					Author = new Author()
					{
						Id = data.Id ?? 0,
						Firstname = data.Firstname ?? "",
						Lastname = data.Lastname ?? "",
						Pseudonym = data.Pseudonym,
					},
					Errors = errors.ToList()
				});
			}

			AuthorRequestModel author = new AuthorRequestModel()
			{
				Id = data.Id ?? 0,
				Firstname = data.Firstname,
				Lastname = data.Lastname,
				Pseudonym = data.Pseudonym
			};

			try
			{
				if (data.Id.HasValue)
				{
					await bookstoreApiService.UpdateAuthorAsync(author);
				}
				else
				{
					await bookstoreApiService.AddAuthorAsync(author);
				}

				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				return View("AuthorForm", new AuthorFormViewModel()
				{
					Author = new Author()
					{
						Id = data.Id ?? 0,
						Firstname = data.Firstname ?? "",
						Lastname = data.Lastname ?? "",
						Pseudonym = data.Pseudonym ?? "",
					},
					Errors = [ex.Message]
				});
			}
		}

		public async Task<ActionResult> DeleteAuthor(int id)
		{
			var author = await bookstoreApiService.GetAuthorAsync(id);

			if (author == null)
			{
                var authors = bookstoreApiService.GetAuthorsAsync();

                return View("Index", new AuthorsIndexViewModel()
                {
					Authors = bookstoreApiService.GetAuthorsAsync().Result.ToList(),
                    Errors = ["El autor no existe o ya fue eliminado anteriormente"]
                });
            }

			try
			{
				await bookstoreApiService.DeleteAuthorAsync(author.Id);
				return RedirectToAction("Index");
			} catch (Exception ex) {
                return View("Index", new AuthorsIndexViewModel()
                {
                    Authors = bookstoreApiService.GetAuthorsAsync().Result.ToList(),
                    Errors = [
						ex.Message.Contains("Book_Author_fk")
							? "No se puede eliminar un autor que es usado en un libro, elimine primero los libros que tengan el autor."
							: ex.Message
					]
                });
            }
		}
	}
}
