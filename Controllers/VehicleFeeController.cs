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
    public class VehicleFeeController : ControllerBase
    {
        private IVehicleFeeRepository _IVehicleFeeRepository;

        public VehicleFeeController(IVehicleFeeRepository IVehicleFeeRepository)
        {
            _IVehicleFeeRepository = IVehicleFeeRepository;
        }

        [HttpPost("AddVehicleFee")]
        public async Task<IActionResult> AddVehicleFee(VehicleFeeViewModel model)
        {
            var rs = await _IVehicleFeeRepository.AddVehicleFee(model);

            return Ok(rs);
        }

        [HttpDelete("DeleteVehicleFeeById")]
        public async Task<IActionResult> DeleteVehicleFeeById([FromQuery] string id)
        {
            var rs = await _IVehicleFeeRepository.DeleteVehicleFeeById(id);

            return Ok(rs);
        }

        [HttpPost("GetAllVehicleFee")]
        public async Task<IActionResult> GetAllVehicleFee(PagingData page)
        {
            var rs = await _IVehicleFeeRepository.GetAllVehicleFee(page);

            return Ok(rs);
        }

        [HttpPost("UpdateVehicleFeeById")]
        public async Task<IActionResult> UpdateVehicleFeeById(VehicleFeeViewModel model)
        {
            var rs = await _IVehicleFeeRepository.UpdateVehicleFeeById(model);

            return Ok(rs);
        }
    }
}
