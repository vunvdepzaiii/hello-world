using API.Common;
using API.Services.Models;

namespace API.Services.Interfaces
{
    public interface IVehicleFeeRepository
    {
        Task<ResultCommon> AddVehicleFee(VehicleFeeViewModel entity);
        Task<ResultCommon> DeleteVehicleFeeById(string id);
        Task<PagingData> GetAllVehicleFee(PagingData page);
        Task<ResultCommon> UpdateVehicleFeeById(VehicleFeeViewModel entity);
    }
}
