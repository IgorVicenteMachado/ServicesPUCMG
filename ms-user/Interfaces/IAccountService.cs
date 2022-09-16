using Microsoft.AspNetCore.Identity;
using ms_user.Models;
using ms_user.ViewModel.Accounts;

namespace ms_user.Interfaces
{
    public interface IAccountService
    {
        Task<bool> UserExists(string email);
        Task<User> GetUserByNameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task<SignInResult> CheckUserPasswordAsync(User usuario, string password);
        Task<User> CreateAccountAsync(User usuario);
        Task<User> UpdateAccount(User usuario);
    }
}
