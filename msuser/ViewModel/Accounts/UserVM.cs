using msuser.Enums;
using System.ComponentModel.DataAnnotations;

namespace msuser.ViewModel.Accounts
{
    public class UserVM
    {
        [Required(ErrorMessage = "Campo obrigatório!")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Campo obrigatório!")]
        [StringLength(255, ErrorMessage = "Senha deve conter entre 6 e 255 caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }


        [Required(ErrorMessage = "Campo obrigatório!")]
        [StringLength(100, ErrorMessage = "Primeiro nome deve conter entre 2 e 100 caracteres", MinimumLength = 2)]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Campo obrigatório!")]
        [StringLength(100, ErrorMessage = "Ultimo nome deve conter entre 2 e 100 caracteres", MinimumLength = 2)]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Campo obrigatório!")]
        public Profile Profile { get; set; }
    }
}
