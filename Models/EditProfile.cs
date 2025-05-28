using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class EditProfileViewModel
    {
        [Display(Name = "Логин")]
        public string Username { get; set; }
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        [Display(Name = "Новый Email")]
        public string? NewEmail { get; set; }
    }
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтвердите новый пароль")]
        public string ConfirmPassword { get; set; }
    }
}
