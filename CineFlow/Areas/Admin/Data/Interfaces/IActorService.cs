namespace CineFlow.Areas.Admin.Data.Interfaces
{
    public interface IActorService : IEntityBaseRepository<Actor>
    {
        Task<List<Movie>> GetMoviesByActorId(int actorId);
        Task<PaginatedList<Actor>> SearchItemPaginatedAsync(string searchString, int pageIndex, int pageSize);
    }
}
