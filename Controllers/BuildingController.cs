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
    public class BuildingController : ControllerBase
    {
        private IBuildingRepository _IBuildingRepository;

        public BuildingController(IBuildingRepository IBuildingRepository)
        {
            _IBuildingRepository = IBuildingRepository;
        }

        [HttpPost("AddBuilding")]
        public async Task<IActionResult> AddBuilding(BuildingViewModel model)
        {
            var rs = await _IBuildingRepository.AddBuilding(model);

            return Ok(rs);
        }

        [HttpDelete("DeleteBuildingById")]
        public async Task<IActionResult> DeleteBuildingById([FromQuery] string id)
        {
            var rs = await _IBuildingRepository.DeleteBuildingById(id);

            return Ok(rs);
        }

        [HttpPost("GetAllBuilding")]
        public async Task<IActionResult> GetAllBuilding(PagingData page)
        {
            var rs = await _IBuildingRepository.GetAllBuilding(page);

            return Ok(rs);
        }

        [HttpPost("UpdateBuildingById")]
        public async Task<IActionResult> UpdateBuildingById(BuildingViewModel model)
        {
            var rs = await _IBuildingRepository.UpdateBuildingById(model);

            return Ok(rs);
        }
    }
}
