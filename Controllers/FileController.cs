using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _IWebHostEnvironment;

        public FileController(IWebHostEnvironment IWebHostEnvironment)
        {
            _IWebHostEnvironment = IWebHostEnvironment;
        }

        [HttpGet("DownloadTemplate/{loai}")]
        public IActionResult DownloadTemplate(int loai) // 1.apartment
        {
            string fileName = "";
            switch(loai)
            {
                case 1:
                    fileName = "Template_Apartment.xlsx";
                    break;
                default:
                    break;
            }    

            var filePath = Path.Combine(_IWebHostEnvironment.ContentRootPath, "Templates", fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound("Không tìm thấy file mẫu.");

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
