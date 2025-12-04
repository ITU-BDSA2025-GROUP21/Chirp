using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Core.Models
{
    public class Author : IdentityUser
    {
        [PersonalData] public string Name { get; set; } = null!;

        [PersonalData] public DateTime CreationDate { get; set; } = DateTime.Now;
        
        public List<Cheep> Cheeps { get; set; } = new();
    }

}