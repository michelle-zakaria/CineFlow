using CineFlow.DataAccess.Extensions;
using System.Data;
using System.Threading.Tasks;

namespace CineFlow.Areas.Admin.Data.Services
{
    public class MovieCategoryService : EntityBaseRepository<MovieCategory>, IMovieCategoryService
    {
        private readonly AppDbContext _context;
        public MovieCategoryService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Movie>> GetMoviesByCategoryId(int categoryId)
        {
            // use in MovieCategoryController Details method
            var movies = await _context.Movies
                .Where(m => m.MovieCategoryId == categoryId)
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
        public async Task<PaginatedList<MovieCategory>> SearchItemPaginatedAsync(string searchString, int pageIndex, int pageSize)
        {
            // Use in MovieCategoryController Index method where searching is needed
            return await SearchPaginatedAsync(m => EF.Functions.Like(m.Name, $"%{searchString}%") || 
                                                   EF.Functions.Like(m.Description!, $"%{searchString}%"), 
                                                   pageIndex, pageSize);
        }
    }
}
