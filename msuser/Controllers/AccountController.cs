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
                var user = _userManager.Users.FirstOrDefault(u => u.Email == email);

                return Ok(new ResultVM<dynamic>(
                    new { Id = user.Id, Email = user.Email, PrimeiroNome = user.FirstName, UltimoNome = user.LastName, Perfil = user.Profile}, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultVM<string>( $"Erro ao tentar recuperar Usuário. Erro: {ex.Message}"));
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

                var roles = GetRolesFromCache();
                var userRole = roles.First(r => r.Name == userVM.Perfil.ToString().ToLower());

                var user = new User() { Id = Guid.NewGuid() };
                MaptoUser(userVM, user, userRole.Id);

                var result = await _userManager.CreateAsync(user, user.PasswordHash);

                if (result.Succeeded)
                    return Ok(new ResultVM<dynamic>(new { UserId = user.Id, Token = _tokenService.CreateToken(user).Result }));

                return BadRequest(new ResultVM<string>(result.GetErrors()));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultVM<string>($"Erro ao tentar Registrar Usuário. Erro: {ex.Message}"));
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

                var result = await _signInManager.CheckPasswordSignInAsync(user, userLogin.Senha, false);

                if (!result.Succeeded)
                    return StatusCode(401, new ResultVM<string>("Usuário ou senha inválidos"));

                return Ok(new ResultVM<dynamic>(new { UserId = user.Id, Token = _tokenService.CreateToken(user).Result }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultVM<string>($"Erro ao tentar realizar Login. Erro: {ex.Message}"));
            }
        }

        [Authorize]
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser( UserVM userVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultVM<string>(ModelState.GetErrors()));

            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var user = _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.Email == email);
                if (user == null)
                    return BadRequest(new ResultVM<string>("Usuário Inválido"));

                var roles = GetRolesFromCache();
                var userRole = roles.First(r => r.Name == userVM.Perfil.ToString().ToLower());

                _context.UserRoles.RemoveRange(user.Roles);
                MaptoUser(userVM, user, userRole.Id);

                if (userVM.Senha != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var reset = await _userManager.ResetPasswordAsync(user, token, userVM.Senha);
                    if (!reset.Succeeded)
                        return BadRequest(new ResultVM<string>(reset.GetErrors()));
                };

                var update = await _userManager.UpdateAsync(user);
                if (update.Succeeded)
                    return Ok(new ResultVM<dynamic>(new { UserId = user.Id, Token = _tokenService.CreateToken(user).Result }));

                return BadRequest(new ResultVM<string>(update.GetErrors()));
            }
            catch (Exception ex)
            {
                return StatusCode(500,  new ResultVM<string>($"Erro ao tentar Atualizar Usuário. Erro: Erro: {ex.Message}"));
            }
        }

        private List<Role> GetRolesFromCache()
        {
            return _cache.GetOrCreate("RolesCache", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return _context.Roles.ToList();
            });
        }

        private static void MaptoUser(UserVM userVM, User? user, Guid roleId)
        {
            user.UserName = userVM.Email;
            user.Email = userVM.Email;
            user.PasswordHash = userVM.Senha;
            user.FirstName = userVM.PrimeiroNome;
            user.LastName = userVM.UltimoNome;
            user.Profile = userVM.Perfil;
            user.NormalizedEmail = userVM.Email.ToUpper();
            user.NormalizedUserName = userVM.Email.ToUpper();
            user.Roles = new List<UserRole>() { new UserRole() { RoleId = roleId, UserId = user.Id } };
        }
    }
}