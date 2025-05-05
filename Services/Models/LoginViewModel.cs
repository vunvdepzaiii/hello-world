namespace API.Services.Models
{
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? Token { get; set; }
        public int? Status { get; set; }
        public string? Message { get; set; }
    }
}
