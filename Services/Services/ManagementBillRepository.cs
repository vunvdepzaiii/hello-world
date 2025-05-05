using API.Common;
using API.Entities;
using API.Services.Interfaces;
using API.Services.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;

namespace API.Services.Services
{
    public class ManagementBillRepository : IManagementBillRepository
    {
        private IMapper _IMapper;
        private DataContext _DataContext;
        private IConfiguration _IConfiguration;

        public ManagementBillRepository(IMapper IMapper, DataContext DataContext, IConfiguration IConfiguration)
        {
            _IMapper = IMapper;
            _DataContext = DataContext;
            _IConfiguration = IConfiguration;
        }

        public async Task<ResultCommon> AddManagementBill(ManagementBillViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.ManagementBills.AsNoTracking().Select(mb => new ManagementBillViewModel
            {
                Id = mb.Id.ToString(),
                BuildingId = mb.BuildingId.ToString(),
                BuildingViewModel = _DataContext.Buildings.AsNoTracking().Select(b => new BuildingViewModel
                {
                    Id = b.Id.ToString(),
                    Name = b.Name,
                    Code = b.Code
                }).FirstOrDefault(x => x.Id == mb.BuildingId.ToString()),
                ApartmentId = mb.ApartmentId.ToString(),
                ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Select(a => new ApartmentViewModel
                {
                    Id = a.Id.ToString(),
                    Number = a.Number
                }).FirstOrDefault(x => x.Id == mb.ApartmentId.ToString()),
                BeginDate = mb.BeginDate,
                EndDate = mb.EndDate
            }).FirstOrDefaultAsync(x => x.BuildingId == entity.BuildingId && x.ApartmentId == entity.ApartmentId && !(entity.EndDate < x.BeginDate || entity.BeginDate > x.EndDate));
            if (existed == null)
            {
                entity.Id = Guid.NewGuid().ToString();
                var managementBill = _IMapper.Map<ManagementBill>(entity);
                var lastReceipt = await _DataContext.ManagementBills.OrderByDescending(r => r.CreatedDate).FirstOrDefaultAsync();
                managementBill.No = $"PTQL{ServiceCommon.GenerateReceiptNo(lastReceipt == null ? "" : lastReceipt.No)}";
                await _DataContext.ManagementBills.AddAsync(managementBill);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Thêm phiếu thu phí quản lý thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Thêm phiếu thu phí quản lý không thành công!");
                }
            }
            else
            {
                result.Status = -1;
                result.Message = string.Format("Đã tồn tại phiếu thu phí quản lý căn hộ: {0} - tòa nhà: {1} trong khoảng thời gian từ ngày: {2} đến ngày: {3} trên hệ thống!",
                    existed.ApartmentViewModel?.Number, existed.BuildingViewModel?.Name, existed.BeginDate.Value.ToString("dd/MM/yyyy"), existed.EndDate.Value.ToString("dd/MM/yyyy"));
            }

            return result;
        }

