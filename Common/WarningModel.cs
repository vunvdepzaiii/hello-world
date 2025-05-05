using API.Services.Models;

namespace API.Common
{
    public class WarningModel
    {
        public string? Id { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string? BuildingId { get; set; }                 // tòa nhà id
        public string? ApartmentId { get; set; }                // căn hộ id
        public string? Owner { get; set; }                      // chủ hộ
        public decimal? ManagementFeeForward { get; set; }      // phí quản lý kỳ trước
        public decimal? VehicleFeeForward { get; set; }         // phí gửi xe kỳ trước
        public decimal? WaterFeeForward { get; set; }           // tiền nước kỳ trước
        public decimal? ManagementFee { get; set; }             // phí quản lý
        public decimal? VehicleFee { get; set; }                // phí gửi xe
        public decimal? WaterFee { get; set; }                  // tiền nước
        public List<ManagementBillViewModel>? ManagementBillViewModel { get; set; }       // danh sách phiếu thu phí quản lý
        public List<VehicleBillViewModel>? VehicleBillViewModel { get; set; }             // danh sách phiếu thu phí gửi xe
        public List<WaterBillViewModel>? WaterBillViewModel { get; set; }                 // danh sách phiếu thu tiền nước
        public List<FeeForward>? ManagementBillForward { get; set; }                      // danh sách phiếu thu phí quản lý nợ kỳ trước
        public List<FeeForward>? VehicleBillForward { get; set; }                         // danh sách phiếu thu phí gửi xe nợ kỳ trước
        public List<FeeForward>? WaterBillForward { get; set; }                           // danh sách phiếu thu tiền nước nợ kỳ trước

        public BuildingViewModel? BuildingViewModel { get; set; }       // thông tin tòa nhà
        public ApartmentViewModel? ApartmentViewModel { get; set; }     // thông tin căn hộ
        public string? ManageName { get; set; }     // in phiếu thông báo
        public string? HostlineAdmin { get; set; }  // in phiếu thông báo
        public string? HostlineTech { get; set; }   // in phiếu thông báo
        public string? Deadline { get; set; }       // in phiếu thông báo
        public string? BankNumber { get; set; }       // in phiếu thông báo
        public string? BankOwner { get; set; }       // in phiếu thông báo
        public string? BankName  { get; set; }       // in phiếu thông báo
    }

    public class FeeForward
    {
        public string? Id { get; set; }             // id   
        public string? No { get; set; }             // số phiếu
        public string? BillingCycle { get; set; }   // kỳ thanh toán
        public decimal? Amount { get; set; }        // phí
    }
}
