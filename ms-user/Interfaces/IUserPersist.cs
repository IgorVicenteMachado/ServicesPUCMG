using ms_user.Models;

namespace ms_user.Interfaces
{
    public interface IUserPersist
    {
        Task Add(User user);
        void Update(User user);
        void Delete(User user);
        Task<bool> SaveChangesAsync();
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> GetUserByNameAsync(string userName);
        Task<User> GetUserByEmailAsync(string email);
    }
}
