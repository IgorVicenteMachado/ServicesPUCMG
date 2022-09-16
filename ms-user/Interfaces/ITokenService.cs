using ms_user.Models;

namespace ms_user.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(User usuario);
    }
}
