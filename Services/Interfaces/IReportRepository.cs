using API.Common;

namespace API.Services.Interfaces
{
    public interface IReportRepository
    {
        Task<byte[]> PrintPDFWarningReport(WarningModel model);
        Task<RevenueModel> RevenueReport(RevenueModel model);
        Task<PagingData> WarningReport(PagingData page);
    }
}
