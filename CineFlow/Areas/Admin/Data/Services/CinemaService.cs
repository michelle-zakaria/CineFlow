namespace CineFlow.Areas.Admin.Data.Services
{
    public class CinemaService : EntityBaseRepository<Cinema>, ICinemaService
    {
        private readonly AppDbContext _context;
        public CinemaService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Movie>> GetMoviesByCinemaId(int cinemaId)
        {
            // use in CinemaController Details method
            var movies = await _context.Movies
                .Where(m => m.CinemaId == cinemaId)
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
                    // Include related entities
                    MovieCategory = new MovieCategory
                    {
                        Id = m.MovieCategory.Id,
                        Name = m.MovieCategory.Name
                    },
                    Cinema = new Cinema
                    {
                        Id = m.Cinema.Id,
                        Name = m.Cinema.Name,
                        Logo = m.Cinema.Logo
                    },
                    // Include actors (optional)
                    Actors_Movies = m.Actors_Movies
                        .Take(3) // Limit to first 3 actors
                        .Select(am => new Actor_Movie
                        {
                            Actor = new Actor
                            {
                                Id = am.Actor.Id,
                                FullName = am.Actor.FullName,
                                ProfilePictureURL = am.Actor.ProfilePictureURL
                            }
                        })
                        .ToList()
                })
                .OrderByDescending(m => m.StartDate)
                .ToListAsync();
            return movies;
        }

        public async Task<PaginatedList<Cinema>> SearchItemPaginatedAsync(string searchString, int pageIndex, int pageSize)
        {
            return await SearchPaginatedAsync(c => EF.Functions.Like(c.Name, $"%{searchString}%") ||
                                                   EF.Functions.Like(c.Description!, $"%{searchString}%"),
                                                   pageIndex, pageSize);
        }
    }
}
