using API.Common;
using API.Services.Models;

namespace API.Services.Interfaces
{
    public interface IVehicleBillRepository
    {
        Task<ResultCommon> AddVehicleBill(VehicleBillViewModel entity);
        Task<ResultCommon> DeleteVehicleBillById(string id);
        Task<PagingData> GetAllVehicleBill(PagingData page);
        Task<byte[]> PrintVehicleBill(string id);
        Task<ResultCommon> UpdateVehicleBillById(VehicleBillViewModel entity);
    }
}
