using System.Threading.Tasks;
using C1.Blazor.Grid;
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
            public string Name { get; set; }
            public string Param1 { get; set; }
            public string Param2 { get; set; }
            public string ParamName1 { get; set; }
            public string ParamName2 { get; set; }
            public ScriptStatement CurrentStatement { get; set; }
            public bool Create { get; set; }
        }

        public class SaveFormModel
        {
            public string Name { get; set; }
            public string File { get; set; }
        }

        [Inject]
        protected ScriptService ScriptService { get; set; }

        protected StandardGridModel<ScriptStatement> CommandsGrid { get; } = new StandardGridModel<ScriptStatement>();
        protected EditorCellFactory GridCellFactory { get; } = new EditorCellFactory();

        protected CommandFormModel CommandModel { get; private set; }
        protected SaveFormModel SaveModel { get; private set; }
        public GridDataMap CommandMap { get; set; } = new GridDataMap();


        protected override Task OnInitializedAsync()
        {
            CommandMap.ItemsSource = ScriptService.CommandsByName.Keys;
            CommandModel = new CommandFormModel();
            SaveModel = new SaveFormModel();
            CommandsGrid.SetData(ScriptService.Current.Commands);
            CommandsGrid.ItemClicked += OnItemClicked;
            CommandsGrid.SelectedItemChanged += OnSelectedItemChanged;
            SaveModel.File = ScriptService.Current.File;
            SaveModel.Name = ScriptService.Current.Name;

            return Task.CompletedTask;
        }

        private void OnItemClicked(string colName, ScriptStatement command)
        {
            int index = ScriptService.Current.Commands.IndexOf(command);
            switch (colName)
            {
                case nameof(ScriptStatement.Insert):
                    Logger.Debug($"Insert ! {index}");
                    var scriptStatement = new ScriptStatement();
                    CommandsGrid.Insert(index+1, scriptStatement);
                    ScriptService.Current.Insert(index+1, scriptStatement);
                    break;
                case nameof(ScriptStatement.Up):
                    Logger.Debug($"Up ! {index}");
                    CommandsGrid.Swap(index, index - 1);
                    ScriptService.Current.Swap(index, index - 1);
                    break;
                case nameof(ScriptStatement.Down):
                    Logger.Debug($"Down !  {index}");
                    CommandsGrid.Swap(index, index + 1);
                    ScriptService.Current.Swap(index, index + 1);
                    break;
                case nameof(ScriptStatement.Trash):
                    Logger.Debug($"Trash !  {index}");
                    CommandsGrid.Remove(index);
                    ScriptService.Current.Remove(index);
                    break;
            }
        }

        private void OnSelectedItemChanged(ScriptStatement statement)
        {
            if (! string.IsNullOrEmpty(statement.Name) && ScriptService.CommandsByName.TryGetValue(statement.Name, out var commandInfo))
            {
                CommandModel.Name = commandInfo.Name;
                CommandModel.ParamName1 = commandInfo.ParamName1;
                CommandModel.ParamName2 = commandInfo.ParamName2;
            }
            
            CommandModel.CurrentStatement = statement;
            CommandModel.Param1 = statement.Param1;
            CommandModel.Param2 = statement.Param2;
            InvokeAsync(StateHasChanged);
        }

        protected void StatementSubmit()
        {
            if (CommandModel.Create)
            {
                Create();
            }
            else
            {
                Update();
            }

            InvokeAsync(StateHasChanged);
        }

        private void Create()
        {
            if (string.IsNullOrEmpty(CommandModel.Name))
            {
                return;
            }

            var scriptCommand = new ScriptStatement {Name = CommandModel.Name, Param1 = CommandModel.Param1, Param2 = CommandModel.Param2};
            ScriptService.Add(scriptCommand);
            CommandsGrid.Add(scriptCommand);
        }

        private void Update()
        {
            var currentStatement = CommandModel.CurrentStatement;
            if (currentStatement == null || string.IsNullOrEmpty(CommandModel.Name))
            {
                return;
            }

            currentStatement.Name = CommandModel.Name;
            currentStatement.Param1 = CommandModel.Param1;
            currentStatement.Param2 = CommandModel.Param2;
            CommandsGrid.Update(currentStatement);
        }

        protected void Save()
        {
            ScriptService.Save(SaveModel.Name, SaveModel.File);
        }
    }
}