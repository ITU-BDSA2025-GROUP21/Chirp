using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Core.Models
{
    public class Author : IdentityUser
    {
        public string Name { get; set; } = null!;

        public DateTime CreationDate { get; set; } = DateTime.Now;
        
        public List<Cheep> Cheeps { get; set; } = new();

        public int karma { get; set; } = 0;
    }
}