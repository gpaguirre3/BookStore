using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Service.Models;
using Service.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoansController(ILoanService loanService) : ControllerBase
	{
		private readonly ILoanService _loanService = loanService;

		[HttpGet]
		[SwaggerOperation(OperationId = "GetLoans")]
		public async Task<IEnumerable<Loan>> GetLoans()
		{
			return await _loanService.GetAllLoans();
		}

		[HttpGet("{id}")]
		[SwaggerOperation(OperationId = "GetLoan")]
		public async Task<IActionResult> GetLoan(int id)
		{
			var book = await _loanService.GetLoanById(id);

			if (book == null)
			{
				return NotFound();
			}

			return Ok(book);
		}

		[HttpGet("/api/Loans/Search")]
		[SwaggerOperation(OperationId = "SearchLoans")]

		public async Task<IEnumerable<Loan>> SearchLoans(int? bookId, string? identificationNumber, string status)
		{
			return await _loanService.FindLoans(l =>
			{
				return (!bookId.HasValue || l.BookId == bookId.Value)
					&& (identificationNumber == null || l.IdentificationNumber == identificationNumber)
					&& l.Status == status;
			});
		}

		[HttpPost]
		[SwaggerOperation(OperationId = "AddLoan")]
		public async Task<IActionResult> AddLoan([FromBody] LoanRequestModel loan)
		{
			try
			{
				bool success = await _loanService.AddLoan(loan);

				if (!success)
				{
					return StatusCode(500, "No se pudo generar el prestamo");
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

			return Ok();
		}

		[HttpPut]
		[SwaggerOperation(OperationId = "UpdateLoan")]
		public async Task<IActionResult> UpdateLoan([FromBody] LoanRequestModel loan)
		{
			bool success = await _loanService.UpdateLoan(loan);

			if (!success)
			{
				return StatusCode(500, "No se pudo actualizar el prestamo");
			}

			return Ok();
		}
	}
}
