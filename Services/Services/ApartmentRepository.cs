using API.Common;
using API.Entities;
using API.Services.Interfaces;
using API.Services.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;

namespace API.Services.Services
{
    public class ApartmentRepository : IApartmentRepository
    {
        private IMapper _IMapper;
        private DataContext _DataContext;

        public ApartmentRepository(IMapper IMapper, DataContext DataContext)
        {
            _IMapper = IMapper;
            _DataContext = DataContext;
        }

        public async Task<ResultCommon> AddApartment(ApartmentViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.Apartments.AsNoTracking().Select(a => new ApartmentViewModel
            {
                Id = a.Id.ToString(),
                Number = a.Number,
                BuildingId = a.BuildingId.ToString(),
                BuildingViewModel = _DataContext.Buildings.AsNoTracking().Where(x => x.Id == a.BuildingId).Select(b => new BuildingViewModel
                {
                    Id = b.Id.ToString(),
                    Name = b.Name,
                    Code = b.Code,
                }).FirstOrDefault(),
            })
                .FirstOrDefaultAsync(x => x.Number == entity.Number && x.BuildingId == entity.BuildingId);
            if (existed == null)
            {
                entity.Id = Guid.NewGuid().ToString();
                var apartment = _IMapper.Map<Apartment>(entity);
                await _DataContext.Apartments.AddAsync(apartment);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Thêm căn hộ thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Thêm căn hộ không thành công!");
                }
            }
            else
            {
                result.Status = -1;
                result.Message = string.Format("Đã tồn tại căn hộ: {0} - tòa nhà: {1} trên hệ thống!", existed.Number, existed.BuildingViewModel?.Name);
            }

            return result;
        }

        public async Task<List<ApartmentViewModel>> CheckingDataImport(IFormFile file)
        {
            var list = new List<ApartmentViewModel>();
            var listBuilding = await _DataContext.Buildings.AsNoTracking().ToListAsync();
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null) return list;

            var rowCount = worksheet.Dimension?.Rows;
            var users = new List<User>();
            var errors = new List<string>();

            for (int row = 6; row <= rowCount; row++) // Bỏ qua dòng header
            {
                var avm = new ApartmentViewModel();
                avm.IndexImport = row;
                avm.Number = worksheet.Cells[row, 1].Text?.Trim();
                avm.Owner = worksheet.Cells[row, 2].Text?.Trim();
                avm.Phone = worksheet.Cells[row, 3].Text?.Trim();
                avm.AccountZalo = worksheet.Cells[row, 7].Text?.Trim();
                avm.ManageFeeImport = worksheet.Cells[row, 5].Text?.Trim();
                avm.StartDateImport = worksheet.Cells[row, 6].Text?.Trim();
                avm.BuildingViewModel = new BuildingViewModel() { Name = worksheet.Cells[row, 4].Text?.Trim() };

                // mã căn hộ
                if (string.IsNullOrEmpty(avm.Number) && avm.IsValid != false)
                {
                    avm.IsValid = false;
                    avm.MessageCheck = string.Format("Dòng {0}: Mã căn hộ không được để trống!", row);
                    list.Add(avm);
                    continue;
                }

                // tên chủ hộ
                if (string.IsNullOrEmpty(avm.Owner) && avm.IsValid != false)
                {
                    avm.IsValid = false;
                    avm.MessageCheck = string.Format("Dòng {0}: Chủ căn hộ không được để trống!", row);
                    list.Add(avm);
                    continue;
                }

                // tòa nhà
                if (listBuilding.FirstOrDefault(x => x.Name.ToLower() == worksheet.Cells[row, 4].Text?.Trim().ToLower()) == null)
                {
                    if (avm.IsValid != false)
                    {
                        avm.IsValid = false;
                        avm.MessageCheck = string.Format("Dòng {0}: Tòa nhà: {1} không có trên hệ thống!", row, worksheet.Cells[row, 4].Text?.Trim());
                        list.Add(avm);
                        continue;
                    }
                }
                else
                {
                    avm.BuildingViewModel = _IMapper.Map<BuildingViewModel>(listBuilding.FirstOrDefault(x => x.Name.ToLower() == worksheet.Cells[row, 4].Text?.Trim().ToLower()));
                    avm.BuildingId = avm.BuildingViewModel?.Id;
                }

                // phí quản lý
                if (string.IsNullOrEmpty(worksheet.Cells[row, 5].Text?.Trim()))
                {
                    if (avm.IsValid != false)
                    {
                        avm.IsValid = false;
                        avm.MessageCheck = string.Format("Dòng {0}: Phí quản lý không được để trống!", row);
                        list.Add(avm);
                        continue;
                    }

                }
                else if (!decimal.TryParse(worksheet.Cells[row, 5].Text?.Trim(), out var fee))
                {
                    if (avm.IsValid != false)
                    {
                        avm.IsValid = false;
                        avm.MessageCheck = string.Format("Dòng {0}: Phí quản lý {1} không đúng định dạng!", row, worksheet.Cells[row, 5].Text?.Trim());
                        list.Add(avm);
                        continue;
                    }
                }
                else
                {
                    avm.ManageFee = decimal.Parse(worksheet.Cells[row, 5].Text?.Trim());
                    avm.ManageFeeImport = avm.ManageFee.Value.ToString("N2", new CultureInfo("vi-VN"));
                }

                // ngày bàn giao
                if (string.IsNullOrEmpty(worksheet.Cells[row, 6].Text?.Trim()))
                {
                    if (avm.IsValid != false)
                    {
                        avm.IsValid = false;
                        avm.MessageCheck = string.Format("Dòng {0}: Ngày bàn giao không được để trống!", row);
                        list.Add(avm);
                        continue;
                    }
                }
                else if (!DateTime.TryParse(worksheet.Cells[row, 6].Text?.Trim(), out var start))
                {
                    if (avm.IsValid != false)
                    {
                        avm.IsValid = false;
                        avm.MessageCheck = string.Format("Dòng {0}: Ngày bàn giao {1} không đúng định dạng!", row, worksheet.Cells[row, 6].Text?.Trim());
                        list.Add(avm);
                        continue;
                    }
                }
                else
                {
                    avm.StartDate = DateTime.Parse(worksheet.Cells[row, 6].Text?.Trim());
                    avm.StartDateImport = avm.StartDate.Value.ToString("dd/MM/yyyy");
                }

                if (list.FirstOrDefault(x => x.Number == avm.Number && x.BuildingId == avm.BuildingId) != null)
                {
                    avm.IsValid = false;
                    avm.MessageCheck = string.Format("Dòng {0}: Mã căn hộ: {1} - Tòa nhà: {2}. Dữ liệu bị trùng!", row, avm.Number, avm.BuildingViewModel?.Name);
                    list.Add(avm);
                    continue;
                }

                avm.IsValid = true;
                avm.MessageCheck = string.Format("Dữ liệu hợp lệ");
                list.Add(avm);
            }

