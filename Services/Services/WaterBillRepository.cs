using API.Common;
using API.Entities;
using API.Services.Interfaces;
using API.Services.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Security.Claims;

namespace API.Services.Services
{
    public class WaterBillRepository : IWaterBillRepository
    {
        private IMapper _IMapper;
        private DataContext _DataContext;
        private IConfiguration _IConfiguration;

        public WaterBillRepository(IMapper IMapper, DataContext DataContext, IConfiguration IConfiguration)
        {
            _IMapper = IMapper;
            _DataContext = DataContext;
            _IConfiguration = IConfiguration;

        }

        public async Task<ResultCommon> AddWaterBill(WaterBillViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.WaterBills.AsNoTracking().Select(wb => new WaterBillViewModel
            {
                Id = wb.Id.ToString(),
                Month = wb.Month,
                Year = wb.Year,
                BuildingId = wb.BuildingId.ToString(),
                BuildingViewModel = _DataContext.Buildings.AsNoTracking().Select(b => new BuildingViewModel
                {
                    Id = b.Id.ToString(),
                    Name = b.Name,
                    Code = b.Code
                }).FirstOrDefault(x => x.Id == wb.BuildingId.ToString()),
                ApartmentId = wb.ApartmentId.ToString(),
                ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Select(a => new ApartmentViewModel
                {
                    Id = a.Id.ToString(),
                    Number = a.Number
                }).FirstOrDefault(x => x.Id == wb.ApartmentId.ToString()),
            }).FirstOrDefaultAsync(x => x.Month == entity.Month && x.Year == entity.Year && x.BuildingId == entity.BuildingId && x.ApartmentId == entity.ApartmentId);
            if (existed == null)
            {
                entity.Id = Guid.NewGuid().ToString();
                var waterBill = _IMapper.Map<WaterBill>(entity);
                var lastReceipt = await _DataContext.WaterBills.OrderByDescending(r => r.CreatedDate).FirstOrDefaultAsync();
                waterBill.No = $"PTN{ServiceCommon.GenerateReceiptNo(lastReceipt == null ? "" : lastReceipt.No)}";
                await _DataContext.WaterBills.AddAsync(waterBill);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Thêm phiếu thu tiền nước thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Thêm phiếu thu tiền nước không thành công!");
                }
            }
            else
            {
                result.Status = -1;
                result.Message = string.Format("Đã tồn tại phiếu thu tiền nước tháng: {0}/{1} của căn hộ: {2} - tòa nhà: {3} trên hệ thống!", existed.Month, existed.Year, existed.ApartmentViewModel?.Number,
                    existed.BuildingViewModel?.Name);
            }

            return result;
        }

