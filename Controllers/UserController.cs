using API.Common;
using API.Services.Interfaces;
using API.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IUserRepository _IUserRepository;

        public UserController(IUserRepository IUserRepository)
        {
            _IUserRepository = IUserRepository;
        }

        [Authorize(Roles = "1")]
        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(UserViewModel model)
        {
            var rs = await _IUserRepository.AddUser(model);

            return Ok(rs);
        }

        [Authorize(Roles = "1")]
        [HttpDelete("DeleteUserById")]
        public async Task<IActionResult> DeleteUserById([FromQuery] string id)
        {
            var rs = await _IUserRepository.DeleteUserById(id);

            return Ok(rs);
        }

        [HttpPost("GetAllUser")]
        public async Task<IActionResult> GetAllUser(PagingData page)
        {
            var rs = await _IUserRepository.GetAllUser(page);

            return Ok(rs);
        }

        [HttpGet("GetRoleUserById")]
        public async Task<IActionResult> GetRoleUserById([FromQuery] string id)
        {
            var rs = await _IUserRepository.GetRoleUserById(id);

            return Ok(rs);
        }

        [Authorize(Roles = "1")]
        [HttpPost("UpdateUserById")]
        public async Task<IActionResult> UpdateUserById(UserViewModel model)
        {
            var rs = await _IUserRepository.UpdateUserById(model);

            return Ok(rs);
        }

        [Authorize(Roles = "1")]
        [HttpPost("UpdatePasswordUserById")]
        public async Task<IActionResult> UpdatePasswordUserById(UserViewModel model)
        {
            var rs = await _IUserRepository.UpdatePasswordUserById(model);

            return Ok(rs);
        }
    }
}
