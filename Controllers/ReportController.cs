using API.Common;
using API.Services.Interfaces;
using API.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private IReportRepository _IReportRepository;

        public ReportController(IReportRepository IReportRepository)
        {
            _IReportRepository = IReportRepository;
        }

        [HttpPost("WarningReport")]
        public async Task<IActionResult> WarningReport(PagingData page)
        {
            var rs = await _IReportRepository.WarningReport(page);

            return Ok(rs);
        }

        [HttpPost("RevenueReport")]
        public async Task<IActionResult> RevenueReport(RevenueModel model)
        {
            var rs = await _IReportRepository.RevenueReport(model);

            return Ok(rs);
        }

        [HttpPost("PrintPDFWarningReport")]
        public async Task<IActionResult> PrintPDFWarningReport(WarningModel model)
        {
            var pdfBytes = await _IReportRepository.PrintPDFWarningReport(model);
            if (pdfBytes.Length == 0) return NotFound();
            return File(pdfBytes, "application/pdf", $"PTK_{DateTime.Now.ToString("ddMMYYYYhhmmss")}.pdf");
        }
    }
}
