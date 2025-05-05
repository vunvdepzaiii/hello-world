using API.Services.Models;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class WaterBill : GeneralViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public string No { get; set; }  // số phiếu thu tiền nước PTN
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid BuildingId { get; set; }    // tòa nhà id
        public Guid ApartmentId { get; set; }   // căn hộ id
        public int BeginNumber { get; set; }    // chỉ số đồng hồ đầu
        public int EndNumber { get; set; }      // chỉ số đồng hồ cuối
        public decimal FeedWaterFee { get; set; }   // phí cấp nước
        public decimal WasteWaterFee { get; set; }  // phí thoát nước
        public bool? IsPay { get; set; }   // đã thanh toán 
        public string? Payer { get; set; }  // người nộp tiên
        public string? Cashier { get; set; }    // thu ngân
        public DateTime? PaymentDate { get; set; }  // ngày thanh toán
        public int? PaymentMethod { get; set; } // 1.tiền mặt, 2.chuyển khoản
    }
}
