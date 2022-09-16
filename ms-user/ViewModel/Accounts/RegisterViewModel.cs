using Microsoft.EntityFrameworkCore;
using ms_user.Enums;
using System.ComponentModel.DataAnnotations;

namespace ms_user.ViewModel.Accounts
{
    public class RegisterViewModel
    {

        [Required (ErrorMessage ="Campo obrigatório!")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [StringLength(255, ErrorMessage = "Senha deve conter entre 5 e 255 caracteres", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [StringLength(255, ErrorMessage = "Senha deve conter entre 5 e 255 caracteres", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "Senhas não correspondem!")]
        public string ConfirmarSenha { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        public Profile Perfil { get; set; }

        public string Token { get; set; }
    }
}
