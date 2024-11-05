using Service.Converters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Service.Models
{
    public class BookRequestModel
    {
        public int? Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "El título es requerido")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "El año es requerido")]
        public int Year { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "La editorial es requerida")]
        public string? Publisher { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "El ISBN es requerido")]
        public string? ISBN { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0, int.MaxValue)]
        public decimal Price { get; set; } = decimal.Zero;

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; } = 0;

        public byte[] Image { get; set; } = [];

        public AuthorRequestModel? Author { get; set; }

        public List<GenreRequestModel> Genres { get; set; } = [];
    }
}
