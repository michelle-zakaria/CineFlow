namespace CineFlow.Areas.Admin.Data.TypeConfigurations
{
    public class BookingConfiguration : BaseEntityConfiguration<Booking>
    {
        protected override string TableName => "Bookings";
        protected override string? Schema => "admins";

        protected override void ConfigureEntity(EntityTypeBuilder<Booking> builder)
        {
            builder.Property(b => b.BookingNumber)
           .IsRequired()
           .HasMaxLength(50)
           .IsUnicode(false);

            builder.Property(b => b.BookingDate)
                .IsRequired();

            builder.Property(b => b.ShowDate)
                .IsRequired();

            builder.Property(b => b.ShowTime)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(b => b.NumberOfTickets)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(b => b.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.SeatNumbers)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(b => b.SeatType)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Regular");

            builder.Property(b => b.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Confirmed");

            builder.Property(b => b.SpecialInstructions)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(b => b.CustomerName)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(b => b.CustomerEmail)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(b => b.CustomerPhone)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(b => b.PaymentMethod)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(b => b.PaymentTransactionId)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(b => b.PaymentStatus)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(b => b.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(b => b.UpdatedAt)
                .IsRequired(false);

            // Foreign key relationship with Movie
            builder.HasOne(b => b.Movie)
                .WithMany(m => m.Bookings)
                .HasForeignKey(b => b.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(b => b.BookingNumber)
                .IsUnique();

            builder.HasIndex(b => b.BookingDate);
            builder.HasIndex(b => b.ShowDate);
            builder.HasIndex(b => b.Status);
            builder.HasIndex(b => b.CustomerEmail);
            builder.HasIndex(b => b.MovieId);

            // Composite index
            builder.HasIndex(b => new { b.MovieId, b.ShowDate, b.ShowTime });
        }
    }
}
