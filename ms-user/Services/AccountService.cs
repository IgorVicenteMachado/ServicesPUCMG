using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ms_user.Interfaces;
using ms_user.Models;

namespace ms_user.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserPersist _userPersist;

        public AccountService(UserManager<User> userManager,
                              SignInManager<User> signInManager,
       
                              IUserPersist userPersist)
        {
            _userManager = userManager;
            _signInManager = signInManager;
     
            _userPersist = userPersist;
        }
        public async Task<bool> UserExists(string email)
        {
            try
            {
                return await _userManager.Users.AnyAsync(user => user.Email  == email);
            }
            catch (System.Exception ex)
            {
                throw new Exception($"Erro ao verificar se usuário existe. Erro: {ex.Message}");
            }
        }

        public async Task<SignInResult> CheckUserPasswordAsync(User usuario, string password)
        {
            try
            {
                var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == usuario.FirstName.ToLower());

                return await _signInManager.CheckPasswordSignInAsync(user, password, false);
            }
            catch (System.Exception ex)
            {
                throw new Exception($"Erro ao tentar verificar password. Erro: {ex.Message}");
            }
        }

        public async Task<User> CreateAccountAsync(User usuario)
        {
            try
            {
                var result = await _userManager.CreateAsync(usuario, usuario.PasswordHash);

                if (result.Succeeded)
                {
                    return usuario;
                }

                return null;
            }
            catch (System.Exception ex)
            {
                throw new Exception($"Erro ao tentar Criar Usuário. Erro: {ex.Message}");
            }
        }

        public async Task<User> GetUserByNameAsync(string userName)
        {
            try
            {
                var user = await _userPersist.GetUserByNameAsync(userName);
                if (user == null) return null;

                return user;
            }
            catch (System.Exception ex)
            {
                throw new Exception($"Erro ao tentar pegar Usuário por Username. Erro: {ex.Message}");
            }
        }    
        
        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userPersist.GetUserByEmailAsync(email);
                if (user == null) return null;

                return user;
            }
            catch (System.Exception ex)
            {
                throw new Exception($"Erro ao tentar pegar Usuário por Username. Erro: {ex.Message}");
            }
        }

        public async Task<User> UpdateAccount(User usuario)
        {
            try
            {
                var user = await _userPersist.GetUserByNameAsync(usuario.UserName);
                if (user == null) return null;

                usuario.Id = user.Id;

                if (usuario.PasswordHash != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, usuario.PasswordHash);
                }

                _userPersist.Update(user);

                if (await _userPersist.SaveChangesAsync())
                   return await _userPersist.GetUserByNameAsync(user.UserName);

                return null;
            }
            catch (System.Exception ex)
            {
                throw new Exception($"Erro ao tentar atualizar usuário. Erro: {ex.Message}");
            }
        }

   
    }
}
