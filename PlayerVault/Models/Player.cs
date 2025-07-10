using System.ComponentModel.DataAnnotations;

namespace PlayerVault.Models
{
    public class Player
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Position { get; set; }

        public string Nationality { get; set; }

        public int Age { get; set; }

        public decimal MarketValue { get; set; } // In million €

        public bool IsApproved { get; set; } = false; // Default: Not approved

        public int UserId { get; set; }

        public User User { get; set; } // Optional, for relationship tracking
    }
}
