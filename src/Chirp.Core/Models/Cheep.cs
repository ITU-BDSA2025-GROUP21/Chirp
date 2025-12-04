using Microsoft.AspNetCore.Identity;
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
        [PersonalData] public string Text { get; set; } = null!;

        [PersonalData] public DateTime TimeStamp { get; set; }
    }
}