using msuser.Enums;
using System.ComponentModel.DataAnnotations;

namespace msuser.ViewModel.Accounts
{
    public class UserVM
    {
        [Required(ErrorMessage = "Email obrigatório!")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }


        [StringLength(255, ErrorMessage = "Senha deve conter entre 6 e 255 caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Senha { get; set; }


        [StringLength(100, ErrorMessage = "Primeiro nome deve conter entre 2 e 100 caracteres", MinimumLength = 2)]
        public string PrimeiroNome { get; set; }


        [StringLength(100, ErrorMessage = "Ultimo nome deve conter entre 2 e 100 caracteres", MinimumLength = 2)]
        public string UltimoNome { get; set; }


        [Required(ErrorMessage = "Obrigatório definição do perfil!")]
        public Profile Perfil { get; set; }
    }
}
