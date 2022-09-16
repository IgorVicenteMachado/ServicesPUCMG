using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ms_user.Extensions;
using ms_user.Interfaces;
using ms_user.Models;
using ms_user.ViewModel.Accounts;

namespace ms_user.Controllers
{
 
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public AccountController(IAccountService accountService,
                                 ITokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }

        [Authorize]
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var userName = User.GetUserName();
                var user = await _accountService.GetUserByNameAsync(userName);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuperar Usuário. Erro: {ex.Message}");
            }
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(User userx)
        {
            try
            {
                if (await _accountService.UserExists(userx.Email))
                    return BadRequest("Usuário já existe");

                var user = await _accountService.CreateAccountAsync(userx);
                if (user != null)
                    return Ok(new
                    {
                        userName = user.UserName,
                        PrimeroNome = user.FirstName,
                        token = _tokenService.CreateToken(user).Result
                    });

                return BadRequest("Usuário não criado, tente novamente mais tarde!");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar Registrar Usuário. Erro: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel userLogin)
        {
            try
            {
                var user = await _accountService.GetUserByNameAsync(userLogin.Email);
                if (user == null) return Unauthorized("Usuário ou Senha inválidos!");

                var result = await _accountService.CheckUserPasswordAsync(user, userLogin.Password);
                if (!result.Succeeded) return Unauthorized();

                return Ok(new
                {
                    userName = user.UserName,
                    PrimeroNome = user.FirstName,
                    token = _tokenService.CreateToken(user).Result
                });
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar realizar Login. Erro: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(User userUpdateDto)
        {
            try
            {
                if (userUpdateDto.UserName != User.GetUserName())
                    return Unauthorized("Usuário Inválido");

                var user = await _accountService.GetUserByNameAsync(User.GetUserName());
                if (user == null) return Unauthorized("Usuário Inválido");

                var userReturn = await _accountService.UpdateAccount(userUpdateDto);
                if (userReturn == null) return NoContent();

                return Ok(new
                {
                    userName = userReturn.UserName,
                    PrimeroNome = userReturn.FirstName,
                    token = _tokenService.CreateToken(userReturn).Result
                });
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar Atualizar Usuário. Erro: {ex.Message}");
            }
        }
    }
}

//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ms_user.Data;
//using ms_user.Extensions;
//using ms_user.Models;
//using ms_user.Services;
//using ms_user.ViewModel;
//using ms_user.ViewModel.Accounts;
//using SecureIdentity.Password;

//namespace ms_user.Controllers
//{
//    [Route("v1/accounts")]
//    [ApiController]
//    public class AccountController : ControllerBase
//    {
//        //faz o cadastro do usuário no sistema
//        [HttpPost("register")]
//        public async Task<IActionResult> Post(
//            [FromServices] DataContext context,
//            [FromServices] EmailService emailService,
//            [FromBody] RegisterViewModel model )
//        {
//            if (!ModelState.IsValid) 
//                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

//            var role = context.Roles.Find((int)model.Perfil);
//            if(role == null)
//                return StatusCode(400, new ResultViewModel<string>("Role não encontrada no banco de dados"));

//            var user = new User
//            {
//                Name = model.Name,
//                Email = model.Email,
//                Roles = new List<Role>() { role }   
//            };

//            user.PasswordHash = PasswordHasher.Hash(model.Senha);
//            await emailService.Send(user.Name, user.Email, $"Bem-Vindo, {user.Name}!", "Comunidade DevGame");

//            await context.Users.AddAsync(user);
//            await context.SaveChangesAsync();
//            return Ok(new ResultViewModel<dynamic>(new
//            {
//                user = user.Name,
//                message = $"Registro realizado com sucesso! E-mail enviado para: {user.Email}"
//            })) ;
//        }

//        //Gera token para acesso a APIs
//        [HttpPost("login")]
//        public async Task<IActionResult> Login(
//            [FromBody] LoginViewModel model,
//            [FromServices] DataContext context,
//            [FromServices] TokenService tokenService)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

//            var user = await context.Users.AsNoTracking().Include(x => x.Roles).FirstOrDefaultAsync(x => x.Email == model.Email);

//            if (user == null)
//                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));    

//            if(!PasswordHasher.Verify(user.PasswordHash, model.Password))
//                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

//            try
//            {
//                var token = tokenService.GenerateToken(user);
//                return Ok(new ResultViewModel<dynamic>(new { token = token}));
//            }
//            catch
//            {
//                return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
//            }
//        }

//        //método temporário (para ambiente de desenvolvimento)
//        [HttpGet]
//        public async Task<IActionResult> Getall([FromServices] DataContext context) => Ok( await context.Users.Include(i => i.Roles).ToListAsync());


//        [Authorize]
//        [HttpGet("{id:int}")]
//        public async Task<IActionResult> Get(int id,
//            [FromServices] DataContext context) 
//        {
//            var user = await context.Users.Include(x => x.Roles).FirstOrDefaultAsync(u => u.Id == id);
//            if (user == null) 
//                return NotFound(new ResultViewModel<string>("Não existe registro com o id específicado"));

//            return Ok(user);
//        }

//        [Authorize]
//        [HttpPut("update/{id:int}")]
//        public async Task<IActionResult> Put(int id,
//           [FromServices] DataContext context,
//           [FromServices] EmailService emailService,
//           [FromBody] UpdateViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

//            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

//            if (user == null) return NotFound(new ResultViewModel<string>("Não existe registro com o id específicado"));

//            user.Name = model.Name;
//            user.PasswordHash = PasswordHasher.Hash(model.Senha);

//            await emailService.Send(user.Name, user.Email, $"Atualização da conta!", $"Olá, {user.Name}! Suas informações foram alteradas recentemente.");

//            context.Users.Update(user);
//            await context.SaveChangesAsync();
//            return Ok(new ResultViewModel<dynamic>(new
//            {
//                user = user.Name,
//                message = $"Atualização realizada com sucesso! E-mail enviado para: {user.Email}"
//            }));
//        }



//        [Authorize]
//        [HttpDelete("{id:int}")]
//        public async Task<IActionResult> DeleteAsync(
//         [FromRoute] int id,
//         [FromServices] DataContext context)
//        {
//            try
//            {
//                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

//                if (user == null) return NotFound(new ResultViewModel<string>("Não existe usuário com o id específicado"));

//                context.Users.Remove(user);
//                await context.SaveChangesAsync();

//                return Ok(new ResultViewModel<User>(user));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ResultViewModel<string>("05X12 - Falha interna no servidor"));
//            }

//        }
//    }
//}
