namespace CineFlow.DataAccess.Base
{
    public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : class, IEntityBase
    {
        protected abstract string TableName { get; }
        protected abstract string? Schema { get; }

        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.ToTable(TableName, Schema);

            // Common configuration for all IEntityBase entities
            builder.HasKey(e => e.Id);

            // Optional: Configure Id as identity column if using SQL Server
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            ConfigureEntity(builder);
        }

        protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
    }
}
