using API.Services.Models;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class VehicleFee : GeneralViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid BuildingId { get; set; }
        public Guid ApartmentId { get; set; }
        public int Type { get; set; }   // 1.xe đạp; 2.xe điện; 3.xe máy; 4.ô tô; 5.loại khác
        public string License { get; set; }    // biển số xe
        public decimal Price { get; set; } = 0; // tiền phí
        public string? Ticket { get; set; }  // vé xe
    }
}
