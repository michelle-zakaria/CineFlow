namespace CineFlow.Areas.Admin.Data.TypeConfigurations
{
    public class ActorConfiguration : BaseEntityConfiguration<Actor>
    {
        protected override string TableName => "Actors";
        protected override string? Schema => "admin";

        protected override void ConfigureEntity(EntityTypeBuilder<Actor> builder)
        {
            builder.Property(a => a.FullName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.Bio)
                .HasMaxLength(2000);

            builder.Property(a => a.ProfilePictureURL)
                .IsRequired()
                .HasMaxLength(500);

            // Relationships
            builder.HasMany(a => a.Actors_Movies)
                .WithOne(am => am.Actor)
                .HasForeignKey(am => am.ActorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
