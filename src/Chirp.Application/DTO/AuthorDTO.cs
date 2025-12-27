namespace Chirp.Application.DTO
{
    public record AuthorDTO
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string CreationDate { get; set; } = null!;

        public string ProfilePicPath { get; set; }
        public int karma { get; set; }
        public string Id { get; set; } = null!;
    }
}