        public async Task<ResultCommon> DeleteWaterBillById(string id)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.WaterBills.FirstOrDefaultAsync(x => x.Id.ToString() == id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại phiếu thu tiền nước trên hệ thống!");
            }
            else
            {
                _DataContext.WaterBills.Remove(existed);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Xóa phiếu thu tiền nước thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Xóa phiếu thu tiền nước không thành công!");
                }
            }

            return result;
        }

        public async Task<PagingData> GetAllWaterBill(PagingData page)
        {
            var query = _DataContext.WaterBills.AsNoTracking()
                .Select(wb => new WaterBillViewModel
                {
                    Id = wb.Id.ToString(),
                    No = wb.No,
                    Year = wb.Year,
                    Month = wb.Month,
                    BuildingId = wb.BuildingId.ToString(),
                    BuildingViewModel = _DataContext.Buildings.AsNoTracking().Select(b => new BuildingViewModel
                    {
                        Id = b.Id.ToString(),
                        Name = b.Name,
                        Code = b.Code
                    }).FirstOrDefault(x => x.Id == wb.BuildingId.ToString()),
                    ApartmentId = wb.ApartmentId.ToString(),
                    ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Select(a => new ApartmentViewModel
                    {
                        Id = a.Id.ToString(),
                        Number = a.Number,
                        Owner = a.Owner
                    }).FirstOrDefault(x => x.Id == wb.ApartmentId.ToString()),
                    BeginNumber = wb.BeginNumber,
                    EndNumber = wb.EndNumber,
                    FeedWaterFee = wb.FeedWaterFee,
                    WasteWaterFee = wb.WasteWaterFee,
                    IsPay = wb.IsPay,
                    Payer = wb.Payer,
                    PaymentDate = wb.PaymentDate,
                    Cashier = wb.Cashier,
                    PaymentMethod = wb.PaymentMethod
                });

            foreach (var s in page.ListSearchData)
            {
                switch (s.ColSearch)
                {
                    case "Month":
                        query = query.Where(x => x.Month.ToString() == s.ValueSearch);
                        break;
                    case "Year":
                        query = query.Where(x => x.Year.ToString() == s.ValueSearch);
                        break;
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
            query = query.OrderByDescending(x => x.Year).OrderByDescending(x => x.Month).OrderBy(x => x.BuildingViewModel.Name).OrderBy(x => x.ApartmentViewModel.Number);

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

        public async Task<byte[]> PrintWaterBill(string id)
        {
            var mb = await _DataContext.WaterBills.AsNoTracking().Select(wb => new ReceiptModel
            {
                Id = wb.Id.ToString(),
                CompanyName = _IConfiguration["Information:CompanyName"],
                CompanyAddress = _IConfiguration["Information:CompanyAddress"],
                TemplateNo = _IConfiguration["Information:Receipt:TemplateNo"],
                Decision = _IConfiguration["Information:Receipt:Decision"],
                PlaceOfDecision = _IConfiguration["Information:Receipt:PlaceOfDecision"],
                ReceiptCode = wb.No,
                ReceiptDate = wb.PaymentDate,
                CustomerName = wb.Payer,
                BuildingViewModel = _DataContext.Buildings.AsNoTracking().Select(b => new BuildingViewModel
                {
                    Id = b.Id.ToString(),
                    Name = b.Name,
                    Code = b.Code
                }).FirstOrDefault(x => x.Id == wb.BuildingId.ToString()),
                ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Select(a => new ApartmentViewModel
                {
                    Id = a.Id.ToString(),
                    Number = a.Number
                }).FirstOrDefault(x => x.Id == wb.ApartmentId.ToString()),
                Content = string.Format("Thu tiền nước tháng {0} năm {1}", wb.Month, wb.Year),
                Amount = wb.FeedWaterFee + wb.WasteWaterFee,
                AmountInWords = ServiceCommon.NumberToWords(wb.FeedWaterFee + wb.WasteWaterFee),
                UserCreatedViewModel = _DataContext.Users.AsNoTracking().Select(u => new UserViewModel
                {
                    Id = u.Id.ToString(),
                    UserName = u.UserName,
                    FullName = u.FullName
                }).FirstOrDefault(x => x.UserName == wb.CreatedBy),
                Cashier = wb.Cashier,
                PaymentMethod = wb.PaymentMethod == 1 ? "Tiền mặt" : wb.PaymentMethod == 2 ? "Chuyển khoản" : "Khác"
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

        public async Task<ResultCommon> UpdateWaterBillById(WaterBillViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.WaterBills.FirstOrDefaultAsync(x => x.Id.ToString() == entity.Id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại phiếu thu tiền nước trên hệ thống!");
            }
            else
            {
                var duplicate = await _DataContext.WaterBills.AsNoTracking().Select(wb => new WaterBillViewModel
                {
                    Id = wb.Id.ToString(),
                    Month = wb.Month,
                    Year = wb.Year,
                    BuildingId = wb.BuildingId.ToString(),
                    BuildingViewModel = _DataContext.Buildings.AsNoTracking().Select(b => new BuildingViewModel
                    {
                        Id = b.Id.ToString(),
                        Name = b.Name,
                        Code = b.Code
                    }).FirstOrDefault(x => x.Id == wb.BuildingId.ToString()),
                    ApartmentId = wb.ApartmentId.ToString(),
                    ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Select(a => new ApartmentViewModel
                    {
                        Id = a.Id.ToString(),
                        Number = a.Number
                    }).FirstOrDefault(x => x.Id == wb.ApartmentId.ToString()),
                }).FirstOrDefaultAsync(x => x.Id != entity.Id && x.Month == entity.Month && x.Year == entity.Year && x.BuildingId == entity.BuildingId && x.ApartmentId == entity.ApartmentId);
                if (duplicate == null)
                {
                    existed.Month = entity.Month.Value;
                    existed.Year = entity.Year.Value;
                    existed.BuildingId = Guid.Parse(entity.BuildingId);
                    existed.ApartmentId = Guid.Parse(entity.ApartmentId);
                    existed.BeginNumber = entity.BeginNumber.Value;
                    existed.EndNumber = entity.EndNumber.Value;
                    existed.FeedWaterFee = entity.FeedWaterFee.Value;
                    existed.WasteWaterFee = entity.WasteWaterFee.Value;
                    existed.IsPay = entity.IsPay;
                    existed.Payer = entity.Payer;
                    existed.PaymentDate = entity.PaymentDate;
                    existed.Cashier = entity.Cashier;
                    existed.PaymentMethod = entity.PaymentMethod;

                    _DataContext.WaterBills.Update(existed);
                    if (await _DataContext.SaveChangesAsync() > 0)
                    {
                        result.Status = 1;
                        result.Message = string.Format("Cập nhật phiếu thu tiền nước thành công");
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = string.Format("Cập nhật phiếu thu tiền nước không thành công!");
                    }
                }
                else
                {
                    result.Status = -1;
                    result.Message = string.Format("Đã tồn tại phiếu thu tiền nước: tháng: {0}/{1} của căn hộ: {2} - tòa nhà: {3} trên hệ thống!", duplicate.Month, duplicate.Year, duplicate.ApartmentViewModel?.Number,
                    duplicate.BuildingViewModel?.Name);
                }
            }

            return result;
        }
    }
}
