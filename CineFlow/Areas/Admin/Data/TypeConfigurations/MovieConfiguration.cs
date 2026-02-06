namespace CineFlow.Areas.Admin.Data.TypeConfigurations
{
    public class MovieConfiguration : BaseEntityConfiguration<Movie>
    {
        protected override string TableName => "Movies";
        protected override string? Schema => "admin";

        protected override void ConfigureEntity(EntityTypeBuilder<Movie> builder)
        {
            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(m => m.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(m => m.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(m => m.ImageURL)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(m => m.StartDate)
                .IsRequired();

            builder.Property(m => m.EndDate)
                .IsRequired();

            // Foreign key relationships
            builder.HasOne(m => m.MovieCategory)
                .WithMany(mc => mc.Movies)
                .HasForeignKey(m => m.MovieCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Cinema)
                .WithMany(c => c.Movies)
                .HasForeignKey(m => m.CinemaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-many relationship through Actor_Movie
            builder.HasMany(m => m.Actors_Movies)
                .WithOne(am => am.Movie)
                .HasForeignKey(am => am.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many relationship with SubImages
            builder.HasMany(m => m.SubImages)
                .WithOne(si => si.Movie)
                .HasForeignKey(si => si.MovieId)
                .OnDelete(DeleteBehavior.Cascade); // If movie is deleted, sub-images are also deleted

            // Indexes
            builder.HasIndex(m => m.Title);
            builder.HasIndex(m => m.Price);
            builder.HasIndex(m => m.StartDate);
            builder.HasIndex(m => m.EndDate);
            builder.HasIndex(m => m.MovieCategoryId);
            builder.HasIndex(m => m.CinemaId);
        }
    }
}
