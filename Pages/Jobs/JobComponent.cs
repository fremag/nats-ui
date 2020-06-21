using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using nats_ui.Data.Scripts;
using NLog;

namespace nats_ui.Pages.Jobs
{
    public class JobsComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        [Inject]
        private ScriptService ScriptService { get; set; }

        [Inject]
        private NatsService NatsService { get; set; }

        [Inject]
        private ExecutorService ExecutorService { get; set; }

        [Inject]
        private NavigationManager NavMgr { get; set; }

        protected StandardGridModel<Job> JobsGrid { get; } = new StandardGridModel<Job>(); 
        protected JobCellFactory JobCellFactory { get; } = new JobCellFactory();

        protected override Task OnInitializedAsync()
        {
            ExecutorService.JobUpdated += OnJobUpdated;
            JobsGrid.SetData(ExecutorService.Jobs);
            JobsGrid.ItemDoubleClicked += OnItemDoubleClicked; 
            JobsGrid.ItemClicked += OnItemClicked; 
            return Task.CompletedTask;
        }

        private void OnJobUpdated(Job job)
        {
            InvokeAsync(() => JobsGrid.Update(job));
        }

        private void OnItemClicked(string colName, Job report)
        {
            switch (colName)
            {
                case nameof(Script.Run):
                    var newReport = ExecutorService.Setup(report, ScriptService);
                    NavMgr.NavigateTo($"/executor/{report.Id}");
                    ExecutorService.Run(newReport);
                    break;
            }
        }

        private void OnItemDoubleClicked(string colName, Job report)
        {
            Logger.Info($"Set current report: {report.Name}");
            NavMgr.NavigateTo($"/executor/{report.Id}");
        }
    }
}