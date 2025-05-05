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
    public class ManagementBillController : ControllerBase
    {
        private IManagementBillRepository _IManagementBillRepository;

        public ManagementBillController(IManagementBillRepository IManagementBillRepository)
        {
            _IManagementBillRepository = IManagementBillRepository;
        }

        [HttpPost("AddManagementBill")]
        public async Task<IActionResult> AddManagementBill(ManagementBillViewModel model)
        {
            var rs = await _IManagementBillRepository.AddManagementBill(model);

            return Ok(rs);
        }

        [HttpDelete("DeleteManagementBillById")]
        public async Task<IActionResult> DeleteManagementBillById([FromQuery] string id)
        {
            var rs = await _IManagementBillRepository.DeleteManagementBillById(id);

            return Ok(rs);
        }

        [HttpPost("GetAllManagementBill")]
        public async Task<IActionResult> GetAllManagementBill(PagingData page)
        {
            var rs = await _IManagementBillRepository.GetAllManagementBill(page);

            return Ok(rs);
        }

        [HttpPost("UpdateManagementBillById")]
        public async Task<IActionResult> UpdateManagementBillById(ManagementBillViewModel model)
        {
            var rs = await _IManagementBillRepository.UpdateManagementBillById(model);

            return Ok(rs);
        }

        [HttpGet("PrintPDF")]
        public async Task<IActionResult> GetInvoicePdf([FromQuery] string id)
        {
            var pdfBytes = await _IManagementBillRepository.PrintManagementBill(id);
            if (pdfBytes.Length == 0) return NotFound();
            return File(pdfBytes, "application/pdf", $"PTQL_{DateTime.Now.ToString("ddMMYYYYhhmmss")}.pdf");
        }
    }
}
