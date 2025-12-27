namespace Chirp.Application.DTO
{
    public record CheepDTO
    {
        public int cheepId { get; set; } = 0;    
        public string Author { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string CreatedDate { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;
    }
}