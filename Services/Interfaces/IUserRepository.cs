using API.Common;
using API.Entities;
using API.Services.Models;

namespace API.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<ResultCommon> AddUser(UserViewModel entity);
        Task<ResultCommon> DeleteUserById(string id);
        Task<PagingData> GetAllUser(PagingData page);
        Task<RoleUserViewModel> GetRoleUserById(string id);
        Task<ResultCommon> UpdatePasswordUserById(UserViewModel entity);
        Task<ResultCommon> UpdateUserById(UserViewModel entity);
    }
}
