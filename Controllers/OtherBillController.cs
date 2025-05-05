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
    public class OtherBillController : ControllerBase
    {
        private IOtherBillRepository _IOtherBillRepository;

        public OtherBillController(IOtherBillRepository IOtherBillRepository)
        {
            _IOtherBillRepository = IOtherBillRepository;
        }

        [HttpPost("AddOtherBill")]
        public async Task<IActionResult> AddOtherBill(OtherBillViewModel model)
        {
            var rs = await _IOtherBillRepository.AddOtherBill(model);

            return Ok(rs);
        }

        [HttpDelete("DeleteOtherBillById")]
        public async Task<IActionResult> DeleteOtherBillById([FromQuery] string id)
        {
            var rs = await _IOtherBillRepository.DeleteOtherBillById(id);

            return Ok(rs);
        }

        [HttpPost("GetAllOtherBill")]
        public async Task<IActionResult> GetAllOtherBill(PagingData page)
        {
            var rs = await _IOtherBillRepository.GetAllOtherBill(page);

            return Ok(rs);
        }

        [HttpPost("UpdateOtherBillById")]
        public async Task<IActionResult> UpdateOtherBillById(OtherBillViewModel model)
        {
            var rs = await _IOtherBillRepository.UpdateOtherBillById(model);

            return Ok(rs);
        }

        [HttpGet("PrintPDF")]
        public async Task<IActionResult> GetInvoicePdf([FromQuery] string id)
        {
            var pdfBytes = await _IOtherBillRepository.PrintOtherBill(id);
            if (pdfBytes.Length == 0) return NotFound();
            return File(pdfBytes, "application/pdf", $"PTK_{DateTime.Now.ToString("ddMMYYYYhhmmss")}.pdf");
        }
    }
}
