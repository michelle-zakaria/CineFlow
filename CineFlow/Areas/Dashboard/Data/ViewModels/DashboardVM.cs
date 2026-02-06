namespace CineFlow.Areas.Dashboard.Data.ViewModels
{
    public class DashboardVM
    {
        // Summary Stats
        public int TotalMovies { get; set; }
        public int TotalAvailableMovies { get; set; }
        public int TotalUpcomingMovies { get; set; }
        public int TotalActors { get; set; }
        public int TotalCinemas { get; set; }

        // Revenue Stats
        public decimal TotalRevenue { get; set; }
        public decimal ThisWeekRevenue { get; set; }
        public decimal LastWeekRevenue { get; set; }
        public decimal RevenueGrowth { get; set; }

        // Booking Stats
        public int TotalBookings { get; set; }
        public int ThisWeekBookings { get; set; }
        public int BookingGrowth { get; set; }

        // Movie Categories Stats
        public List<CategoryStat> CategoryStats { get; set; } = new List<CategoryStat>();

        // Recent Movies
        public List<Movie> RecentMovies { get; set; } = new List<Movie>();

        // Top Rated Movies
        public List<MovieRating> TopRatedMovies { get; set; } = new List<MovieRating>();

        // Weekly Revenue Chart Data
        public List<WeeklyRevenue> WeeklyRevenues { get; set; } = new List<WeeklyRevenue>();

        // Most Popular Movies (by bookings)
        public List<PopularMovie> PopularMovies { get; set; } = new List<PopularMovie>();

        // Recently Added Actors
        public List<Actor> RecentActors { get; set; } = new List<Actor>();
    }
}
