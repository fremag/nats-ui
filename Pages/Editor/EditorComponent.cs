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

        [Inject]
        protected NatsService NatsService { get; set; }

        [Inject]
        protected RecordService RecordService { get; set; }
        
        protected StandardGridModel<ScriptStatement> StatementsGrid { get; } = new StandardGridModel<ScriptStatement>();
        protected EditorCellFactory GridCellFactory { get; } = new EditorCellFactory();

        protected CommandFormModel StatementModel { get; private set; }
        protected SaveFormModel SaveModel { get; private set; }
        public GridDataMap StatementMap { get; } = new GridDataMap();


        protected override Task OnInitializedAsync()
        {
            StatementMap.ItemsSource = ScriptService.CommandsByName.Keys;
            StatementModel = new CommandFormModel();
            SaveModel = new SaveFormModel();
            StatementsGrid.SetData(ScriptService.Current.Statements);
            StatementsGrid.ItemClicked += OnItemClicked;
            StatementsGrid.SelectedItemChanged += OnSelectedItemChanged;
            SaveModel.File = ScriptService.Current.File;
            SaveModel.Name = ScriptService.Current.Name;

            return Task.CompletedTask;
        }

        private void OnItemClicked(string colName, ScriptStatement command)
        {
            int index = ScriptService.Current.Statements.IndexOf(command);
            switch (colName)
            {
                case nameof(ScriptStatement.Insert):
                    Logger.Debug($"Insert ! {index}");
                    var scriptStatement = new ScriptStatement();
                    StatementsGrid.Insert(index+1, scriptStatement);
                    ScriptService.Current.Insert(index+1, scriptStatement);
                    break;
                case nameof(ScriptStatement.Up):
                    Logger.Debug($"Up ! {index}");
                    StatementsGrid.Swap(index, index - 1);
                    ScriptService.Current.Swap(index, index - 1);
                    break;
                case nameof(ScriptStatement.Down):
                    Logger.Debug($"Down !  {index}");
                    StatementsGrid.Swap(index, index + 1);
                    ScriptService.Current.Swap(index, index + 1);
                    break;
                case nameof(ScriptStatement.Trash):
                    Logger.Debug($"Trash !  {index}");
                    StatementsGrid.Remove(index);
                    ScriptService.Current.Remove(index);
                    break;
            }
        }

        private void OnSelectedItemChanged(ScriptStatement statement)
        {
            if (! string.IsNullOrEmpty(statement.Name) && ScriptService.CommandsByName.TryGetValue(statement.Name, out var commandInfo))
            {
                StatementModel.Name = commandInfo.Name;
                StatementModel.ParamName1 = commandInfo.ParamName1;
                StatementModel.ParamName2 = commandInfo.ParamName2;
            }
            
            StatementModel.CurrentStatement = statement;
            StatementModel.Param1 = statement.Param1;
            StatementModel.Param2 = statement.Param2;
            InvokeAsync(StateHasChanged);
        }

        protected void StatementSubmit()
        {
            if (StatementModel.Create)
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
            Logger.Info($"Create new statement");
            if (string.IsNullOrEmpty(StatementModel.Name))
            {
                return;
            }

            var scriptCommand = new ScriptStatement {Name = StatementModel.Name, Param1 = StatementModel.Param1, Param2 = StatementModel.Param2};
            ScriptService.Add(scriptCommand);
            StatementsGrid.Add(scriptCommand);
        }

        private void Update()
        {
            Logger.Info($"Update: {StatementModel.Name}");

            var currentStatement = StatementModel.CurrentStatement;
            if (currentStatement == null || string.IsNullOrEmpty(StatementModel.Name))
            {
                return;
            }

            currentStatement.Name = StatementModel.Name;
            currentStatement.Param1 = StatementModel.Param1;
            currentStatement.Param2 = StatementModel.Param2;
            StatementsGrid.Update(currentStatement);
        }

        protected void Save()
        {
            Logger.Info($"Save: {SaveModel.Name}, {SaveModel.File}");
            ScriptService.Save(SaveModel.Name, SaveModel.File);
            ScriptService.Load();
        }

        protected void ReloadScript()
        {
            Logger.Info($"Reload current script: {ScriptService.Current.Name}");
            ScriptService.Reload(ScriptService.Current);
            StatementsGrid.SetData(ScriptService.Current.Statements);
        }

        protected void NewScript()
        {
            Logger.Info($"New Script");
            ScriptService.SetCurrent(new Script());
        }
        
        protected void Remove()
        {
            foreach(var (index, statement) in StatementsGrid.GetCheckedItems()) 
            {
                Logger.Info($"Remove: {statement}");
                StatementsGrid.Remove(index);
                ScriptService.Current.Remove(index);
            }
        }
        
        protected void StartRecord()
        {
            Logger.Info($"Start record");
            RecordService.StartRecord(NatsService, ScriptService);
        }

        public bool Recording { get; private set; }

        protected void StopRecord()
        {
            Logger.Info($"Stop record");
            RecordService.StopRecord(NatsService);
        }
    }
}