using API.Services.Models;

namespace API.Common
{
    public class RevenueModel
    {
        public DateTime? BeginDate { get; set; }        // ngày bắt đầu báo cáo
        public DateTime? EndDate { get; set; }          // ngày kết thúc báo cáo
        public decimal? ManagementAmount { get; set; }  // số tiền phí quản lý
        public int? ManagementNumber { get; set; }      // số căn thu phí quản lý
        public decimal? WaterAmount { get; set; }       // số tiền nước
        public int? WaterNumber { get; set; }           // số căn thu tiền nước
        public decimal? VehicleAmount { get; set; }     // số tiền phí gửi xe
        public int? VehicleNumber { get; set; }         // số căn thu phí gửi xe
        public decimal? OtherAmount { get; set; }       // số tiền phí khác
        public decimal? TotalAmount { get; set; }       // tổng cộng số tiền
    }
}
