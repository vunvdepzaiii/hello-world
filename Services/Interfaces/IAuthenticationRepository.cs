using API.Entities;
using API.Services.Models;

namespace API.Services.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task<LoginViewModel> Login(LoginViewModel user);
    }
}
