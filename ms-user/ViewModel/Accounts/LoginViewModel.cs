using System.ComponentModel.DataAnnotations;

namespace ms_user.ViewModel.Accounts
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }


        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Password { get; set; }
    }
}
