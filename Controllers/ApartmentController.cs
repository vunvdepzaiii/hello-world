using API.Common;
using API.Services.Interfaces;
using API.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ApartmentController : ControllerBase
    {
        private IApartmentRepository _IApartmentRepository;

        public ApartmentController(IApartmentRepository IApartmentRepository)
        {
            _IApartmentRepository = IApartmentRepository;
        }

        [HttpPost("AddApartment")]
        public async Task<IActionResult> AddApartment(ApartmentViewModel model)
        {
            var rs = await _IApartmentRepository.AddApartment(model);

            return Ok(rs);
        }

        [HttpDelete("DeleteApartmentById")]
        public async Task<IActionResult> DeleteApartmentById([FromQuery] string id)
        {
            var rs = await _IApartmentRepository.DeleteApartmentById(id);

            return Ok(rs);
        }

        [HttpPost("GetAllApartment")]
        public async Task<IActionResult> GetAllApartment(PagingData page)
        {
            var rs = await _IApartmentRepository.GetAllApartment(page);

            return Ok(rs);
        }

        [HttpPost("UpdateApartmentById")]
        public async Task<IActionResult> UpdateApartmentById(ApartmentViewModel model)
        {
            var rs = await _IApartmentRepository.UpdateApartmentById(model);

            return Ok(rs);
        }

        [HttpPost("CheckingDataImport")]
        public async Task<IActionResult> CheckingDataImport(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            var rs = await _IApartmentRepository.CheckingDataImport(file);

            return Ok(rs);
        }

        [HttpPost("ImportApartment")]
        public async Task<IActionResult> ImportApartment(List<ApartmentViewModel> list)
        {
            var rs = await _IApartmentRepository.ImportApartment(list);

            return Ok(rs);
        }
    }
}
