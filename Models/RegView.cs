using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Models
{

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Логин должен содержать от 3 до 50 символов")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Имя обязательно")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } // Не обязательное
        [Required(ErrorMessage = "Фамилия обязательна")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать не менее 6 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
    }
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Логин обязателен")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
