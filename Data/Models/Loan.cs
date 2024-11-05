using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
	public class Loan : BaseModel
	{
		public int Id { get; set; }
		public int BookId { get; set; }
		public string IdentificationNumber { get; set; }
		public string Email { get; set; }
		public string Status { get; set; }
		public DateTime LoanedOn { get; set; }
		public Book Book { get; set; }
	}
}
