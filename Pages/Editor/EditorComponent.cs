using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using nats_ui.Data.Scripts;
using NLog;

namespace nats_ui.Pages.Editor
{
    public class EditorComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public class CommandModel
        {
            private ScriptService ScriptService { get; }

            public CommandModel(ScriptService scriptService)
            {
                ScriptService = scriptService;
            }

            private string name;

            public string Name
            {
                get => name;
                set
                {
                    name = value;
                    var obj = ScriptService.Create(name);
                    ParamName1 = obj.ParamName1;
                    ParamName2 = obj.ParamName2;
                }
            }

            public string Param1 { get; set;}
            public string Param2 { get; set;}
            public string ParamName1 { get; set; }
            public string ParamName2 { get; set; }
        }

        [Inject]
        protected ScriptService ScriptService { get; set; }

        protected StandardGridModel<IScriptCommand> CommandsGrid { get; } = new StandardGridModel<IScriptCommand>(); 
        protected EditorCellFactory GridCellFactory { get; } = new EditorCellFactory();

        protected CommandModel Model { get; private set; }

        protected override Task OnInitializedAsync()
        {
            Model = new CommandModel(ScriptService);
            CommandsGrid.SetData(ScriptService.Current.Commands);
            
            return Task.CompletedTask;
        }

        private void OnSelectedItemChanged(IScriptCommand message)
        {
            Model.Name = message.GetType().Name;
            Model.Param1 = message.Param1;
            Model.Param2 = message.Param2;
            Model.ParamName1 = message.ParamName1;
            Model.ParamName2 = message.ParamName2;

            InvokeAsync(StateHasChanged);
        }

        protected void Add()
        {
            if (string.IsNullOrEmpty(Model.Name))
            {
                return;
            }
            var scriptCommand = ScriptService.Create(Model.Name);
            scriptCommand.Param1 = Model.Param1;
            scriptCommand.Param2 = Model.Param2;
            ScriptService.Add(scriptCommand);
            CommandsGrid.Add(scriptCommand);
            InvokeAsync(StateHasChanged);
        }
    }
}