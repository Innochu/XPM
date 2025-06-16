

namespace XPMTest.Persistence.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(string id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer> AddAsync(Customer user)
        {
            await _context.Customers.AddAsync(user);
            await _context.SaveChangesAsync(); 
            return user; 
        }

        public async Task AddAppUserAsync(AppUser appUser)
        {
            await _context.Users.AddAsync(appUser);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<Customer?> FindAsync(Expression<Func<Customer, bool>> predicate)
        {
            return _context.Customers.FirstOrDefaultAsync(predicate);
        }
    }
}