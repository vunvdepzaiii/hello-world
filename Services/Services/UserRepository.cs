using API.Common;
using API.Entities;
using API.Services.Interfaces;
using API.Services.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Services.Services
{
    public class UserRepository : IUserRepository
    {
        private IMapper _IMapper;
        private DataContext _DataContext;

        public UserRepository(IMapper IMapper, DataContext DataContext)
        {
            _IMapper = IMapper;
            _DataContext = DataContext;

        }

        public async Task<ResultCommon> AddUser(UserViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.Users.FirstOrDefaultAsync(x => x.UserName == entity.UserName);
            if (existed == null)
            {
                entity.Id = Guid.NewGuid().ToString();
                var user = _IMapper.Map<User>(entity);
                user.Password = ServiceCommon.HashPassword(user, user.Password);
                await _DataContext.Users.AddAsync(user);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Thêm tài khoản thành công");
                    var roleUser = new RoleUser()
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id
                    };
                    if (entity.IsAdmin == true) roleUser.RoleType = 1;
                    else roleUser.RoleType = 2;
                    await _DataContext.RoleUsers.AddAsync(roleUser);
                    await _DataContext.SaveChangesAsync();
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Thêm tài khoản không thành công!");
                }
            }
            else
            {
                result.Status = -1;
                result.Message = string.Format("Đã tồn tại tài khoản: {0} trên hệ thống!", existed.UserName);
            }

            return result;
        }

        public async Task<ResultCommon> DeleteUserById(string id)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.Users.FirstOrDefaultAsync(x => x.Id.ToString() == id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại tài khoản trên hệ thống!");
            }
            else
            {
                var me = ServiceCommon.GetCurrentUserClaim(ClaimTypes.NameIdentifier);
                if (me == existed.UserName)
                {
                    result.Status = 0;
                    result.Message = string.Format("Không thể tự xóa tài khoản của mình!");
                }
                else
                {
                    _DataContext.Users.Remove(existed);
                    if (await _DataContext.SaveChangesAsync() > 0)
                    {
                        result.Status = 1;
                        result.Message = string.Format("Xóa tài khoản thành công");

                        // xóa role user
                        var roleUser = await _DataContext.RoleUsers.FirstOrDefaultAsync(x => x.UserId == existed.Id);
                        if (roleUser != null)
                        {
                            _DataContext.RoleUsers.Remove(roleUser);
                            await _DataContext.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = string.Format("Xóa tài khoản không thành công!");
                    }
                }
            }

            return result;
        }

        public async Task<PagingData> GetAllUser(PagingData page)
        {
            var query = _DataContext.Users.AsNoTracking()
                .Select(u => new UserViewModel
                {
                    Id = u.Id.ToString(),
                    UserName = u.UserName,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone
                });

            foreach (var s in page.ListSearchData)
            {
                switch (s.ColSearch)
                {
                    case "UserName":
                        query = query.Where(x => x.UserName.Contains(s.ValueSearch));
                        break;
                    case "FullName":
                        query = query.Where(x => x.FullName.Contains(s.ValueSearch));
                        break;
                    case "Email":
                        query = query.Where(x => x.Email.Contains(s.ValueSearch));
                        break;
                    case "Phone":
                        query = query.Where(x => x.Phone.Contains(s.ValueSearch));
                        break;
                    default:
                        break;
                }
            }
            query = query.OrderBy(x => x.UserName);

            int totalItems = query.Count();

            var list = await query.Skip((page.PageNum.Value - 1) * page.PageSize.Value).Take(page.PageSize.Value).ToListAsync();

            return new PagingData
            {
                Data = list,
                PageNum = page.PageNum,
                PageSize = page.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((double)totalItems / page.PageSize.Value)
            };
        }

        public async Task<RoleUserViewModel> GetRoleUserById(string id)
        {
            var role = await _DataContext.RoleUsers.FirstOrDefaultAsync(x => x.UserId.ToString() == id);
            return _IMapper.Map<RoleUserViewModel>(role);
        }

        public async Task<ResultCommon> UpdatePasswordUserById(UserViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.Users.FirstOrDefaultAsync(x => x.Id.ToString() == entity.Id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại tài khoản trên hệ thống!");
            }
            else
            {
                existed.Password = ServiceCommon.HashPassword(existed, entity.Password);
                _DataContext.Users.Update(existed);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Cập nhật mật khẩu thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Cập nhật mật khẩu không thành công!");
                }
            }
            return result;
        }

        public async Task<ResultCommon> UpdateUserById(UserViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.Users.FirstOrDefaultAsync(x => x.Id.ToString() == entity.Id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại tài khoản trên hệ thống!");
            }
            else
            {
                var duplicate = await _DataContext.Users.AsNoTracking().Select(u => new UserViewModel
                {
                    Id = u.Id.ToString(),
                    UserName = u.UserName
                }).FirstOrDefaultAsync(x => x.Id != entity.Id && x.UserName == entity.UserName);
                if (duplicate == null)
                {
                    existed.FullName = entity.FullName;
                    existed.Email = entity.Email;
                    existed.Phone = entity.Phone;
                    _DataContext.Users.Update(existed);
                    if (await _DataContext.SaveChangesAsync() > 0)
                    {
                        result.Status = 1;
                        result.Message = string.Format("Cập nhật tài khoản thành công");

                        var roleUser = await _DataContext.RoleUsers.FirstOrDefaultAsync(x => x.UserId == existed.Id);
                        if (roleUser == null)
                        {
                            roleUser = new RoleUser()
                            {
                                Id = Guid.NewGuid(),
                                UserId = existed.Id
                            };
                            if (entity.IsAdmin == true) roleUser.RoleType = 1;
                            else roleUser.RoleType = 2;
                            await _DataContext.RoleUsers.AddAsync(roleUser);
                        }
                        else
                        {
                            if (entity.IsAdmin == true) roleUser.RoleType = 1;
                            else roleUser.RoleType = 2;
                            _DataContext.RoleUsers.Update(roleUser);
                        }
                        await _DataContext.SaveChangesAsync();
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = string.Format("Cập nhật tài khoản không thành công!");
                    }
                }
                else
                {
                    result.Status = -1;
                    result.Message = string.Format("Đã tồn tại tài khoản: {0} trên hệ thống!", duplicate.UserName);
                }
            }

            return result;
        }
    }
}
