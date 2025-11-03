namespace Chirp.Core.DTO
{
    public record AuthorDTO
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}