namespace CineFlow.Areas.Admin.Data.Services
{
    public class ActorService : EntityBaseRepository<Actor>, IActorService
    {
        private readonly AppDbContext _context;
        public ActorService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Movie>> GetMoviesByActorId(int actorId)
        {
            // use in ActorController Details method
            var movies = await _context.Movies
                .Where(m => m.Actors_Movies.Any(am => am.ActorId == actorId))
                .Select(m => new Movie
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    Price = m.Price,
                    ImageURL = m.ImageURL,
                    SubImages = m.SubImages,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,

                    // Include related entities if needed
                    Cinema = new Cinema { Id = m.Cinema.Id, Name = m.Cinema.Name },
                    MovieCategory = new MovieCategory { Id = m.MovieCategory.Id, Name = m.MovieCategory.Name },

                    // Include actors if needed (optional)
                    Actors_Movies = m.Actors_Movies
                        .Select(am => new Actor_Movie
                        {
                            Actor = new Actor { Id = am.Actor.Id, FullName = am.Actor.FullName }
                        })
                        .ToList()
                })
                .ToListAsync();

            return movies;
        }
        public async Task<PaginatedList<Actor>> SearchItemPaginatedAsync(string searchString, int pageIndex, int pageSize)
        {
            return await SearchPaginatedAsync(a => EF.Functions.Like(a.FullName, $"%{searchString}%") ||
                                                   EF.Functions.Like(a.Bio!, $"%{searchString}%"), 
                                                   pageIndex, pageSize);
        }
    }
}
