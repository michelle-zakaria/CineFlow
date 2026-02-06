namespace CineFlow.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // This one line does it all - automatically finds and applies all configurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            /*
            Note: ApplyConfigurationsFromAssembly() automatically finds and applies all classes that implement
            IEntityTypeConfiguration < TEntity > in the specified assembly, so you don't need to manually
            */

            // If you have additional fluent configurations:
            
        }

        public DbSet<MovieCategory> MovieCategories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor_Movie> Actors_Movies { get; set; }
        public DbSet<MovieSubImage> MovieSubImages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingsSeat { get; set; }

    }
}
