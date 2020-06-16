using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using nats_ui.Data.Scripts;
using NLog;

namespace nats_ui.Pages.Executor
{
    public class ExecutorComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        [Inject]
        private ExecutorService ExecutorService { get; set; }

        [Inject]
        private NavigationManager NavMgr { get; set; }

        [Parameter]
        public int JobId { get; set; } = -1;

        protected StandardGridModel<IScriptCommand> ScriptCommandGrid { get; } = new StandardGridModel<IScriptCommand>(); 
        protected ScriptCommandCellFactory GridCellFactory { get; } = new ScriptCommandCellFactory();

        protected override Task OnInitializedAsync()
        {
            if (JobId < 0 || JobId >= ExecutorService.Jobs.Count)
            {
                if (ExecutorService.LastJob == null)
                {
                    return Task.CompletedTask;
                }

                JobId = ExecutorService.LastJob.Id;
            }
            
            var job = ExecutorService.Jobs[JobId];
            if (job.Commands != null)
            {
                ScriptCommandGrid.SetData(new List<IScriptCommand>(job.Commands));
            }

            ExecutorService.CommandUpdated += OnCommandUpdated; 
            return Task.CompletedTask;
        }

        private void OnCommandUpdated(IScriptCommand command)
        {
            InvokeAsync(() => ScriptCommandGrid.Update(command));
        }
    }
}