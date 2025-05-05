namespace API.Services.Models
{
    public class WaterBillViewModel : GeneralViewModel
    {
        public string? Id { get; set; }
        public string? No { get; set; }  // số phiếu thu tiền nước PTN
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string? BuildingId { get; set; }  // tòa nhà id
        public string? ApartmentId { get; set; }   // căn hộ id
        public int? BeginNumber { get; set; }    // chỉ số đồng hồ đầu
        public int? EndNumber { get; set; }      // chỉ số đồng hồ cuối
        public decimal? FeedWaterFee { get; set; }   // phí cấp nước
        public decimal? WasteWaterFee { get; set; }  // phí thoát nước
        public bool? IsPay { get; set; }   // đã thanh toán 
        public string? Payer { get; set; }  // người nộp tiên
        public string? Cashier { get; set; }    // thu ngân
        public DateTime? PaymentDate { get; set; }  // ngày thanh toán
        public int? PaymentMethod { get; set; } // 1.tiền mặt, 2.chuyển khoản

        public ApartmentViewModel? ApartmentViewModel { get; set; } // thông tin căn hộ
        public BuildingViewModel? BuildingViewModel { get; set; }   // thông tin tòa nhà
        public string? BillingCycle { get; set; }   // kỳ thanh toán
    }
}
