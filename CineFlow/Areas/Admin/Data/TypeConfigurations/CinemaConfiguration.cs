namespace CineFlow.Areas.Admin.Data.TypeConfigurations
{
    public class CinemaConfiguration : BaseEntityConfiguration<Cinema>
    {
        protected override string TableName => "Cinemas";
        protected override string? Schema => "admin";

        protected override void ConfigureEntity(EntityTypeBuilder<Cinema> builder)
        {
            builder.Property(c => c.Logo)
             .IsRequired()
             .HasMaxLength(500);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(1000);

            // Relationships
            builder.HasMany(c => c.Movies)
                .WithOne(m => m.Cinema)
                .HasForeignKey(m => m.CinemaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
