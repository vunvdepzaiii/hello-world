namespace API.Services.Models
{
    public class VehicleFeeViewModel : GeneralViewModel
    {
        public string? Id { get; set; }
        public string? BuildingId { get; set; }
        public string? ApartmentId { get; set; }
        public int? Type { get; set; }   // 1.xe đạp; 2.xe điện; 3.xe máy; 4.ô tô; 5.loại khác
        public string? License { get; set; }    // biển số xe
        public decimal? Price { get; set; } // tiền phí
        public string? Ticket { get; set; }  // vé xe
        public string? TypeName { get; set; }   // loại xe
    }
}
