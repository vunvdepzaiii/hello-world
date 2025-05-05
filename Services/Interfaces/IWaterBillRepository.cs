using API.Common;
using API.Services.Models;

namespace API.Services.Interfaces
{
    public interface IWaterBillRepository
    {
        Task<ResultCommon> AddWaterBill(WaterBillViewModel entity);
        Task<ResultCommon> DeleteWaterBillById(string id);
        Task<PagingData> GetAllWaterBill(PagingData page);
        Task<byte[]> PrintWaterBill(string id);
        Task<ResultCommon> UpdateWaterBillById(WaterBillViewModel entity);
    }
}
