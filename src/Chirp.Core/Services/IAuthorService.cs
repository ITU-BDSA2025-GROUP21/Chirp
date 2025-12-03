using Chirp.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.Core.Services
{
    public interface IAuthorService
    {

        AuthorDTO? FindAuthorByEmail(string email);
        AuthorDTO? FindAuthorByName(string name);

        IEnumerable<AuthorDTO> GetFollowers(string name);
        IEnumerable<AuthorDTO> GetFollowing(string name);

        bool IsFollowing(string followerName, string followeeName);
        void FollowAuthor(string followerName, string followeeName);
        void UnfollowAuthor(string followerName, string followeeName);
    }
}
