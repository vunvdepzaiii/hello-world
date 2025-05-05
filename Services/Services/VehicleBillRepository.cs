using API.Common;
using API.Entities;
using API.Services.Interfaces;
using API.Services.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace API.Services.Services
{
    public class VehicleBillRepository : IVehicleBillRepository
    {
        private IMapper _IMapper;
        private DataContext _DataContext;
        private IConfiguration _IConfiguration;

        public VehicleBillRepository(IMapper IMapper, DataContext DataContext, IConfiguration IConfiguration)
        {
            _IMapper = IMapper;
            _DataContext = DataContext;
            _IConfiguration = IConfiguration;
        }

        public async Task<ResultCommon> AddVehicleBill(VehicleBillViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.VehicleBills.AsNoTracking().Select(vb => new VehicleBillViewModel
            {
                Id = vb.Id.ToString(),
                BuildingId = vb.BuildingId.ToString(),
                BuildingViewModel = _DataContext.Buildings.AsNoTracking().Where(x => x.Id == vb.BuildingId).Select(b => new BuildingViewModel
                {
                    Id = b.Id.ToString(),
                    Name = b.Name,
                    Code = b.Code
                }).FirstOrDefault(),
                ApartmentId = vb.ApartmentId.ToString(),
                ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Where(x => x.Id == vb.ApartmentId).Select(a => new ApartmentViewModel
                {
                    Id = a.Id.ToString(),
                    Number = a.Number
                }).FirstOrDefault(),
                BeginDate = vb.BeginDate,
                EndDate = vb.EndDate
            }).FirstOrDefaultAsync(x => x.BuildingId == entity.BuildingId && x.ApartmentId == entity.ApartmentId && !(entity.EndDate < x.BeginDate || entity.BeginDate > x.EndDate));
            if (existed == null)
            {
                entity.Id = Guid.NewGuid().ToString();
                var vehicleBill = _IMapper.Map<VehicleBill>(entity);
                var lastReceipt = await _DataContext.VehicleBills.OrderByDescending(r => r.CreatedDate).FirstOrDefaultAsync();
                vehicleBill.No = $"PTX{ServiceCommon.GenerateReceiptNo(lastReceipt == null ? "" : lastReceipt.No)}";
                await _DataContext.VehicleBills.AddAsync(vehicleBill);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Thêm phiếu thu phí gửi xe thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Thêm phiếu thu phí gửi xe không thành công!");
                }
            }
            else
            {
                result.Status = -1;
                result.Message = string.Format("Đã tồn tại phiếu thu phí gửi xe căn hộ: {0} - tòa nhà: {1} trong khoảng thời gian từ ngày: {2} đến ngày: {3} trên hệ thống!",
                    existed.ApartmentViewModel?.Number, existed.BuildingViewModel?.Name, existed.BeginDate.Value.ToString("dd/MM/yyyy"), existed.EndDate.Value.ToString("dd/MM/yyyy"));
            }

            return result;
        }

        public async Task<ResultCommon> DeleteVehicleBillById(string id)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.VehicleBills.FirstOrDefaultAsync(x => x.Id.ToString() == id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại phiếu thu phí gửi xe trên hệ thống!");
            }
            else
            {
                _DataContext.VehicleBills.Remove(existed);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Xóa phiếu thu phí gửi xe thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Xóa phiếu thu phí gửi xe không thành công!");
                }
            }

            return result;
        }

        public async Task<PagingData> GetAllVehicleBill(PagingData page)
        {
            var query = _DataContext.VehicleBills.AsNoTracking()
                .Select(vb => new VehicleBillViewModel
                {
                    Id = vb.Id.ToString(),
                    No = vb.No,
                    BuildingId = vb.BuildingId.ToString(),
                    BuildingViewModel = _DataContext.Buildings.AsNoTracking().Where(x => x.Id == vb.BuildingId).Select(b => new BuildingViewModel
                    {
                        Id = b.Id.ToString(),
                        Name = b.Name,
                        Code = b.Code
                    }).FirstOrDefault(),
                    ApartmentId = vb.ApartmentId.ToString(),
                    ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Where(x => x.Id == vb.ApartmentId).Select(a => new ApartmentViewModel
                    {
                        Id = a.Id.ToString(),
                        Number = a.Number,
                        Owner = a.Owner
                    }).FirstOrDefault(),
                    BeginDate = vb.BeginDate,
                    EndDate = vb.EndDate,
                    Amount = vb.Amount,
                    IsPay = vb.IsPay,
                    Payer = vb.Payer,
                    PaymentDate = vb.PaymentDate,
                    Cashier = vb.Cashier,
                    PaymentMethod = vb.PaymentMethod
                });

            foreach (var s in page.ListSearchData)
            {
                switch (s.ColSearch)
                {
                    case "BuildingId":
                        query = query.Where(x => x.BuildingId == s.ValueSearch);
                        break;
                    case "ApartmentId":
                        query = query.Where(x => x.ApartmentId == s.ValueSearch);
                        break;
                    case "IsPay":
                        query = query.Where(x => x.IsPay.ToString() == s.ValueSearch);
                        break;
                    case "PaymentDate":
                        query = query.Where(x => x.PaymentDate.Value.Date == DateTime.Parse(s.ValueSearch).Date);
                        break;
                    default:
                        break;
                }
            }
            query = query.OrderBy(x => x.BuildingViewModel.Name).OrderBy(x => x.ApartmentViewModel.Number).OrderByDescending(x => x.EndDate);

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

        public async Task<byte[]> PrintVehicleBill(string id)
        {
            var mb = await _DataContext.VehicleBills.AsNoTracking().Select(vb => new ReceiptModel
            {
                Id = vb.Id.ToString(),
                CompanyName = _IConfiguration["Information:CompanyName"],
                CompanyAddress = _IConfiguration["Information:CompanyAddress"],
                TemplateNo = _IConfiguration["Information:Receipt:TemplateNo"],
                Decision = _IConfiguration["Information:Receipt:Decision"],
                PlaceOfDecision = _IConfiguration["Information:Receipt:PlaceOfDecision"],
                ReceiptCode = vb.No,
                ReceiptDate = vb.PaymentDate,
                CustomerName = vb.Payer,
                BuildingViewModel = _DataContext.Buildings.AsNoTracking().Select(b => new BuildingViewModel
                {
                    Id = b.Id.ToString(),
                    Name = b.Name,
                    Code = b.Code
                }).FirstOrDefault(x => x.Id == vb.BuildingId.ToString()),
                ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Select(a => new ApartmentViewModel
                {
                    Id = a.Id.ToString(),
                    Number = a.Number
                }).FirstOrDefault(x => x.Id == vb.ApartmentId.ToString()),
                Content = string.Format("Thu phí gửi xe ( từ {0} đến {1} )", vb.BeginDate.ToString("dd/MM/yyyy"), vb.EndDate.ToString("dd/MM/yyyy")),
                Amount = vb.Amount,
                AmountInWords = ServiceCommon.NumberToWords(vb.Amount),
                UserCreatedViewModel = _DataContext.Users.AsNoTracking().Select(u => new UserViewModel
                {
                    Id = u.Id.ToString(),
                    UserName = u.UserName,
                    FullName = u.FullName
                }).FirstOrDefault(x => x.UserName == vb.CreatedBy),
                Cashier = vb.Cashier,
                PaymentMethod = vb.PaymentMethod == 1 ? "Tiền mặt" : vb.PaymentMethod == 2 ? "Chuyển khoản" : "Khác"
            }).FirstOrDefaultAsync(x => x.Id == id);
            if (mb == null)
            {
                return new byte[0];
            }
            else
            {
                try
                {
                    var document = new ReceiptDocument(mb);
                    return document.GeneratePdf();
                }
                catch (Exception ex)
                {
                    return new byte[0];
                }
            }
        }

        public async Task<ResultCommon> UpdateVehicleBillById(VehicleBillViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.VehicleBills.FirstOrDefaultAsync(x => x.Id.ToString() == entity.Id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại phiếu thu phí gửi xe trên hệ thống!");
            }
            else
            {
                var duplicate = await _DataContext.VehicleBills.AsNoTracking().Select(mb => new VehicleBillViewModel
                {
                    Id = mb.Id.ToString(),
                    BuildingId = mb.BuildingId.ToString(),
                    BuildingViewModel = _DataContext.Buildings.AsNoTracking().Where(x => x.Id == mb.BuildingId).Select(b => new BuildingViewModel
                    {
                        Id = b.Id.ToString(),
                        Name = b.Name,
                        Code = b.Code
                    }).FirstOrDefault(),
                    ApartmentId = mb.ApartmentId.ToString(),
                    ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Where(x => x.Id == mb.ApartmentId).Select(a => new ApartmentViewModel
                    {
                        Id = a.Id.ToString(),
                        Number = a.Number
                    }).FirstOrDefault(),
                    BeginDate = mb.BeginDate,
                    EndDate = mb.EndDate
                }).FirstOrDefaultAsync(x => x.Id != entity.Id && x.BuildingId == entity.BuildingId && x.ApartmentId == entity.ApartmentId && !(entity.EndDate < x.BeginDate || entity.BeginDate > x.EndDate));
                if (duplicate == null)
                {
                    existed.BuildingId = Guid.Parse(entity.BuildingId);
                    existed.ApartmentId = Guid.Parse(entity.ApartmentId);
                    existed.BeginDate = entity.BeginDate.Value;
                    existed.EndDate = entity.EndDate.Value;
                    existed.Amount = entity.Amount.Value;
                    existed.IsPay = entity.IsPay;
                    existed.Payer = entity.Payer;
                    existed.PaymentDate = entity.PaymentDate;
                    existed.Cashier = entity.Cashier;
                    existed.PaymentMethod = entity.PaymentMethod;

                    _DataContext.VehicleBills.Update(existed);
                    if (await _DataContext.SaveChangesAsync() > 0)
                    {
                        result.Status = 1;
                        result.Message = string.Format("Cập nhật phiếu thu phí gửi xe thành công");
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = string.Format("Cập nhật phiếu thu phí gửi xe không thành công!");
                    }
                }
                else
                {
                    result.Status = -1;
                    result.Message = string.Format("Đã tồn tại phiếu thu phí gửi xe căn hộ: {0} - tòa nhà: {1} trong khoảng thời gian từ ngày: {2} đến ngày: {3} trên hệ thống!",
                    duplicate.ApartmentViewModel?.Number, duplicate.BuildingViewModel?.Name, duplicate.BeginDate.Value.ToString("dd/MM/yyyy"), duplicate.EndDate.Value.ToString("dd/MM/yyyy"));
                }
            }

            return result;
        }
    }
}
