using CineFlow.Areas.Admin.Data.Interfaces;

namespace CineFlow.Areas.Dashboard.Controllers
{
    [Area(SD.DASH_AREA)]
    public class DashboardController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IActorService _actorService;
        private readonly ICinemaService _cinemaService;
        private readonly IMovieCategoryService _categoryService;
        private readonly IBookingService _bookingService;

        public DashboardController(IMovieService movieService, 
            IActorService actorService,
            ICinemaService cinemaService, 
            IMovieCategoryService categoryService,
            IBookingService bookingService)
        {
            _movieService = movieService;
            _actorService = actorService;
            _cinemaService = cinemaService;
            _categoryService = categoryService;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.GetAllAsync();
            var bookings = await _bookingService.GetAllAsync();
            var actors = await _actorService.GetAllAsync();
            var cinemas = await _cinemaService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();

            var currentDate = DateTime.Now;
            var oneWeekAgo = currentDate.AddDays(-7);

            // Prepare dashboard data
            var model = new DashboardVM
            {
                // Basic Stats
                TotalMovies = movies.Count(),
                TotalAvailableMovies = movies.Count(m => m.StartDate <= currentDate && m.EndDate >= currentDate),
                TotalUpcomingMovies = movies.Count(m => m.StartDate > currentDate),
                TotalActors = actors.Count(),
                TotalCinemas = cinemas.Count(),

                // Revenue Calculations
                TotalRevenue = bookings.Sum(b => b.TotalAmount),
                ThisWeekRevenue = bookings
                    .Where(b => b.BookingDate >= oneWeekAgo)
                    .Sum(b => b.TotalAmount),
                LastWeekRevenue = bookings
                    .Where(b => b.BookingDate >= oneWeekAgo.AddDays(-7) && b.BookingDate < oneWeekAgo)
                    .Sum(b => b.TotalAmount),

                // Booking Stats
                TotalBookings = bookings.Count(),
                ThisWeekBookings = bookings.Count(b => b.BookingDate >= oneWeekAgo),

                // Category Stats
                CategoryStats = categories
                    .Select(c => new CategoryStat
                    {
                        CategoryName = c.Name,
                        MovieCount = movies.Count(m => m.MovieCategoryId == c.Id),
                        Percentage = movies.Any() ?
                            (decimal)movies.Count(m => m.MovieCategoryId == c.Id) / movies.Count() * 100 : 0,
                        Color = GetCategoryColor(c.Name)
                    })
                    .Where(cs => cs.MovieCount > 0)
                    .ToList(),

                // Recent Movies (last 5 added)
                RecentMovies = movies
                    .OrderByDescending(m => m.Id)
                    .Take(5)
                    .ToList(),

                // Sample Rating Data (in real app, get from reviews table)
                TopRatedMovies = movies
                    .Take(5)
                    .Select(m => new MovieRating
                    {
                        Movie = m,
                        Rating = new Random().NextDouble() * 2 + 8, // 8.0-10.0
                        Reviews = new Random().Next(50, 500)
                    })
                    .OrderByDescending(mr => mr.Rating)
                    .ToList(),

                // Weekly Revenue Data (sample)
                WeeklyRevenues = GetSampleWeeklyRevenue(),

                // Popular Movies (sample)
                PopularMovies = movies
                    .Take(5)
                    .Select(m => new PopularMovie
                    {
                        Movie = m,
                        Bookings = new Random().Next(100, 1000),
                        Revenue = new Random().Next(1000, 10000)
                    })
                    .OrderByDescending(pm => pm.Bookings)
                    .ToList(),

                // Recent Actors
                RecentActors = actors
                    .OrderByDescending(a => a.Id)
                    .Take(5)
                    .ToList()
            };

            // Calculate growth percentages
            model.RevenueGrowth = model.LastWeekRevenue > 0 ?
                ((model.ThisWeekRevenue - model.LastWeekRevenue) / model.LastWeekRevenue) * 100 : 100;

            model.BookingGrowth = bookings.Any(b => b.BookingDate < oneWeekAgo) ? 15 : 100; // Sample growth

            return View(model);
        }

        private List<WeeklyRevenue> GetSampleWeeklyRevenue()
        {
            var days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            var random = new Random();

            return days.Select(day => new WeeklyRevenue
            {
                Day = day,
                Revenue = random.Next(1000, 5000)
            }).ToList();
        }

        private string GetCategoryColor(string categoryName)
        {
            return categoryName.ToLower() switch
            {
                "action" => "primary",
                "comedy" => "warning",
                "drama" => "info",
                "horror" => "danger",
                "romance" => "pink",
                "sci-fi" => "success",
                "thriller" => "purple",
                _ => "secondary"
            };
        }
    }
}
