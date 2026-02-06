namespace CineFlow.Areas.Admin.Data.TypeConfigurations
{
    public class MovieCategoryConfiguration : BaseEntityConfiguration<MovieCategory>
    {
        protected override string TableName => "MovieCategories";
        protected override string? Schema => "admin";

        protected override void ConfigureEntity(EntityTypeBuilder<MovieCategory> builder)
        {
            builder.Property(mc => mc.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(mc => mc.Description)
                .IsRequired()
                .HasMaxLength(1000);

            // Relationships
            builder.HasMany(mc => mc.Movies)
                .WithOne(m => m.MovieCategory)
                .HasForeignKey(m => m.MovieCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
