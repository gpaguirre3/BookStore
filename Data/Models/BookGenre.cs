using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class BookGenre : BaseModel
    {
        public int BooksId { get; set; }
        public int GenresId { get; set; }

        public Book Book { get; set; }
        public Genre Genre { get; set; }
    }
}
