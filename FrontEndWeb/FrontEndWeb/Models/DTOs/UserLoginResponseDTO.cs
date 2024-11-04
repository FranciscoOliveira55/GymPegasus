using System.ComponentModel.DataAnnotations;

namespace FrontEndWeb.Models.DTOs
{
    public class UserLoginResponseDTO
    {
        public string Id { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Token { get; set; }

        public IEnumerable<string>? Roles { get; set; } = new List<string>();
        public int? ClienteId { get; set; } = -1;
        public int? EmployeeId { get; set; } = -1;

    }
}
