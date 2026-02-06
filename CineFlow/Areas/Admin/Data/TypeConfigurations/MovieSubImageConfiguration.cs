namespace CineFlow.Areas.Admin.Data.TypeConfigurations
{
    public class MovieSubImageConfiguration : BaseEntityConfiguration<MovieSubImage>
    {
        protected override string TableName => "MovieSubImages";
        protected override string? Schema => "admin";

        protected override void ConfigureEntity(EntityTypeBuilder<MovieSubImage> builder)
        {
            // Properties
            builder.Property(si => si.ImageURL)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(si => si.Caption)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(si => si.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);

            // Foreign key relationship with Movie
            builder.HasOne(si => si.Movie)
                .WithMany(m => m.SubImages)
                .HasForeignKey(si => si.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(si => si.MovieId);
            builder.HasIndex(si => si.DisplayOrder);

            // Composite index if needed
            builder.HasIndex(si => new { si.MovieId, si.DisplayOrder });
        }
    }
}
