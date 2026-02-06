namespace CineFlow.Areas.Admin.Data.Services
{
    public class BookingService : EntityBaseRepository<Booking>, IBookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