            return list;
        }

        public async Task<ResultCommon> DeleteApartmentById(string id)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.Apartments.FirstOrDefaultAsync(x => x.Id.ToString() == id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại căn hộ trên hệ thống!");
            }
            else
            {
                _DataContext.Apartments.Remove(existed);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    // xóa phí xe
                    var vehicleFee = await _DataContext.VehicleFees.Where(x => x.ApartmentId.ToString() == id).ToListAsync();
                    _DataContext.VehicleFees.RemoveRange(vehicleFee);
                    await _DataContext.SaveChangesAsync();

                    result.Status = 1;
                    result.Message = string.Format("Xóa căn hộ thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Xóa căn hộ không thành công!");
                }
            }

            return result;
        }

        public async Task<PagingData> GetAllApartment(PagingData page)
        {
            var query = _DataContext.Apartments.AsNoTracking()
                .Select(a => new ApartmentViewModel
                {
                    Id = a.Id.ToString(),
                    Number = a.Number,
                    Owner = a.Owner,
                    Phone = a.Phone,
                    AccountZalo = a.AccountZalo,
                    BuildingId = a.BuildingId.ToString(),
                    BuildingViewModel = _DataContext.Buildings.AsNoTracking().Where(x => x.Id == a.BuildingId).Select(b => new BuildingViewModel
                    {
                        Id = b.Id.ToString(),
                        Name = b.Name,
                        Code = b.Code,
                        CreatedDate = b.CreatedDate
                    }).FirstOrDefault(),
                    ManageFee = a.ManageFee,
                    StartDate = a.StartDate,
                    ListVehicleFeeViewModel = _DataContext.VehicleFees.AsNoTracking().Where(x => x.BuildingId == a.BuildingId && x.ApartmentId == a.Id)
                        .Select(vf => new VehicleFeeViewModel
                        {
                            Id = vf.Id.ToString(),
                            TypeName = vf.Type == 1 ? "Xe đạp" : vf.Type == 2 ? "Xe điện" : vf.Type == 3 ? "Xe máy" : vf.Type == 4 ? "Ô tô" : "Loại khác",
                            License = vf.License,
                            Price = vf.Price
                        }).ToList(),
                });

            foreach (var s in page.ListSearchData)
            {
                switch (s.ColSearch)
                {
                    case "Number":
                        query = query.Where(x => x.Number.Contains(s.ValueSearch));
                        break;
                    case "Owner":
                        query = query.Where(x => x.Owner.Contains(s.ValueSearch));
                        break;
                    case "Phone":
                        query = query.Where(x => x.Phone.Contains(s.ValueSearch));
                        break;
                    case "AccountZalo":
                        query = query.Where(x => x.AccountZalo.Contains(s.ValueSearch));
                        break;
                    case "BuildingId":
                        query = query.Where(x => x.BuildingId == s.ValueSearch);
                        break;
                    case "StartDate":
                        query = query.Where(x => x.StartDate.Value.Date == DateTime.Parse(s.ValueSearch).Date);
                        break;
                    default:
                        break;
                }
            }
            query = query.OrderByDescending(x => x.BuildingViewModel.CreatedDate).OrderByDescending(x => x.Number);

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

        public async Task<List<ApartmentViewModel>> ImportApartment(List<ApartmentViewModel> list)
        {
            var inputKeySet = list.Select(x => $"{x.Number}_{x.BuildingId}").ToHashSet();
            var dbKeySet = _DataContext.Apartments.AsNoTracking().AsEnumerable().Select(ap => $"{ap.Number}_{ap.BuildingId.ToString()}").Where(key => inputKeySet.Contains(key)).ToHashSet();
            var existedList = list.Where(x => dbKeySet.Contains($"{x.Number}_{x.BuildingId}")).ToList();
            var notExistedList = list.Where(x => !dbKeySet.Contains($"{x.Number}_{x.BuildingId}")).ToList();

            foreach (var itemEx in existedList)
            {
                itemEx.IsValid = false;
                itemEx.MessageCheck = string.Format("Dòng {0}: Mã căn hộ: {1} - Tòa nhà: {2}. Dữ liệu đã tồn tại trên hệ thống!", itemEx.IndexImport, itemEx.Number, itemEx.BuildingViewModel?.Name);
            }

            var listAdd = new List<Apartment>();
            foreach (var itemNotEx in notExistedList)
            {
                itemNotEx.Id = Guid.NewGuid().ToString();
                var apartment = _IMapper.Map<Apartment>(itemNotEx);
                await _DataContext.Apartments.AddAsync(apartment);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    itemNotEx.IsValid = true;
                    itemNotEx.MessageCheck = string.Format("Thêm căn hộ thành công");
                }
                else
                {
                    itemNotEx.IsValid = false;
                    itemNotEx.MessageCheck = string.Format("Dòng {0}: Thêm căn hộ không thành công", itemNotEx.IndexImport);
                }
            }

            return list;
        }

        public async Task<ResultCommon> UpdateApartmentById(ApartmentViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.Apartments.FirstOrDefaultAsync(x => x.Id.ToString() == entity.Id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại căn hộ trên hệ thống!");
            }
            else
            {
                var duplicate = await _DataContext.Apartments.AsNoTracking().Select(a => new ApartmentViewModel
                {
                    Id = a.Id.ToString(),
                    Number = a.Number,
                    BuildingId = a.BuildingId.ToString(),
                    BuildingViewModel = _DataContext.Buildings.AsNoTracking().Where(x => x.Id == a.BuildingId).Select(b => new BuildingViewModel
                    {
                        Id = b.Id.ToString(),
                        Name = b.Name,
                        Code = b.Code,
                    }).FirstOrDefault(),
                }).FirstOrDefaultAsync(x => x.Number == entity.Number && x.BuildingId == entity.BuildingId);
                if (duplicate == null)
                {
                    existed.Number = entity.Number;
                    existed.Owner = entity.Owner;
                    existed.Phone = entity.Phone;
                    existed.AccountZalo = entity.AccountZalo;
                    existed.BuildingId = Guid.Parse(entity.BuildingId);
                    existed.ManageFee = entity.ManageFee.Value;
                    existed.StartDate = entity.StartDate.Value;
                    _DataContext.Apartments.Update(existed);
                    if (await _DataContext.SaveChangesAsync() > 0)
                    {
                        result.Status = 1;
                        result.Message = string.Format("Cập nhật căn hộ thành công");
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = string.Format("Cập nhật căn hộ không thành công!");
                    }
                }
                else
                {
                    result.Status = -1;
                    result.Message = string.Format("Đã tồn tại căn hộ: {0} - tòa nhà: {1} trên hệ thống!", duplicate.Number, duplicate?.BuildingViewModel?.Name);
                }
            }

            return result;
        }
    }
}
