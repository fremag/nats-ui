using System;
using System.Linq;
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
        
        [Inject]
        protected ExecutorService ExecutorService { get; set; }

        [Inject]
        private NavigationManager NavMgr { get; set; }
        
        protected FlexGrid StatementsGrid { get; set; }
        
        protected StandardGridModel<ScriptStatement> Statements { get; } = new StandardGridModel<ScriptStatement>();
        protected EditorCellFactory GridCellFactory { get; } = new EditorCellFactory();

        protected CommandFormModel StatementModel { get; private set; }
        protected SaveFormModel SaveModel { get; private set; }
        public GridDataMap StatementMap { get; } = new GridDataMap();

        protected string ResultClass { get; set; } = "d-none";
        protected string Result  { get; set; } = "";
        
        protected override Task OnInitializedAsync()
        {
            StatementMap.ItemsSource = ScriptService.CommandsByName.Keys;
            StatementModel = new CommandFormModel();
            SaveModel = new SaveFormModel();
            Statements.ItemClicked += OnItemClicked;
            Statements.SelectedItemChanged += OnSelectedItemChanged;
            Init();

            return Task.CompletedTask;
        }

        private void Init()
        {
            Statements.SetData(ScriptService.Current.Statements);
            SaveModel.File = ScriptService.Current.File;
            SaveModel.Name = ScriptService.Current.Name;
        }

        private void OnItemClicked(string colName, ScriptStatement command)
        {
            int index = ScriptService.Current.Statements.IndexOf(command);
            switch (colName)
            {
                case nameof(ScriptStatement.Insert):
                    Logger.Debug($"Insert ! {index}");
                    var scriptStatement = new ScriptStatement();
                    Statements.Insert(index+1, scriptStatement);
                    ScriptService.Current.Insert(index+1, scriptStatement);
                    break;
                case nameof(ScriptStatement.Up):
                    Logger.Debug($"Up ! {index}");
                    Statements.Swap(index, index - 1);
                    ScriptService.Current.Swap(index, index - 1);
                    break;
                case nameof(ScriptStatement.Down):
                    Logger.Debug($"Down !  {index}");
                    Statements.Swap(index, index + 1);
                    ScriptService.Current.Swap(index, index + 1);
                    break;
                case nameof(ScriptStatement.Trash):
                    Logger.Debug($"Trash !  {index}");
                    Statements.Remove(index);
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

        protected void Create()
        {
            Logger.Info($"Creating a new statement: {StatementModel.Name}");
            if (string.IsNullOrEmpty(StatementModel.Name))
            {
                Logger.Warn("Statement not created, command name is null or empty.");
                return;
            }

            var scriptCommand = new ScriptStatement {Name = StatementModel.Name, Param1 = StatementModel.Param1, Param2 = StatementModel.Param2};
            ScriptService.Add(scriptCommand);
            Statements.Add(scriptCommand);
            InvokeAsync(StateHasChanged);
        }

        protected void Update()
        {
            Logger.Info($"Update: {StatementModel.Name}");

            var currentStatement = StatementModel.CurrentStatement;
            if (currentStatement == null || string.IsNullOrEmpty(StatementModel.Name))
            {
                Logger.Warn($"Update: failed, no statement is selected.");
                return;
            }

            currentStatement.Name = StatementModel.Name;
            currentStatement.Param1 = StatementModel.Param1;
            currentStatement.Param2 = StatementModel.Param2;
            Statements.Update(currentStatement);
            InvokeAsync(StateHasChanged);
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
            Statements.SetData(ScriptService.Current.Statements);
        }

        protected void NewScript()
        {
            Logger.Info("New Script.");
            ScriptService.SetCurrent(new Script());
            Init();
            InvokeAsync(StateHasChanged);
        }
        
        protected void StartRecord()
        {
            Logger.Info("Starting to record.");
            RecordService.StartRecord(NatsService, ScriptService);
        }

        protected void StopRecord()
        {
            Logger.Info("Stop recording");
            RecordService.StopRecord(NatsService);
        }

        public void Run()
        {
            Logger.Info($"{nameof(Run)}");
            ExecutorService.Setup(ScriptService.Current, ScriptService);
            NavMgr.NavigateTo("/executor");
            ExecutorService.Run();
        }
        
        public void Step()
        {
            int row = StatementsGrid.Selection?.Row ?? -1;
            if (row != -1)
            {
                Logger.Info($"{nameof(Step)}");
                try
                {
                    var stmt = Statements[row];
                    var command = ScriptService.BuildCommand(stmt);
                    var result = command.Execute(NatsService, ExecutorService);
                    Logger.Info($"Success ! {result}");
                    Result = result;
                    ResultClass = "d-block";
                    InvokeAsync(StateHasChanged);
                }
                catch (Exception e)
                {
                    Logger.Error($"Command failed ! {e.Message}");
                }
            }            
            GoNext();
        }
        
        public void GoNext()
        {
            int row = StatementsGrid.Selection?.Row ?? -1;
            int nextRow = Math.Min(Statements.Count()-1, row + 1);
            Logger.Debug($"{nameof(GoNext)}: {row} => {nextRow}");
            StatementsGrid.Select(new GridCellRange(nextRow, 0), true); 
        }

        public void GoTop()
        {
            if (!Statements.Any())
            {
                return;
            }

            Logger.Debug($"{nameof(GoTop)}");
            StatementsGrid.Select(new GridCellRange(0, 0), true);
        }
        
        public void GoBottom()
        {
            if (!Statements.Any())
            {
                return;
            }

            Logger.Debug($"{nameof(GoBottom)}");
            StatementsGrid.Select(new GridCellRange(Statements.Count()-1, 0), true);
        }
        
        public void GoPrevious()
        {
            int row = StatementsGrid.Selection?.Row ?? -1;
            int nextRow = Math.Max(0, row - 1);
            Logger.Debug($"{nameof(GoPrevious)}: {row} => {nextRow}");
            StatementsGrid.Select(new GridCellRange(nextRow, 0), true); 
        }
    }
}