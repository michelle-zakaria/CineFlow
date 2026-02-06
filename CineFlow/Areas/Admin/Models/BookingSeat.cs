namespace CineFlow.Areas.Dashboard.Models
{
    public class BookingSeat : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; }

        [Required]
        [MaxLength(10)]
        public string SeatNumber { get; set; } // Format: "A12", "B5", etc.

        [Required]
        [MaxLength(20)]
        public string Row { get; set; }

        [Required]
        public int SeatIndex { get; set; }

        [Required]
        [MaxLength(20)]
        public string SeatType { get; set; } = "Regular";

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
