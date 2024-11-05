namespace Service.Models
{
    public class AuthorRequestModel
    {
        public int? Id { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Pseudonym {  get; set; }

        public DateTime? Birthdate { get; set; }
    }
}
