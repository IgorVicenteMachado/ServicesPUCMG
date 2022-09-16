using Microsoft.EntityFrameworkCore;
using ms_user.Interfaces;
using ms_user.Models;

namespace ms_user.Data
{
    public class UserPersist : IUserPersist
    {
        private readonly DataContext _context;

        public UserPersist(DataContext context)
        {
            _context = context;
        }
        public async Task Add(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByNameAsync(string userName)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.FirstName == userName.ToLower());
        }  
        
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public void Update(User user)
        {
            _context.Users.Update(user); 
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
