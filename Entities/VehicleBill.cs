using API.Services.Models;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class VehicleBill : GeneralViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public string No { get; set; }  // số phiếu thu phí gửi xe PTX
        public Guid BuildingId { get; set; }    // tòa nhà id
        public Guid ApartmentId { get; set; }   // căn hộ id
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }     // số tiền
        public bool? IsPay { get; set; }   // đã thanh toán 
        public string? Payer { get; set; }  // người nộp tiên
        public string? Cashier { get; set; }    // thu ngân
        public DateTime? PaymentDate { get; set; }  // ngày thanh toán
        public int? PaymentMethod { get; set; } // 1.tiền mặt, 2.chuyển khoản
    }
}
