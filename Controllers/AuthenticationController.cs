using API.Services.Interfaces;
using API.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private IAuthenticationRepository _IAuthenticationRepository;

        public AuthenticationController(IAuthenticationRepository IAuthenticationRepository)
        {
            _IAuthenticationRepository = IAuthenticationRepository;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Authenticate user
            var user = await _IAuthenticationRepository.Login(model);

            if (user == null)
                return Unauthorized();

            return Ok(user);
        }
    }
}
