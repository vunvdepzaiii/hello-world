using API.Services.Models;

namespace API.Common
{
    public class ReceiptModel
    {
        public string? Id { get; set; }
        public string? CompanyName { get; set; }        // đơn vị
        public string? CompanyAddress { get; set; }     // địa chỉ
        public string? TemplateNo { get; set; }         // mẫu số
        public string? Decision { get; set; }           // quyết định ban hành
        public string? PlaceOfDecision { get; set; }    // nơi ban hành quyết định
        public string? ReceiptCode { get; set; }        // số phiếu
        public DateTime? ReceiptDate { get; set; }      // ngày phiếu thu
        public string? CustomerName { get; set; }       // người nộp tiền
        public string? Content { get; set; }            // nội dung nộp
        public decimal? Amount { get; set; }            // số tiền
        public string? AmountInWords { get; set; }      // số tiền bằng chữ
        public string? Cashier { get; set; }            // thu ngân
        public string? PaymentMethod { get; set; }      // hình thức thanh toán

        public ApartmentViewModel? ApartmentViewModel { get; set; } // thông tin căn hộ
        public BuildingViewModel? BuildingViewModel { get; set; }   // thông tin tòa nhà
        public UserViewModel? UserCreatedViewModel { get; set; }    // người tạo phiếu
    }
}
