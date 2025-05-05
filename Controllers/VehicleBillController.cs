using API.Common;
using API.Services.Interfaces;
using API.Services.Models;
using API.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class VehicleBillController : ControllerBase
    {
        private IVehicleBillRepository _IVehicleBillRepository;

        public VehicleBillController(IVehicleBillRepository IVehicleBillRepository)
        {
            _IVehicleBillRepository = IVehicleBillRepository;
        }

        [HttpPost("AddVehicleBill")]
        public async Task<IActionResult> AddVehicleBill(VehicleBillViewModel model)
        {
            var rs = await _IVehicleBillRepository.AddVehicleBill(model);

            return Ok(rs);
        }

        [HttpDelete("DeleteVehicleBillById")]
        public async Task<IActionResult> DeleteVehicleBillById([FromQuery] string id)
        {
            var rs = await _IVehicleBillRepository.DeleteVehicleBillById(id);

            return Ok(rs);
        }

        [HttpPost("GetAllVehicleBill")]
        public async Task<IActionResult> GetAllVehicleBill(PagingData page)
        {
            var rs = await _IVehicleBillRepository.GetAllVehicleBill(page);

            return Ok(rs);
        }

        [HttpPost("UpdateVehicleBillById")]
        public async Task<IActionResult> UpdateVehicleBillById(VehicleBillViewModel model)
        {
            var rs = await _IVehicleBillRepository.UpdateVehicleBillById(model);

            return Ok(rs);
        }

        [HttpGet("PrintPDF")]
        public async Task<IActionResult> GetInvoicePdf([FromQuery] string id)
        {
            var pdfBytes = await _IVehicleBillRepository.PrintVehicleBill(id);
            if (pdfBytes.Length == 0) return NotFound();
            return File(pdfBytes, "application/pdf", $"PTX_{DateTime.Now.ToString("ddMMYYYYhhmmss")}.pdf");
        }
    }
}
