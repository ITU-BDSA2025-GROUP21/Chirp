
namespace Chirp.Core.DTO
{
    public record UserFollowDTO
    {
        public string FollowerId { get; set; } = null!;
        public string FolloweeId { get; set; } = null!;
        public string TimeStamp { get; set; } = null!;
    }
}
