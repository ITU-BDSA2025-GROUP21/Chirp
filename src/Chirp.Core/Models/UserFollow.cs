using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Core.Models
{
    public class UserFollow
    {
        /* A 'Follower' FOLLOWS a 'Followee' */

        [ForeignKey(nameof(Author))]
        public string FollowerId { get; set; } = null!;

        public Author Follower { get; set; } = null!;

        [ForeignKey(nameof(Author))]
        public string FolloweeId { get; set; } = null!;

        public Author Followee { get; set; } = null!;

        public DateTime TimeStamp { get; set; }
    }
}
