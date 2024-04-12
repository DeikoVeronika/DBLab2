using System.ComponentModel.DataAnnotations;

namespace BeautySpaceInfrastructure.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введіть електронну пошту")]
        [Display(Name ="Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введіть пароль")]
        [Display(Name ="Пароль")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Введіть пароль ще раз")]
        [Compare("Password", ErrorMessage="Паролі не співпадають")]
        [Display(Name = "Підтвердження пароля")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        [Display(Name = "Аватар")]
        public byte[]? Avatar { get; set; }


    }
}
