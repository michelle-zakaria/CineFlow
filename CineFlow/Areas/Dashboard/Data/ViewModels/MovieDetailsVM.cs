namespace CineFlow.Areas.Dashboard.Data.ViewModels
{
    public class MovieDetailsVM
    {
        public Movie Movie { get; set; }
        public List<Movie> SimilarMovies { get; set; } = new List<Movie>();
    }
}
