using System.ComponentModel.DataAnnotations;

namespace FrontEndWeb.Models.DTOs
{
    public class UserRoleDTO
    {
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [RegularExpression("^(Cliente|Employee)$", ErrorMessage = "RoleName must be 'Cliente' or 'Employee'.")]
        public string RoleName { get; set; }
    }
}
