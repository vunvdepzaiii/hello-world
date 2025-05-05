namespace API.Services.Models
{
    public class VehicleBillViewModel : GeneralViewModel
    {
        public string? Id { get; set; }
        public string? No { get; set; }  // số phiếu thu phí gửi xe PTX
        public string? BuildingId { get; set; }     // tòa nhà id
        public string? ApartmentId { get; set; }    // căn hộ id
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Amount { get; set; }     // số tiền
        public bool? IsPay { get; set; }   // đã thanh toán 
        public string? Payer { get; set; }  // người nộp tiên
        public string? Cashier { get; set; }    // thu ngân
        public DateTime? PaymentDate { get; set; }  // ngày thanh toán
        public int? PaymentMethod { get; set; } // 1.tiền mặt, 2.chuyển khoản

        public BuildingViewModel? BuildingViewModel { get; set; }    // thông tin tòa nhà
        public ApartmentViewModel? ApartmentViewModel { get; set; }  // thông tin căn hộ
        public string? BillingCycle { get; set; }   // kỳ thanh toán
    }
}
