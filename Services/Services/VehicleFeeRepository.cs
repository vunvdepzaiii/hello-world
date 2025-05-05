using API.Common;
using API.Entities;
using API.Services.Interfaces;
using API.Services.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Services
{
    public class VehicleFeeRepository : IVehicleFeeRepository
    {
        private IMapper _IMapper;
        private DataContext _DataContext;

        public VehicleFeeRepository(IMapper IMapper, DataContext DataContext)
        {
            _IMapper = IMapper;
            _DataContext = DataContext;
        }

        public async Task<ResultCommon> AddVehicleFee(VehicleFeeViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.VehicleFees.AsNoTracking().Select(vf => new VehicleFeeViewModel
            {
                Id = vf.Id.ToString(),
                Type = vf.Type,
                License = vf.License,
                TypeName = vf.Type == 1 ? "Xe đạp" : vf.Type == 2 ? "Xe điện" : vf.Type == 3 ? "Xe máy" : vf.Type == 4 ? "Ô tô" : "Loại khác",
                ApartmentId = vf.ApartmentId.ToString(),
                BuildingId = vf.BuildingId.ToString(),
            }).FirstOrDefaultAsync(x => x.BuildingId == entity.BuildingId && x.ApartmentId == entity.ApartmentId && x.Type == entity.Type && x.License == entity.License);
            if (existed == null)
            {
                entity.Id = Guid.NewGuid().ToString();
                var vehicleFee = _IMapper.Map<VehicleFee>(entity);
                await _DataContext.VehicleFees.AddAsync(vehicleFee);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Thêm phí gửi xe thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Thêm phí gửi xe không thành công!");
                }
            }
            else
            {
                result.Status = -1;
                result.Message = string.Format("Đã tồn tại phí gửi xe: {0} - biển số: {1} trên hệ thống!", existed.TypeName, existed.License);
            }

            return result;
        }

        public async Task<ResultCommon> DeleteVehicleFeeById(string id)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.VehicleFees.FirstOrDefaultAsync(x => x.Id.ToString() == id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại phí gửi xe trên hệ thống!");
            }
            else
            {
                _DataContext.VehicleFees.Remove(existed);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Xóa phí gửi xe thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Xóa phí gửi xe không thành công!");
                }
            }

            return result;
        }

        public async Task<PagingData> GetAllVehicleFee(PagingData page)
        {
            var query = _DataContext.VehicleFees.AsNoTracking()
                .Select(vf => new VehicleFeeViewModel
                {
                    Id = vf.Id.ToString(),
                    BuildingId = vf.BuildingId.ToString(),
                    ApartmentId = vf.ApartmentId.ToString(),
                    Type = vf.Type,
                    License = vf.License,
                    Price = vf.Price,
                    Ticket = vf.Ticket,
                    TypeName = vf.Type == 1 ? "Xe đạp" : vf.Type == 2 ? "Xe điện" : vf.Type == 3 ? "Xe máy" : vf.Type == 4 ? "Ô tô" : "Loại khác",
                    CreatedDate = vf.CreatedDate
                });

            foreach (var s in page.ListSearchData)
            {
                switch (s.ColSearch)
                {
                    case "Type":
                        query = query.Where(x => x.Type == int.Parse(s.ValueSearch));
                        break;
                    case "License":
                        query = query.Where(x => x.License.Contains(s.ValueSearch));
                        break;
                    case "Ticket":
                        query = query.Where(x => x.Ticket.Contains(s.ValueSearch));
                        break;
                    case "ApartmentId":
                        query = query.Where(x => x.ApartmentId == s.ValueSearch);
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

        public async Task<ResultCommon> UpdateVehicleFeeById(VehicleFeeViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.VehicleFees.FirstOrDefaultAsync(x => x.Id.ToString() == entity.Id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại phí gửi xe trên hệ thống!");
            }
            else
            {
                var duplicate = await _DataContext.VehicleFees.AsNoTracking().Select(vf => new VehicleFeeViewModel
                {
                    Id = vf.Id.ToString(),
                    Type = vf.Type,
                    License = vf.License,
                    TypeName = vf.Type == 1 ? "Xe đạp" : vf.Type == 2 ? "Xe điện" : vf.Type == 3 ? "Xe máy" : vf.Type == 4 ? "Ô tô" : "Loại khác",
                    ApartmentId = vf.ApartmentId.ToString(),
                    BuildingId = vf.BuildingId.ToString(),
                }).FirstOrDefaultAsync(x => x.Id != entity.Id && x.BuildingId == entity.BuildingId && x.ApartmentId == entity.ApartmentId && x.Type == entity.Type && x.License == entity.License);
                if (duplicate == null)
                {
                    existed.BuildingId = Guid.Parse(entity.BuildingId);
                    existed.ApartmentId = Guid.Parse(entity.ApartmentId);
                    existed.Type = entity.Type.Value;
                    existed.License = entity.License;
                    existed.Price = entity.Price.Value;
                    existed.Ticket = entity.Ticket;
                    _DataContext.VehicleFees.Update(existed);
                    if (await _DataContext.SaveChangesAsync() > 0)
                    {
                        result.Status = 1;
                        result.Message = string.Format("Cập nhật phí gửi xe thành công");
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = string.Format("Cập nhật phí gửi xe không thành công!");
                    }
                }
                else
                {
                    result.Status = -1;
                    result.Message = string.Format("Đã tồn tại phí gửi xe: {0} - biển số: {1} trên hệ thống!", duplicate.TypeName, duplicate.License);
                }
            }

            return result;
        }
    }
}
