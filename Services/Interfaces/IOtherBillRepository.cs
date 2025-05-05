using API.Common;
using API.Services.Models;

namespace API.Services.Interfaces
{
    public interface IOtherBillRepository
    {
        Task<ResultCommon> AddOtherBill(OtherBillViewModel entity);
        Task<ResultCommon> DeleteOtherBillById(string id);
        Task<PagingData> GetAllOtherBill(PagingData page);
        Task<byte[]> PrintOtherBill(string id);
        Task<ResultCommon> UpdateOtherBillById(OtherBillViewModel entity);
    }
}
