using System.ComponentModel.DataAnnotations;

namespace msuser.ViewModel.Accounts
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email obrigatório!")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Senha obrigatória!")]
        [StringLength(255, ErrorMessage = "Senha deve conter entre 6 e 255 caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
    }
}
