using API.Common;
using API.Services.Models;

namespace API.Services.Interfaces
{
    public interface IManagementBillRepository
    {
        Task<ResultCommon> AddManagementBill(ManagementBillViewModel entity);
        Task<ResultCommon> DeleteManagementBillById(string id);
        Task<PagingData> GetAllManagementBill(PagingData page);
        Task<byte[]> PrintManagementBill(string id);
        Task<ResultCommon> UpdateManagementBillById(ManagementBillViewModel entity);
    }
}
