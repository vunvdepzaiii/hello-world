namespace API.Services.Models
{
    public class RoleUserViewModel : GeneralViewModel
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public int? RoleType { get; set; }   // 1. Admin, 2. User
    }
}
