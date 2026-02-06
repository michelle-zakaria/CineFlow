namespace CineFlow.Areas.Dashboard.Controllers
{
    [Area(SD.DASH_AREA)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;
        private readonly ICinemaService _cinemaService;
        private readonly IMovieCategoryService _categoryService;

        public HomeController(ILogger<HomeController> logger, 
            IMovieService movieService, 
            ICinemaService cinemaService, 
            IMovieCategoryService categoryService)
        {
            _logger = logger;
            _movieService = movieService;
            _cinemaService = cinemaService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string searchString = null, int? categoryId = null, int? cinemaId = null)
        {
            var currentDate = DateTime.Now;
            var allMovies = await _movieService.GetAllAsync();

            // Get available movies
            var availableMovies = allMovies
                .Where(m => m.StartDate <= currentDate && m.EndDate >= currentDate)
                .ToList();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                availableMovies = availableMovies
                    .Where(m => m.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                               m.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Apply category filter
            if (categoryId.HasValue)
            {
                availableMovies = availableMovies
                    .Where(m => m.MovieCategoryId == categoryId.Value)
                    .ToList();
            }

            // Apply cinema filter
            if (cinemaId.HasValue)
            {
                availableMovies = availableMovies
                    .Where(m => m.CinemaId == cinemaId.Value)
                    .ToList();
            }

            // Get upcoming movies
            var upcomingMovies = allMovies
                .Where(m => m.StartDate > DateTime.Now)
                .OrderBy(m => m.StartDate)
                .Take(5)
                .ToList();

            // Get all cinemas and categories
            var cinemas = await _cinemaService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();

            // Create SelectLists with selected values
            var cinemaSelectList = new SelectList(cinemas, "Id", "Name", cinemaId);
            var categorySelectList = new SelectList(categories, "Id", "Name", categoryId);

            // Create view model
            var model = new HomeIndexVM
            {
                AvailableMovies = availableMovies,
                UpcomingMovies = upcomingMovies,
                Cinemas = cinemas.ToList(),
                Categories = categories.ToList(),
                SearchString = searchString,
                CategoryId = categoryId,
                CinemaId = cinemaId,
                CinemaSelectList = cinemaSelectList,
                CategorySelectList = categorySelectList
            };

            return View(model);
        }

        public async Task<IActionResult> MovieDetails(int id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);

            if (movie == null)
                return NotFound();

            // Get similar movies
            var allMovies = await _movieService.GetAllAsync();
            var similarMovies = allMovies
                .Where(m => m.Id != id &&
                       m.MovieCategoryId == movie.MovieCategoryId &&
                       m.StartDate <= DateTime.Now &&
                       m.EndDate >= DateTime.Now)
                .Take(3)
                .ToList();

            // Create view model
            var model = new MovieDetailsVM
            {
                Movie = movie,
                SimilarMovies = similarMovies
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
