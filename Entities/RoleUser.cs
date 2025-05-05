using API.Services.Models;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class RoleUser : GeneralViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int RoleType { get; set; }   // 1. Admin, 2. User
    }
}
