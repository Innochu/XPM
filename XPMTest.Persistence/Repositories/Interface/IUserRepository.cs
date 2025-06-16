
namespace XPMTest.Persistence.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<Customer?> GetByIdAsync(string id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> AddAsync(Customer user);
        Task<bool> SaveChangesAsync();
        Task<Customer?> FindAsync(Expression<Func<Customer, bool>> predicate);
        Task AddAppUserAsync(AppUser appUser);
    }
}
