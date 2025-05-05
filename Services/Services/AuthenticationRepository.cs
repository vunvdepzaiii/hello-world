using API.Common;
using API.Entities;
using API.Services.Interfaces;
using API.Services.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services.Services
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private DataContext _dataContext;

        public AuthenticationRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<LoginViewModel> Login(LoginViewModel login)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.UserName == login.UserName);
            if (user == null)
            {
                login.Status = -1;
                login.Message = string.Format("Tài khoản không tồn tại trong hệ thống!");
            }
            else
            {
                if (ServiceCommon.VerifyPassword(user, user.Password, login.Password))
                {
                    login.Status = 1;
                    login.Message = string.Format("Đăng nhập thành công!");
                    var roleUser = await _dataContext.RoleUsers.FirstOrDefaultAsync(x => x.UserId == user.Id);

                    login.Token = ServiceCommon.GenerateJwtToken(user, roleUser);
                }
                else
                {
                    login.Status = 0;
                    login.Message = string.Format("Mật khẩu không chính xác!");
                }
            }
            return login;
        }
    }
}
