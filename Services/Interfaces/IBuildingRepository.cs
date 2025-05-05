using API.Common;
using API.Services.Models;

namespace API.Services.Interfaces
{
    public interface IBuildingRepository
    {
        Task<ResultCommon> AddBuilding(BuildingViewModel entity);
        Task<ResultCommon> DeleteBuildingById(string id);
        Task<PagingData> GetAllBuilding(PagingData page);
        Task<ResultCommon> UpdateBuildingById(BuildingViewModel entity);
    }
}
