using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.DTO
{
    public record CheepDTO
    {
        public string Author { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string CreatedDate { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
    }
}