        public async Task<ResultCommon> DeleteManagementBillById(string id)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.ManagementBills.FirstOrDefaultAsync(x => x.Id.ToString() == id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại phiếu thu phí quản lý trên hệ thống!");
            }
            else
            {
                _DataContext.ManagementBills.Remove(existed);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Xóa phiếu thu phí quản lý thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Xóa phiếu thu phí quản lý không thành công!");
                }
            }

            return result;
        }

        public async Task<PagingData> GetAllManagementBill(PagingData page)
        {
            var query = _DataContext.ManagementBills.AsNoTracking()
                .Select(mb => new ManagementBillViewModel
                {
                    Id = mb.Id.ToString(),
                    No = mb.No,
                    BuildingId = mb.BuildingId.ToString(),
                    BuildingViewModel = _DataContext.Buildings.AsNoTracking().Select(b => new BuildingViewModel
                    {
                        Id = b.Id.ToString(),
                        Name = b.Name,
                        Code = b.Code
                    }).FirstOrDefault(x => x.Id == mb.BuildingId.ToString()),
                    ApartmentId = mb.ApartmentId.ToString(),
                    ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Select(a => new ApartmentViewModel
                    {
                        Id = a.Id.ToString(),
                        Number = a.Number,
                        Owner = a.Owner,
                        ManageFee = a.ManageFee
                    }).FirstOrDefault(x => x.Id == mb.ApartmentId.ToString()),
                    BeginDate = mb.BeginDate,
                    EndDate = mb.EndDate,
                    Amount = mb.Amount,
                    IsPay = mb.IsPay,
                    Payer = mb.Payer,
                    PaymentDate = mb.PaymentDate,
                    Cashier = mb.Cashier,
                    PaymentMethod = mb.PaymentMethod
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

        public async Task<byte[]> PrintManagementBill(string id)
        {
            var mb = await _DataContext.ManagementBills.AsNoTracking().Select(mb => new ReceiptModel
            {
                Id = mb.Id.ToString(),
                CompanyName = _IConfiguration["Information:CompanyName"],
                CompanyAddress = _IConfiguration["Information:CompanyAddress"],
                TemplateNo = _IConfiguration["Information:Receipt:TemplateNo"],
                Decision = _IConfiguration["Information:Receipt:Decision"],
                PlaceOfDecision = _IConfiguration["Information:Receipt:PlaceOfDecision"],
                ReceiptCode = mb.No,
                ReceiptDate = mb.PaymentDate,
                CustomerName = mb.Payer,
                BuildingViewModel = _DataContext.Buildings.AsNoTracking().Select(b => new BuildingViewModel
                {
                    Id = b.Id.ToString(),
                    Name = b.Name,
                    Code = b.Code
                }).FirstOrDefault(x => x.Id == mb.BuildingId.ToString()),
                ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Select(a => new ApartmentViewModel
                {
                    Id = a.Id.ToString(),
                    Number = a.Number
                }).FirstOrDefault(x => x.Id == mb.ApartmentId.ToString()),
                Content = string.Format("Thu phí quản lý ( từ {0} đến {1} )", mb.BeginDate.ToString("dd/MM/yyyy"), mb.EndDate.ToString("dd/MM/yyyy")),
                Amount = mb.Amount,
                AmountInWords = ServiceCommon.NumberToWords(mb.Amount),
                UserCreatedViewModel = _DataContext.Users.AsNoTracking().Select(u => new UserViewModel
                {
                    Id = u.Id.ToString(),
                    UserName = u.UserName,
                    FullName = u.FullName
                }).FirstOrDefault(x => x.UserName == mb.CreatedBy),
                Cashier = mb.Cashier,
                PaymentMethod = mb.PaymentMethod == 1 ? "Tiền mặt" : mb.PaymentMethod == 2 ? "Chuyển khoản" : "Khác"
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

        public async Task<ResultCommon> UpdateManagementBillById(ManagementBillViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.ManagementBills.FirstOrDefaultAsync(x => x.Id.ToString() == entity.Id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại phiếu thu phí quản lý trên hệ thống!");
            }
            else
            {
                var duplicate = await _DataContext.ManagementBills.AsNoTracking().Select(mb => new ManagementBillViewModel
                {
                    Id = mb.Id.ToString(),
                    BuildingId = mb.BuildingId.ToString(),
                    BuildingViewModel = _DataContext.Buildings.AsNoTracking().Select(b => new BuildingViewModel
                    {
                        Id = b.Id.ToString(),
                        Name = b.Name,
                        Code = b.Code
                    }).FirstOrDefault(x => x.Id == mb.BuildingId.ToString()),
                    ApartmentId = mb.ApartmentId.ToString(),
                    ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Select(a => new ApartmentViewModel
                    {
                        Id = a.Id.ToString(),
                        Number = a.Number
                    }).FirstOrDefault(x => x.Id == mb.ApartmentId.ToString()),
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

                    _DataContext.ManagementBills.Update(existed);
                    if (await _DataContext.SaveChangesAsync() > 0)
                    {
                        result.Status = 1;
                        result.Message = string.Format("Cập nhật phiếu thu phí quản lý thành công");
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = string.Format("Cập nhật phiếu thu phí quản lý không thành công!");
                    }
                }
                else
                {
                    result.Status = -1;
                    result.Message = string.Format("Đã tồn tại phiếu thu phí quản lý căn hộ: {0} - tòa nhà: {1} trong khoảng thời gian từ ngày: {2} đến ngày: {3} trên hệ thống!",
                    duplicate.ApartmentViewModel?.Number, duplicate.BuildingViewModel?.Name, duplicate.BeginDate.Value.ToString("dd/MM/yyyy"), duplicate.EndDate.Value.ToString("dd/MM/yyyy"));
                }
            }

            return result;
        }
    }
}
