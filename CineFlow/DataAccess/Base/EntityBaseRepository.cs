using System.Data;

namespace CineFlow.DataAccess.Base
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        private readonly AppDbContext _context;

        public EntityBaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, T entity)
        {
            EntityEntry entityEntry = _context.Entry<T>(entity);
            entityEntry.State = EntityState.Modified;

            await SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            EntityEntry entityEntry = _context.Entry<T>(entity);
            entityEntry.State = EntityState.Deleted;
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }   

        //=======================================================================================================
        // Get
        //=====
        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        // Original method (with tracking - for Edit operations)
        public async Task<T> GetByIdAsync(int id)
            => await _context.Set<T>().FirstOrDefaultAsync(n => n.Id == id);

        // New method (without tracking - for read-only operations)
        public async Task<T> GetByIdNoTrackingAsync(int id)
            => await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);

        // You can also add an optional parameter
        public async Task<T> GetByIdAsync(int id, bool trackChanges = true)
        {
            var query = _context.Set<T>().AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await query.FirstOrDefaultAsync(n => n.Id == id);
        }

        //=======================================================================================================
        // Search methods
        //================
        // Role-based search pagination

        public async Task<PaginatedList<T>> GetPaginatedAsync(int pageIndex, int pageSize)
        {
            return await PaginatedList<T>.CreateAsync(_context.Set<T>().AsQueryable(), pageIndex, pageSize);
        }

        public async Task<PaginatedList<T>> GetPaginatedAsync(int pageIndex, int pageSize, 
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            //query = ApplyRoleFilter(query, role);
            return await PaginatedList<T>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<PaginatedList<T>> SearchPaginatedAsync(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize)
        {
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            //query = ApplyRoleFilter(query, role);
            return await PaginatedList<T>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<PaginatedList<T>> SearchPaginatedAsync(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, 
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            //query = ApplyRoleFilter(query, role);
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await PaginatedList<T>.CreateAsync(query, pageIndex, pageSize);
        }

       
    }
}
