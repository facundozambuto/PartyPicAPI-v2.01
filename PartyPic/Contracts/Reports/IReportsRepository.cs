using PartyPic.Models.Reports;

namespace PartyPic.Contracts.Reports
{
    public interface IReportsRepository
    {
        ReportsResponse GetReports();
    }
}
