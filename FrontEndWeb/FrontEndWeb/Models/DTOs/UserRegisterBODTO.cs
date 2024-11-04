using System.ComponentModel.DataAnnotations;

namespace FrontEndWeb.Models.DTOs
{
    public class UserRegisterBODTO
    {
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public IList<string>? Roles { get; set; }
    }
}
