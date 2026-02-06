namespace CineFlow.Areas.Admin.Data.Interfaces
{
    public interface ICinemaService : IEntityBaseRepository<Cinema>
    {
        Task<List<Movie>> GetMoviesByCinemaId(int cinemaId);
        Task<PaginatedList<Cinema>> SearchItemPaginatedAsync(string searchString, int pageIndex, int pageSize);
    }
}
