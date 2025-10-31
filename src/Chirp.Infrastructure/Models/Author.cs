namespace Chirp.Infrastructure.Models
{
    public class Author
    {
        public int AuthorId { get; set; }                 // PK from dump
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<Cheep> Cheeps { get; set; } = new();
    }

}
