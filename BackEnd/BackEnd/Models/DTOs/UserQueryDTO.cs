using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.DTOs
{
    public class UserQueryDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
