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

        public class CommandFormModel
        {
            private ScriptService ScriptService { get; }

            public CommandFormModel(ScriptService scriptService)
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

            public string Param1 { get; set; }
            public string Param2 { get; set; }
            public string ParamName1 { get; set; }
            public string ParamName2 { get; set; }
        }

        public class SaveFormModel
        {
            public string Name { get; set; }
            public string File { get; set; }
        }

        [Inject]
        protected ScriptService ScriptService { get; set; }

        protected StandardGridModel<IScriptCommand> CommandsGrid { get; } = new StandardGridModel<IScriptCommand>();
        protected EditorCellFactory GridCellFactory { get; } = new EditorCellFactory();

        protected CommandFormModel CommandModel { get; private set; }
        protected SaveFormModel SaveModel { get; private set; }

        protected override Task OnInitializedAsync()
        {
            CommandModel = new CommandFormModel(ScriptService);
            SaveModel = new SaveFormModel();
            CommandsGrid.SetData(ScriptService.Current.Commands);
            CommandsGrid.ItemClicked += OnItemClicked;
            SaveModel.File = ScriptService.Current.File;
            SaveModel.Name = ScriptService.Current.Name;

            return Task.CompletedTask;
        }

        private void OnItemClicked(string colName, IScriptCommand command)
        {
            int index = ScriptService.Current.Commands.IndexOf(command);
            switch (colName)
            {
                case nameof(IScriptCommand.Up):
                    Logger.Info($"Up ! {index}");
                    CommandsGrid.Swap(index, index-1);
                    ScriptService.Current.Swap(index, index - 1);
                    break;
                case nameof(IScriptCommand.Down):
                    Logger.Info($"Down !  {index}");
                    CommandsGrid.Swap(index, index+1);
                    ScriptService.Current.Swap(index, index + 1);
                    break;
                case nameof(IScriptCommand.Trash):
                    Logger.Info($"Trash !  {index}");
                    CommandsGrid.Remove(index);
                    ScriptService.Current.Remove(index);
                    break;
            }
        }

        private void OnSelectedItemChanged(IScriptCommand command)
        {
            CommandModel.Name = command.GetType().Name;
            CommandModel.Param1 = command.Param1;
            CommandModel.Param2 = command.Param2;
            CommandModel.ParamName1 = command.ParamName1;
            CommandModel.ParamName2 = command.ParamName2;
            InvokeAsync(StateHasChanged);
        }

        protected void Add()
        {
            if (string.IsNullOrEmpty(CommandModel.Name))
            {
                return;
            }

            var scriptCommand = ScriptService.Create(CommandModel.Name);
            scriptCommand.Param1 = CommandModel.Param1;
            scriptCommand.Param2 = CommandModel.Param2;
            ScriptService.Add(scriptCommand);
            CommandsGrid.Add(scriptCommand);
            InvokeAsync(StateHasChanged);
        }

        protected void Save()
        {
            ScriptService.Save(SaveModel.Name, SaveModel.File);
        }
    }
}