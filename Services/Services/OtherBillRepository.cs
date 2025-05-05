using API.Common;
using API.Entities;
using API.Services.Interfaces;
using API.Services.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace API.Services.Services
{
    public class OtherBillRepository : IOtherBillRepository
    {
        private IMapper _IMapper;
        private DataContext _DataContext;
        private IConfiguration _IConfiguration;

        public OtherBillRepository(IMapper IMapper, DataContext DataContext, IConfiguration IConfiguration)
        {
            _IMapper = IMapper;
            _DataContext = DataContext;
            _IConfiguration = IConfiguration;
        }

        public async Task<ResultCommon> AddOtherBill(OtherBillViewModel entity)
        {
            var result = new ResultCommon();

            entity.Id = Guid.NewGuid().ToString();
            var otherBill = _IMapper.Map<OtherBill>(entity);
            var lastReceipt = await _DataContext.OtherBills.OrderByDescending(r => r.CreatedDate).FirstOrDefaultAsync();
            otherBill.No = $"PTK{ServiceCommon.GenerateReceiptNo(lastReceipt == null ? "" : lastReceipt.No)}";
            await _DataContext.OtherBills.AddAsync(otherBill);
            if (await _DataContext.SaveChangesAsync() > 0)
            {
                result.Status = 1;
                result.Message = string.Format("Thêm phiếu thu khác thành công");
            }
            else
            {
                result.Status = 0;
                result.Message = string.Format("Thêm phiếu thu khác không thành công!");
            }

            return result;
        }

        public async Task<ResultCommon> DeleteOtherBillById(string id)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.OtherBills.FirstOrDefaultAsync(x => x.Id.ToString() == id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại phiếu thu khác trên hệ thống!");
            }
            else
            {
                _DataContext.OtherBills.Remove(existed);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Xóa phiếu thu khác thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Xóa phiếu thu khác không thành công!");
                }
            }

            return result;
        }

        public async Task<PagingData> GetAllOtherBill(PagingData page)
        {
            var query = _DataContext.OtherBills.AsNoTracking()
                .Select(ob => new OtherBillViewModel
                {
                    Id = ob.Id.ToString(),
                    No = ob.No,
                    BuildingId = ob.BuildingId.ToString(),
                    BuildingViewModel = _DataContext.Buildings.AsNoTracking().Where(x => x.Id == ob.BuildingId).Select(b => new BuildingViewModel
                    {
                        Id = b.Id.ToString(),
                        Name = b.Name,
                        Code = b.Code
                    }).FirstOrDefault(),
                    ApartmentId = ob.ApartmentId.ToString(),
                    ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Where(x => x.Id == ob.ApartmentId).Select(a => new ApartmentViewModel
                    {
                        Id = a.Id.ToString(),
                        Number = a.Number,
                        Owner = a.Owner,
                    }).FirstOrDefault(),
                    Content = ob.Content,
                    Amount = ob.Amount,
                    IsPay = ob.IsPay,
                    Payer = ob.Payer,
                    PaymentDate = ob.PaymentDate,
                    Cashier = ob.Cashier,
                    PaymentMethod = ob.PaymentMethod
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
                    case "Content":
                        query = query.Where(x => x.Content.Contains(s.ValueSearch));
                        break;
                    default:
                        break;
                }
            }
            query = query.OrderByDescending(x => x.No);

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

        public async Task<byte[]> PrintOtherBill(string id)
        {
            var mb = await _DataContext.OtherBills.AsNoTracking().Select(ob => new ReceiptModel
            {
                Id = ob.Id.ToString(),
                CompanyName = _IConfiguration["Information:CompanyName"],
                CompanyAddress = _IConfiguration["Information:CompanyAddress"],
                TemplateNo = _IConfiguration["Information:Receipt:TemplateNo"],
                Decision = _IConfiguration["Information:Receipt:Decision"],
                PlaceOfDecision = _IConfiguration["Information:Receipt:PlaceOfDecision"],
                ReceiptCode = ob.No,
                ReceiptDate = ob.PaymentDate,
                CustomerName = ob.Payer,
                BuildingViewModel = _DataContext.Buildings.AsNoTracking().Select(b => new BuildingViewModel
                {
                    Id = b.Id.ToString(),
                    Name = b.Name,
                    Code = b.Code
                }).FirstOrDefault(x => x.Id == ob.BuildingId.ToString()),
                ApartmentViewModel = _DataContext.Apartments.AsNoTracking().Select(a => new ApartmentViewModel
                {
                    Id = a.Id.ToString(),
                    Number = a.Number
                }).FirstOrDefault(x => x.Id == ob.ApartmentId.ToString()),
                Content = ob.Content,
                Amount = ob.Amount,
                AmountInWords = ServiceCommon.NumberToWords(ob.Amount),
                UserCreatedViewModel = _DataContext.Users.AsNoTracking().Select(u => new UserViewModel
                {
                    Id = u.Id.ToString(),
                    UserName = u.UserName,
                    FullName = u.FullName
                }).FirstOrDefault(x => x.UserName == ob.CreatedBy),
                Cashier = ob.Cashier,
                PaymentMethod = ob.PaymentMethod == 1 ? "Tiền mặt" : ob.PaymentMethod == 2 ? "Chuyển khoản" : "Khác"
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

        public async Task<ResultCommon> UpdateOtherBillById(OtherBillViewModel entity)
        {
            var result = new ResultCommon();
            var existed = await _DataContext.OtherBills.FirstOrDefaultAsync(x => x.Id.ToString() == entity.Id);
            if (existed == null)
            {
                result.Status = -1;
                result.Message = string.Format("Không tồn tại phiếu thu khác trên hệ thống!");
            }
            else
            {
                existed.BuildingId = Guid.Parse(entity.BuildingId);
                existed.ApartmentId = Guid.Parse(entity.ApartmentId);
                existed.Content = entity.Content;
                existed.Amount = entity.Amount.Value;
                existed.IsPay = entity.IsPay;
                existed.Payer = entity.Payer;
                existed.PaymentDate = entity.PaymentDate;
                existed.Cashier = entity.Cashier;
                existed.PaymentMethod = entity.PaymentMethod;

                _DataContext.OtherBills.Update(existed);
                if (await _DataContext.SaveChangesAsync() > 0)
                {
                    result.Status = 1;
                    result.Message = string.Format("Cập nhật phiếu thu khác thành công");
                }
                else
                {
                    result.Status = 0;
                    result.Message = string.Format("Cập nhật phiếu thu khác không thành công!");
                }
            }

            return result;
        }
    }
}
