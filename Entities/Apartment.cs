using API.Services.Models;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Apartment : GeneralViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Number { get; set; }  // mã căn hộ
        public string Owner { get; set; }   // chủ hộ
        public string? Phone { get; set; }
        public string? AccountZalo { get; set; }
        public Guid BuildingId { get; set; }    // tòa nhà
        public decimal ManageFee { get; set; } = 0; // phí quản lý
        public DateTime StartDate { get; set; } // ngày nhận nhà
    }
}
