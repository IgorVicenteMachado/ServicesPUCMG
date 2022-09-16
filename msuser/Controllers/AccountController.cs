using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using msuser.Data;
using msuser.Extensions;
using msuser.Models;
using msuser.Services;
using msuser.ViewModel;
using msuser.ViewModel.Accounts;
using System.Security.Claims;

namespace msuser.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly TokenService _tokenService;
        private readonly IMemoryCache _cache;

        public AccountController(DataContext context, UserManager<User> userManager, SignInManager<User> signInManager, TokenService tokenService, IMemoryCache cache)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _cache = cache;
        }

        [Authorize]
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return Ok(new ResultVM<User>(user));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResultVM<string>($"Erro ao tentar recuperar Usuário. Erro: {ex.Message}"));
            }
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserVM userVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultVM<string>(ModelState.GetErrors()));

            try
            {
                if (await _userManager.Users.AsNoTracking().AnyAsync(u => u.Email == userVM.Email))
                    return BadRequest(new ResultVM<string>("Usuário já existe"));

                var role = GetRolesFromCache().First(x => x.NormalizedName == userVM.Profile.ToString().ToUpper());

                var user = CreateUser(userVM, role);

                var result = await _userManager.CreateAsync(user, user.PasswordHash);

                if (result.Succeeded)
                    return Ok(new ResultVM<dynamic>(new
                    {
                        token = _tokenService.CreateToken(user).Result
                    }));

                return BadRequest(new ResultVM<string>(result.GetErrors()));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                  new ResultVM<string>($"Erro ao tentar Registrar Usuário. Erro: {ex.Message}"));
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM userLogin)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultVM<string>(ModelState.GetErrors()));

            try
            {
                var user = await _userManager.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email == userLogin.Email);
                if (user == null)
                    return StatusCode(401, new ResultVM<string>("Usuário ou senha inválidos"));

                var result = await _signInManager.CheckPasswordSignInAsync(user, userLogin.Password, false);

                if (!result.Succeeded)
                    return StatusCode(401, new ResultVM<string>("Usuário ou senha inválidos"));

                return Ok(new ResultVM<dynamic>(new { token = _tokenService.CreateToken(user).Result }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResultVM<string>($"Erro ao tentar realizar Login. Erro: {ex.Message}"));
            }
        }

        [Authorize]
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserVM userVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultVM<string>(ModelState.GetErrors()));

            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var user = await _userManager.Users.Include(x => x.Roles).FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                    return BadRequest(new ResultVM<string>("Usuário Inválido"));

                if (userVM.PasswordHash != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, userVM.PasswordHash);
                }

                _context.UserRoles.RemoveRange(user.Roles);
                var role = GetRolesFromCache().First(x => x.NormalizedName == userVM.Profile.ToString().ToUpper());
                user = UpdateUser(user, userVM, role);
                await _userManager.UpdateAsync(user);

                return Ok(new
                {
                    token = _tokenService.CreateToken(user).Result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                     new ResultVM<string>($"Erro ao tentar Atualizar Usuário. Erro: Erro: {ex.Message}"));
            }
        }
        private List<Role> GetRolesFromCache()
        {
            try
            {
                var categories = _cache.GetOrCreate("RolesCache", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return _context.Roles.ToList();
                });

                return categories;
            }
            catch
            {
                return null;
            }
        }
        private static User CreateUser(UserVM userVM, Role role)
        {
            var user = new User();
            user.Id = Guid.NewGuid();
            user.Roles = new List<UserRole>() { new UserRole() { RoleId = role.Id, UserId = user.Id } };
            user.UserName = userVM.Email;
            user.PasswordHash = userVM.PasswordHash;
            user.Email = userVM.Email;
            user.FirstName = userVM.FirstName;
            user.LastName = userVM.LastName;
            user.Profile = userVM.Profile;
            return user;
        }

        private static User UpdateUser(User user, UserVM userVM, Role role)
        {
            user.Roles = new List<UserRole>() { new UserRole() { RoleId = role.Id, UserId = user.Id } };
            user.UserName = userVM.Email;
            user.Email = userVM.Email;
            user.FirstName = userVM.FirstName;
            user.LastName = userVM.LastName;
            user.Profile = userVM.Profile;
            return user;
        }
    }
}


