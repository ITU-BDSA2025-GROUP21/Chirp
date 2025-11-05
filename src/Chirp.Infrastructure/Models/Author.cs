using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Infrastructure.Models
{
    public class Author : IdentityUser
    {
        public string Name { get; set; } = null!;
        
        public List<Cheep> Cheeps { get; set; } = new();
    }

}
