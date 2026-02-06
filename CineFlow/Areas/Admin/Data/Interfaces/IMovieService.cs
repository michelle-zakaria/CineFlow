namespace CineFlow.Areas.Admin.Data.Interfaces
{
    public interface IMovieService : IEntityBaseRepository<Movie>
    {
        Task<Movie> GetMovieByIdAsync(int id);
        Task<NewMovieDropdownsVM> GetNewMovieDropdownsValues();
        Task AddNewMovieAsync(MovieVM data);
        Task UpdateMovieAsync(MovieVM data);
        Task<MovieVM> GetMovieForEditAsync(int id);
        Task<Movie> GetMovieForEditWithTrackingAsync(int id);
        Task UpdateMovieActorsAsync(int movieId, List<int>? actorIds);
        Task<PaginatedList<Movie>> SearchItemPaginatedAsync(string searchString, int pageIndex, int pageSize);
    }
}
