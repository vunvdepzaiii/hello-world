namespace API.Services.Models
{
    public class UserViewModel : GeneralViewModel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool? IsAdmin { get; set; }
    }
}
