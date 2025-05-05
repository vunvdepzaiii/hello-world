using API.Services.Models;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class User : GeneralViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
