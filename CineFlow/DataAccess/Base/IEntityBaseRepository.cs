namespace CineFlow.DataAccess.Base
{
    public interface IEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        // Default
        //=========
        Task AddAsync(T entity);
        Task UpdateAsync(int id, T entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();

        //===============================================
        // Get
        //=====
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdNoTrackingAsync(int id);
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync();

        // Pagination methods
        Task<PaginatedList<T>> GetPaginatedAsync(int pageIndex, int pageSize);

        //===============================================
        // Search methods
        //================

        // Pagination methods
        Task<PaginatedList<T>> GetPaginatedAsync(int pageIndex, int pageSize, params Expression<Func<T, object>>[] includeProperties);
        Task<PaginatedList<T>> SearchPaginatedAsync(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize);
        Task<PaginatedList<T>> SearchPaginatedAsync(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize,
            params Expression<Func<T, object>>[] includeProperties);
    }
}
