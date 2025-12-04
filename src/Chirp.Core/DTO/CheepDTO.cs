using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Chirp.Core.DTO
{
    public record CheepDTO
    {
        public string cheepId { get; set; } = null!;    
        public string Author { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string CreatedDate { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;
    }
}
