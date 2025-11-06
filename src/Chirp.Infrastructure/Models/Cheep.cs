using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models
{
    public class Cheep
    {
        public int CheepId { get; set; }
        public string AuthorId { get; set; } = null!;
        public Author Author { get; set; } = null!;
        [Required]
        [StringLength(160)]
        public string Text { get; set; } = null!;
        public DateTime TimeStamp { get; set; }
    }

}