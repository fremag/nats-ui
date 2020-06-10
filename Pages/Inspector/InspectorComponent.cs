using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JsonPathway;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using nats_ui.Data.Scripts;
using NLog;

namespace nats_ui.Pages.Inspector
{
    public class DataCaptureModel
    {
        public CaptureType CaptureType { get; set; }
        public string Name { get; set; }
        public string Expression { get; set; }
        public string Result { get; set; }
        public string Data { get; set; }
    }

    public class InspectorComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        [Inject]
        protected InspectorService Inspector { get; set; }

        [Inject]
        protected ScriptService ScriptService { get; set; }

        [Inject]
        protected NatsService NatsService { get; set; }
        
        protected DataCaptureModel Model { get; } = new DataCaptureModel();
        protected StandardGridModel<DataCapture> DataCaptureGrid { get; } = new StandardGridModel<DataCapture>();
        protected DataCaptureCellFactory GridCellFactory { get; } = new DataCaptureCellFactory();
        
        protected override Task OnInitializedAsync()
        {
            DataCaptureGrid.SetData(NatsService.Configuration.DataCaptures);
            DataCaptureGrid.ItemClicked += OnItemClicked;
            DataCaptureGrid.ItemDoubleClicked += OnItemDoubleClicked;
            DataCaptureGrid.SelectedItemChanged += OnSelectedItemChanged;
            Model.Data = Inspector.Data;
            return Task.CompletedTask;
        }

        private void OnSelectedItemChanged(DataCapture dataCapture)
        {
            Model.CaptureType = dataCapture.Type;
            Model.Name = dataCapture.Name;
            Model.Expression = dataCapture.Expression;
        }

        private void OnItemDoubleClicked(string colName, DataCapture dataCapture)
        {
            OnSelectedItemChanged(dataCapture);
            TestCapture();
            InvokeAsync(StateHasChanged);
        }

        private void OnItemClicked(string colName, DataCapture dataCapture)
        {
            switch(colName)
            {
              case nameof(DataCapture.Trash):
                  NatsService.Configuration.DataCaptures.Remove(dataCapture);
                  NatsService.Save();
                  DataCaptureGrid.Remove(dataCapture);                  
                  break;
              case nameof(DataCapture.Run):
                  OnSelectedItemChanged(dataCapture);
                  TestCapture();
                  break;
            }
            InvokeAsync(StateHasChanged);
        }

        protected void SaveCapture()
        {
            var dataCapture = new DataCapture{Expression = Model.Expression, Name = Model.Name, Type = Model.CaptureType};
            NatsService.Configuration.DataCaptures.Add(dataCapture);
            NatsService.Save();
            DataCaptureGrid.Insert(0, dataCapture);
        }

        protected void TestCapture()
        {
            if (string.IsNullOrEmpty(Model.Expression))
            {
                const string msg = "Expression is null or empty !";
                Logger.Warn(msg);
                Model.Result = msg;
                return;
            }

            if (string.IsNullOrEmpty(Model.Data))
            {
                const string msg = "Data is null or empty !";
                Logger.Warn(msg);
                Model.Result = msg;
                return;
            }
            
            switch (Model.CaptureType)
            {
                case CaptureType.Regex:
                    TestRegex();
                    break;
                case CaptureType.JsonPath:
                    TestJson();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(Model.CaptureType.ToString());
            }
        }

        protected void AddCommand()
        {
            var scriptStatement = new ScriptStatement { Param1 = Model.Expression, Param2 = Model.Result};
            scriptStatement.Name = Model.CaptureType == CaptureType.Regex ? nameof(CheckRegexCommand) : nameof(CheckJsonCommand);
            ScriptService.Current.Statements.Add(scriptStatement);
        }

        private void TestRegex()
        {
            Regex regex = new Regex(Model.Expression);
            var match = regex.Match(Model.Data);
            if (match.Success && match.Groups.Count > 1)
            {
                var capture = match.Groups[1];
                Model.Result = capture.Value;
            }
            else
            {
                Model.Result = $"Failed: {match.Name} / {match.Value}";
            }
        }

        private void TestJson()
        {
            try
            {
                IReadOnlyList<JsonElement> result = JsonPath.ExecutePath(Model.Expression, Model.Data);
                Model.Result = JsonSerializer.Serialize(result, new JsonSerializerOptions {WriteIndented = true});
            }
            catch (Exception e)
            {
                var msg = $"Failed to apply Json path: {e.Message}";
                Logger.Error(msg);
                Model.Result = msg;
            }
        }
    }
}