namespace CineFlow.Areas.Dashboard.Models
{
    public class Booking : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BookingNumber { get; set; } = GenerateBookingNumber();

        [Required]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime ShowDate { get; set; }

        [Required]
        public string ShowTime { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Number of tickets must be between 1 and 20")]
        public int NumberOfTickets { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        // Seat Information
        [MaxLength(50)]
        public string SeatNumbers { get; set; } // Comma-separated seat numbers

        [Required]
        [MaxLength(20)]
        public string SeatType { get; set; } = "Regular"; // Regular, Premium, VIP

        // Booking Status
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Confirmed"; // Confirmed, Cancelled, Completed

        [MaxLength(500)]
        public string? SpecialInstructions { get; set; }

        // Foreign Keys
        [Required]
        public int MovieId { get; set; }

        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; }

        public int? UserId { get; set; } // If you have user authentication

        [MaxLength(100)]
        public string? CustomerName { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? CustomerEmail { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? CustomerPhone { get; set; }

        // Payment Information
        [MaxLength(50)]
        public string? PaymentMethod { get; set; } // Credit Card, PayPal, Cash

        [MaxLength(100)]
        public string? PaymentTransactionId { get; set; }

        public DateTime? PaymentDate { get; set; }

        [MaxLength(20)]
        public string? PaymentStatus { get; set; } // Paid, Pending, Failed

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();

        // Helper Methods
        private static string GenerateBookingNumber()
        {
            return $"BK{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        public decimal CalculateTotal()
        {
            // Calculate based on movie price and seat type
            decimal basePrice = Movie?.Price ?? 0;
            decimal seatMultiplier = GetSeatTypeMultiplier();
            return basePrice * seatMultiplier * NumberOfTickets;
        }

        private decimal GetSeatTypeMultiplier()
        {
            return SeatType.ToLower() switch
            {
                "premium" => 1.3m, // 30% more expensive
                "vip" => 1.5m,     // 50% more expensive
                _ => 1.0m          // Regular price
            };
        }

        public bool IsActive()
        {
            return Status != "Cancelled" && ShowDate >= DateTime.Now.Date;
        }

        public bool CanBeCancelled()
        {
            return Status == "Confirmed" &&
                   (ShowDate - DateTime.Now).TotalHours > 2; // Can cancel up to 2 hours before show
        }
    }
}
