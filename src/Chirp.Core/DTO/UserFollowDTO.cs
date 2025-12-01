
namespace Chirp.Core.DTO
{
    public record UserFollowDTO
    {
        public string FollowerId { get; set; }
        public string Follower { get; set; }
        public string FolloweeId { get; set; }
        public string Followee { get; set; }
        public string TimeStamp { get; set; }
    }
}
