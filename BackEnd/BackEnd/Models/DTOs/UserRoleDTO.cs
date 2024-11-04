using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.DTOs
{
    public class UserRoleDTO
    {
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }
        public string RoleName { get; set; }
    }
}
