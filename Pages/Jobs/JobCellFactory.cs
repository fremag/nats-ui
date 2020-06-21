using C1.Blazor.Core;
using nats_ui.Data;
using nats_ui.Data.Scripts;

namespace nats_ui.Pages.Jobs
{
    public class JobCellFactory : StandardCellFactory<Job>
    {
        protected override void PrepareCellStyle(string colName, Job job, C1Style cellStyle)
        {
            switch (colName)
            {
                case nameof(Job.Errors):
                    cellStyle.BackgroundColor = job.Errors > 0 ? C1Color.Red : C1Color.Green;
                    break;
                case nameof(Job.Status):
                    cellStyle.BackgroundColor = job.Status switch
                    {
                        ExecutionStatus.Failed => C1Color.Red,
                        ExecutionStatus.Running => C1Color.LightGreen,
                        ExecutionStatus.Waiting => C1Color.LightBlue,
                        ExecutionStatus.Executed => C1Color.Green,
                        _ => cellStyle.BackgroundColor
                    };
                    break;
            }
        }
    }
}