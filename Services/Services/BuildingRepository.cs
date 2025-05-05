using API.Common;
using API.Entities;
using API.Services.Interfaces;
using API.Services.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Services.Services
{
    public class BuildingRepository : IBuildingRepository
    {
        private IMapper _IMapper;
        private DataContext _DataContext;

        public BuildingRepository(IMapper IMapper, DataContext DataContext)
        {
            _IMapper = IMapper;
            _DataContext = DataContext;
        }

        public async Task<ResultCommon> AddBuilding(BuildingViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.Buildings.FirstOrDefaultAsync(x => x.Name == entity.Name && x.Code == entity.Code);
            if (existed == null)
            {
                entity.Id = Guid.NewGuid().ToString();
                var building = _IMapper.Map<Building>(entity);
                await _DataContext.Buildings.AddAsync(building);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Thêm tòa nhà thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Thêm tòa nhà không thành công!");
                }
            }
            else
            {
                result.Status = -1;
                result.Message = string.Format("Đã tồn tại tòa nhà trên hệ thống!");
            }

            return result;
        }

        public async Task<ResultCommon> DeleteBuildingById(string id)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.Buildings.FirstOrDefaultAsync(x => x.Id.ToString() == id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại tòa nhà trên hệ thống!");
            }
            else
            {
                _DataContext.Buildings.Remove(existed);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Xóa tòa nhà thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Xóa tòa nhà không thành công!");
                }
            }

            return result;
        }

        public async Task<PagingData> GetAllBuilding(PagingData page)
        {
            var query = _DataContext.Buildings.AsNoTracking()
                .Select(b => new BuildingViewModel
                {
                    Id = b.Id.ToString(),
                    Name = b.Name,
                    Code = b.Code,
                    Address = b.Address,
                    Description = b.Description,
                    CreatedDate = b.CreatedDate
                });

            foreach (var s in page.ListSearchData)
            {
                switch (s.ColSearch)
                {
                    case "Name":
                        query = query.Where(x => x.Name.Contains(s.ValueSearch));
                        break;
                    case "Code":
                        query = query.Where(x => x.Code.Contains(s.ValueSearch));
                        break;
                    case "Address":
                        query = query.Where(x => x.Address.Contains(s.ValueSearch));
                        break;
                    case "Description":
                        query = query.Where(x => x.Description.Contains(s.ValueSearch));
                        break;
                    default:
                        break;
                }
            }
            query = query.OrderByDescending(x => x.CreatedDate);

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

        public async Task<ResultCommon> UpdateBuildingById(BuildingViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.Buildings.FirstOrDefaultAsync(x => x.Id.ToString() == entity.Id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại tòa nhà trên hệ thống!");
            }
            else
            {
                var duplicate = await _DataContext.Buildings.AsNoTracking().Select(b => new BuildingViewModel
                {
                    Id = b.Id.ToString(),
                    Code = b.Code,
                    Name = b.Name
                }).FirstOrDefaultAsync(x => x.Id != entity.Id && x.Code == entity.Code && x.Name == entity.Name);
                if (duplicate == null)
                {
                    existed.Name = entity.Name;
                    existed.Code = entity.Code;
                    existed.Address = entity.Address;
                    existed.Description = entity.Description;
                    _DataContext.Buildings.Update(existed);
                    if (await _DataContext.SaveChangesAsync() > 0)
                    {
                        result.Status = 1;
                        result.Message = string.Format("Cập nhật tòa nhà thành công");
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = string.Format("Cập nhật tòa nhà không thành công!");
                    }
                }
                else
                {
                    result.Status = -1;
                    result.Message = string.Format("Đã tồn tại tòa nhà: {0} - {1} trên hệ thống!", duplicate.Code, duplicate.Name);
                }
            }

            return result;
        }
    }
}
