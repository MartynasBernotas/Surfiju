using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DevKnowledgeBase.Domain.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }  // Optional, for profile picture URL
        public string? RefreshToken { get; set; } 
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public ICollection<Trip> OrganizedTrips { get; set; } = new List<Trip>();
    }

}
