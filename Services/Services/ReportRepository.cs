using API.Common;
using API.Services.Interfaces;
using API.Services.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace API.Services.Services
{
    public class ReportRepository : IReportRepository
    {
        private DataContext _DataContext;
        private IConfiguration _IConfiguration;

        public ReportRepository(DataContext DataContext, IConfiguration IConfiguration)
        {
            _DataContext = DataContext;
            _IConfiguration = IConfiguration;
        }

        public async Task<byte[]> PrintPDFWarningReport(WarningModel model)
        {
            if (model == null)
            {
                return new byte[0];
            }
            else
            {
                try
                {
                    var document = new WarningDocument(model);
                    return document.GeneratePdf();
                }
                catch (Exception ex)
                {
                    return new byte[0];
                }
            }
        }

        public async Task<RevenueModel> RevenueReport(RevenueModel model)
        {
            // thu phí quản lý
            var queryManagement = _DataContext.ManagementBills.Where(x => x.IsPay == true && x.PaymentDate >= model.BeginDate && x.PaymentDate <= model.EndDate).AsNoTracking()
                .Select(mb => new ManagementBillViewModel
                {
                    Id = mb.Id.ToString(),
                    Amount = mb.Amount,
                    IsPay = mb.IsPay,
                    PaymentDate = mb.PaymentDate,
                    PaymentMethod = mb.PaymentMethod
                });
            model.ManagementAmount = await queryManagement.SumAsync(x => x.Amount);
            model.ManagementNumber = await queryManagement.CountAsync();

            // thu tiền nước
            var queryWater = _DataContext.WaterBills.Where(x => x.IsPay == true && x.PaymentDate >= model.BeginDate && x.PaymentDate <= model.EndDate).AsNoTracking()
                .Select(wb => new WaterBillViewModel
                {
                    Id = wb.Id.ToString(),
                    FeedWaterFee = wb.FeedWaterFee,
                    WasteWaterFee = wb.WasteWaterFee,
                    IsPay = wb.IsPay,
                    PaymentDate = wb.PaymentDate,
                    PaymentMethod = wb.PaymentMethod
                });
            model.WaterAmount = await queryWater.SumAsync(x => (x.FeedWaterFee + x.WasteWaterFee));
            model.WaterNumber = await queryWater.CountAsync();

            // thu phí gửi xe
            var queryVehicle = _DataContext.VehicleBills.Where(x => x.IsPay == true && x.PaymentDate >= model.BeginDate && x.PaymentDate <= model.EndDate).AsNoTracking()
                .Select(mb => new VehicleBillViewModel
                {
                    Id = mb.Id.ToString(),
                    Amount = mb.Amount,
                    IsPay = mb.IsPay,
                    PaymentDate = mb.PaymentDate,
                    PaymentMethod = mb.PaymentMethod
                });
            model.VehicleAmount = await queryVehicle.SumAsync(x => x.Amount);
            model.VehicleNumber = await queryVehicle.CountAsync();

            // thu phí khác
            var queryOther = _DataContext.OtherBills.Where(x => x.IsPay == true && x.PaymentDate >= model.BeginDate && x.PaymentDate <= model.EndDate).AsNoTracking()
                .Select(mb => new OtherBillViewModel
                {
                    Id = mb.Id.ToString(),
                    Amount = mb.Amount,
                    IsPay = mb.IsPay,
                    PaymentDate = mb.PaymentDate,
                    PaymentMethod = mb.PaymentMethod
                });
            model.OtherAmount = await queryOther.SumAsync(x => x.Amount);

            model.TotalAmount = model.ManagementAmount + model.WaterAmount + model.VehicleAmount + model.OtherAmount;

            return model;
        }

        public async Task<PagingData> WarningReport(PagingData page)
        {
            int.TryParse(page.ListSearchData.FirstOrDefault(x => x.ColSearch == "Month")?.ValueSearch, out int month);
            int.TryParse(page.ListSearchData.FirstOrDefault(x => x.ColSearch == "Year")?.ValueSearch, out int year);
            var targetDateBegin = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            var targetDateEnd = new DateTime(year, month, 1);
            var query = _DataContext.Apartments.Select(wm => new WarningModel
            {
                Id = wm.Id.ToString(),
                Month = month,
                Year = year,
                BuildingId = wm.BuildingId.ToString(),
                BuildingViewModel = _DataContext.Buildings.AsNoTracking().Where(x => x.Id == wm.BuildingId).Select(b => new BuildingViewModel
                {
                    Id = b.Id.ToString(),
                    Name = b.Name,
                    Code = b.Code
                }).FirstOrDefault(),
                ApartmentId = wm.Id.ToString(),
                ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Where(x => x.Id == wm.Id).Select(a => new ApartmentViewModel
                {
                    Id = a.Id.ToString(),
                    Number = a.Number,
                    Owner = a.Owner,
                    ManageFee = a.ManageFee,
                    ListVehicleFeeViewModel = _DataContext.VehicleFees.AsNoTracking().Where(x => x.BuildingId == a.BuildingId && x.ApartmentId == a.Id)
                    .Select(vf => new VehicleFeeViewModel
                    {
                        Id = vf.Id.ToString(),
                        TypeName = vf.Type == 1 ? "Xe đạp" : vf.Type == 2 ? "Xe điện" : vf.Type == 3 ? "Xe máy" : vf.Type == 4 ? "Ô tô" : "Loại khác",
                        License = vf.License,
                        Price = vf.Price
                    }).ToList(),
                }).FirstOrDefault(),
                Owner = wm.Owner,
                ManagementFeeForward = _DataContext.ManagementBills.AsNoTracking().Where(x => x.BuildingId == wm.BuildingId && x.ApartmentId == wm.Id && x.EndDate < targetDateEnd && x.IsPay != true)
                .Sum(x => x.Amount),
                ManagementFee = _DataContext.ManagementBills.AsNoTracking().Where(x => x.BuildingId == wm.BuildingId && x.ApartmentId == wm.Id
                    && x.BeginDate <= targetDateBegin && x.EndDate >= targetDateEnd && x.IsPay != true)
                .Sum(x => x.Amount),
                ManagementBillViewModel = _DataContext.ManagementBills.AsNoTracking().Where(x => x.BuildingId == wm.BuildingId && x.ApartmentId == wm.Id
                    && x.BeginDate <= targetDateBegin && x.EndDate >= targetDateEnd && x.IsPay != true)
                .Select(mb => new ManagementBillViewModel
                {
                    Id = mb.Id.ToString(),
                    No = mb.No,
                    Amount = mb.Amount,
                    BillingCycle = string.Format("{0} - {1}", mb.BeginDate.ToString("dd/MM/yyyy"), mb.EndDate.ToString("dd/MM/yyyy"))
                }).ToList(),
                VehicleFeeForward = _DataContext.VehicleBills.AsNoTracking().Where(x => x.BuildingId == wm.BuildingId && x.ApartmentId == wm.Id && x.EndDate < targetDateEnd && x.IsPay != true)
                .Sum(x => x.Amount),
                VehicleFee = _DataContext.VehicleBills.AsNoTracking().Where(x => x.BuildingId == wm.BuildingId && x.ApartmentId == wm.Id
                    && x.BeginDate <= targetDateBegin && x.EndDate >= targetDateEnd && x.IsPay != true)
                .Sum(x => x.Amount),
                VehicleBillViewModel = _DataContext.VehicleBills.AsNoTracking().Where(x => x.BuildingId == wm.BuildingId && x.ApartmentId == wm.Id
                    && x.BeginDate <= targetDateBegin && x.EndDate >= targetDateEnd && x.IsPay != true)
                .Select(vb => new VehicleBillViewModel
                {
                    Id = vb.Id.ToString(),
                    No = vb.No,
                    Amount = vb.Amount,
                    BillingCycle = string.Format("{0} - {1}", vb.BeginDate.ToString("dd/MM/yyyy"), vb.EndDate.ToString("dd/MM/yyyy"))
                }).ToList(),
                WaterFeeForward = _DataContext.WaterBills.AsNoTracking().Where(x => x.BuildingId == wm.BuildingId && x.ApartmentId == wm.Id
                    && (x.Year < year || (x.Year == year && x.Month < month)) && x.IsPay != true)
                .Sum(x => (x.FeedWaterFee + x.WasteWaterFee)),
                WaterFee = _DataContext.WaterBills.AsNoTracking().Where(x => x.BuildingId == wm.BuildingId && x.ApartmentId == wm.Id && x.Month == month && x.Year == year && x.IsPay != true)
                .Sum(x => (x.FeedWaterFee + x.WasteWaterFee)),
                WaterBillViewModel = _DataContext.WaterBills.AsNoTracking().Where(x => x.BuildingId == wm.BuildingId && x.ApartmentId == wm.Id && x.Month == month && x.Year == year && x.IsPay != true)
                .Select(wb => new WaterBillViewModel
                {
                    Id = wb.Id.ToString(),
                    No = wb.No,
                    BeginNumber = wb.BeginNumber,
                    EndNumber = wb.EndNumber,
                    FeedWaterFee = wb.FeedWaterFee,
                    WasteWaterFee = wb.WasteWaterFee,
                    BillingCycle = string.Format("Tháng {0}/{1}", wb.Month, wb.Year)
                }).ToList(),
                ManagementBillForward = _DataContext.ManagementBills.AsNoTracking().Where(x => x.BuildingId == wm.BuildingId && x.ApartmentId == wm.Id && x.EndDate < targetDateEnd && x.IsPay != true)
                .Select(mb => new FeeForward
                {
                    Id = mb.Id.ToString(),
                    No = mb.No,
                    Amount = mb.Amount,
                    BillingCycle = string.Format("{0} - {1}", mb.BeginDate.ToString("dd/MM/yyyy"), mb.EndDate.ToString("dd/MM/yyyy"))
                }).ToList(),
                VehicleBillForward = _DataContext.VehicleBills.AsNoTracking().Where(x => x.BuildingId == wm.BuildingId && x.ApartmentId == wm.Id && x.EndDate < targetDateEnd && x.IsPay != true)
                .Select(vb => new FeeForward
                {
                    Id = vb.Id.ToString(),
                    No = vb.No,
                    Amount = vb.Amount,
                    BillingCycle = string.Format("{0} - {1}", vb.BeginDate.ToString("dd/MM/yyyy"), vb.EndDate.ToString("dd/MM/yyyy"))
                }).ToList(),
                WaterBillForward = _DataContext.WaterBills.AsNoTracking().Where(x => x.BuildingId == wm.BuildingId && x.ApartmentId == wm.Id && (x.Year < year || (x.Year == year && x.Month < month)) && x.IsPay != true)
                .Select(wb => new FeeForward
                {
                    Id = wb.Id.ToString(),
                    No = wb.No,
                    Amount = wb.FeedWaterFee + wb.WasteWaterFee,
                    BillingCycle = string.Format("Tháng {0}/{1}", wb.Month, wb.Year)
                }).ToList(),
                ManageName = _IConfiguration["Information:ManageName"],
                HostlineAdmin = _IConfiguration["Information:Warning:HostlineAdmin"],
                HostlineTech = _IConfiguration["Information:Warning:HostlineTech"],
                Deadline = _IConfiguration["Information:Warning:Deadline"],
                BankNumber = _IConfiguration["Information:Warning:BankNumber"],
                BankOwner = _IConfiguration["Information:Warning:BankOwner"],
                BankName = _IConfiguration["Information:Warning:BankName"],
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
                    default:
                        break;
                }
            }

            query = query.Where(x => x.ManagementFeeForward > 0 || x.VehicleFeeForward > 0 || x.WaterFeeForward > 0 || x.ManagementFee > 0 || x.VehicleFee > 0 || x.WaterFee > 0)
                .OrderBy(x => x.BuildingViewModel.Name).OrderBy(x => x.ApartmentViewModel.Number).OrderByDescending(x => x.Year).OrderByDescending(x => x.Month);

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
    }
}
