using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Core.Models
{
    public class Cheep
    {
        public int CheepId { get; set; }
        
        [ForeignKey(nameof(Author))]
        public string AuthorId { get; set; } = null!;
        
        public Author Author { get; set; } = null!;
        
        [Required]
        [StringLength(160)]
        public string Text { get; set; } = null!;
        
        public DateTime TimeStamp { get; set; }
        
        public HashSet<String> LikedBy { get; set; } = new();

        public HashSet<String> DislikedBy { get; set; } = new();
    
    }
}