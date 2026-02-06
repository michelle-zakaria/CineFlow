namespace CineFlow.Areas.Admin.Data.TypeConfigurations
{
    public class BookingSeatConfiguration : BaseEntityConfiguration<BookingSeat>
    {
        protected override string TableName => "BookingSeats";
        protected override string? Schema => "admins";

        protected override void ConfigureEntity(EntityTypeBuilder<BookingSeat> builder)
        {
            builder.Property(bs => bs.SeatNumber)
            .IsRequired()
            .HasMaxLength(10);

            builder.Property(bs => bs.Row)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(bs => bs.SeatIndex)
                .IsRequired();

            builder.Property(bs => bs.SeatType)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Regular");

            builder.Property(bs => bs.Price)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(bs => bs.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Foreign key relationship with Booking
            builder.HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingSeats)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(bs => bs.BookingId);
            builder.HasIndex(bs => bs.SeatNumber);

            // Unique constraint for seat in a booking
            builder.HasIndex(bs => new { bs.BookingId, bs.SeatNumber })
                .IsUnique();
        }
    }
}
