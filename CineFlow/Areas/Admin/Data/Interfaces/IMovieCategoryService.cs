namespace CineFlow.Areas.Admin.Data.Interfaces
{
    public interface IMovieCategoryService : IEntityBaseRepository<MovieCategory>
    {
        Task<List<Movie>> GetMoviesByCategoryId(int categoryId);
        Task<PaginatedList<MovieCategory>> SearchItemPaginatedAsync(string searchString, int pageIndex, int pageSize);
    }
}
