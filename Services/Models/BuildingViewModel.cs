namespace API.Services.Models
{
    public class BuildingViewModel : GeneralViewModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }    // tên tòa nhà
        public string? Code { get; set; }    // mã tòa nhà
        public string? Address { get; set; } // địa chỉ
        public string? Description { get; set; } // mô tả
    }
}
