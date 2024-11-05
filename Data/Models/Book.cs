using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Book : BaseModel
    {
        
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? Year { get; set; }
        public string? Publisher { get; set; }
        public string? ISBN { get; set; }
        public decimal? Price { get; set; }
        public string? Image { get; set; }

        [JsonIgnore]
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public ICollection<Genre> Genres { get; set; } = [];
    }
}
