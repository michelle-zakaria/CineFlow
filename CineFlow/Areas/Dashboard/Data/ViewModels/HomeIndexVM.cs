namespace CineFlow.Areas.Dashboard.Data.ViewModels
{
    public class HomeIndexVM
    {
        public List<Movie> AvailableMovies { get; set; } = new List<Movie>();
        public List<Movie> UpcomingMovies { get; set; } = new List<Movie>();
        public List<Cinema> Cinemas { get; set; } = new List<Cinema>();
        public List<MovieCategory> Categories { get; set; } = new List<MovieCategory>();
        public string SearchString { get; set; }
        public int? CategoryId { get; set; }
        public int? CinemaId { get; set; }

        // SelectLists for dropdowns
        public SelectList CinemaSelectList { get; set; }
        public SelectList CategorySelectList { get; set; }
    }
}
