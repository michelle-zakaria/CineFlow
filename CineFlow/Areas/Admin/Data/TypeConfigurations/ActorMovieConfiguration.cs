namespace CineFlow.Areas.Admin.Data.TypeConfigurations
{
    public class ActorMovieConfiguration : IEntityTypeConfiguration<Actor_Movie>
    {
        public void Configure(EntityTypeBuilder<Actor_Movie> builder)
        {
            builder.ToTable("ActorMovies");

            // Composite primary key
            builder.HasKey(am => new { am.ActorId, am.MovieId });

            // Foreign keys
            builder.HasOne(am => am.Actor)
                .WithMany(a => a.Actors_Movies)
                .HasForeignKey(am => am.ActorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(am => am.Movie)
                .WithMany(m => m.Actors_Movies)
                .HasForeignKey(am => am.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
