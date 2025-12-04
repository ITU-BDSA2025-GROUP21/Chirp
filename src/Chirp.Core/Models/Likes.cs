using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.Core.Models
{
    public class Likes
    {
        [ForeignKey(nameof(Author))]
        public string authorId { get; set; } = null!;
        
        [ForeignKey(nameof(Cheep))]
        public int CheepId { get; set; }

        public int likeStatus { get; set; } = 0; // 1 = like, -1 = dislike, 0 = neutral
        public Author Author { get; set; } = null!;
        public Cheep Cheep { get; set; } = null!;
    }
}
