using C1.Blazor.Core;
using nats_ui.Data;

namespace nats_ui.Pages.History
{
    public class JobCellFactory : StandardCellFactory<Job>
    {
        protected override void PrepareCellStyle(string colName, Job job, C1Style cellType)
        {
        }
    }
}