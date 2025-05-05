using API.Common;
using API.Services.Models;

namespace API.Services.Interfaces
{
    public interface IApartmentRepository
    {
        Task<ResultCommon> AddApartment(ApartmentViewModel entity);
        Task<List<ApartmentViewModel>> CheckingDataImport(IFormFile file);
        Task<ResultCommon> DeleteApartmentById(string id);
        Task<PagingData> GetAllApartment(PagingData page);
        Task<List<ApartmentViewModel>> ImportApartment(List<ApartmentViewModel> list);
        Task<ResultCommon> UpdateApartmentById(ApartmentViewModel entity);
    }
}
