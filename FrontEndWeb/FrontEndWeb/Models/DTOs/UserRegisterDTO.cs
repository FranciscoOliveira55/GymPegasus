using System.ComponentModel.DataAnnotations;

namespace FrontEndWeb.Models.DTOs
{
    public class UserRegisterDTO
    {
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
