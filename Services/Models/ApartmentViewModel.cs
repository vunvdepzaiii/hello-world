namespace API.Services.Models
{
    public class ApartmentViewModel : GeneralViewModel
    {
        public string? Id { get; set; }
        public string? Number { get; set; }  // mã căn hộ
        public string? Owner { get; set; }   // chủ hộ
        public string? Phone { get; set; }
        public string? AccountZalo { get; set; }
        public string? BuildingId { get; set; }    // tòa nhà
        public BuildingViewModel? BuildingViewModel { get; set; }   // thông tin tòa nhà
        public decimal? ManageFee { get; set; } // phí quản lý
        public DateTime? StartDate { get; set; } // ngày nhận nhà
        public List<VehicleFeeViewModel>? ListVehicleFeeViewModel { get; set; }   // thông tin phí gửi xe

        //import
        public bool? IsValid { get; set; }  // checking import data
        public string? MessageCheck { get; set; }   // message checking import data
        public string? ManageFeeImport { get; set; }
        public string? StartDateImport { get; set; }
        public int? IndexImport { get; set; }
    }
}
