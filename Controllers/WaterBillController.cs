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
    public class WaterBillController : ControllerBase
    {
        private IWaterBillRepository _IWaterBillRepository;

        public WaterBillController(IWaterBillRepository IWaterBillRepository)
        {
            _IWaterBillRepository = IWaterBillRepository;
        }

        [HttpPost("AddWaterBill")]
        public async Task<IActionResult> AddWaterBill(WaterBillViewModel model)
        {
            var rs = await _IWaterBillRepository.AddWaterBill(model);

            return Ok(rs);
        }

        [HttpDelete("DeleteWaterBillById")]
        public async Task<IActionResult> DeleteWaterBillById([FromQuery] string id)
        {
            var rs = await _IWaterBillRepository.DeleteWaterBillById(id);

            return Ok(rs);
        }

        [HttpPost("GetAllWaterBill")]
        public async Task<IActionResult> GetAllWaterBill(PagingData page)
        {
            var rs = await _IWaterBillRepository.GetAllWaterBill(page);

            return Ok(rs);
        }

        [HttpPost("UpdateWaterBillById")]
        public async Task<IActionResult> UpdateWaterBillById(WaterBillViewModel model)
        {
            var rs = await _IWaterBillRepository.UpdateWaterBillById(model);

            return Ok(rs);
        }

        [HttpGet("PrintPDF")]
        public async Task<IActionResult> GetInvoicePdf([FromQuery] string id)
        {
            var pdfBytes = await _IWaterBillRepository.PrintWaterBill(id);
            if (pdfBytes.Length == 0) return NotFound();
            return File(pdfBytes, "application/pdf", $"PTX_{DateTime.Now.ToString("ddMMYYYYhhmmss")}.pdf");
        }
    }
}
