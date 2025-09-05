using System.ComponentModel.DataAnnotations;

namespace ApiIntegrationMvc.Areas.Account.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100)]
        public string Password { get; set; }
    }
}
