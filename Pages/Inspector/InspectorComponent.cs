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
    public class InspectorComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        [Inject]
        protected InspectorService Inspector { get; set; }

        [Inject]
        protected ScriptService ScriptService { get; set; }

        protected string Data { get; set; }
        protected PatternModel Model { get; } = new PatternModel();
        
        protected override Task OnInitializedAsync()
        {
            Data = Inspector.Data;
            return Task.CompletedTask;
        }

        public void TestPattern()
        {
            if (string.IsNullOrEmpty(Model.Pattern))
            {
                var msg = "Pattern is null or empty !";
                Logger.Warn(msg);
                Model.Result = msg;
                return;
            }
            if (string.IsNullOrEmpty(Data))
            {
                var msg = "Data is null or empty !";
                Logger.Warn(msg);
                Model.Result = msg;
                return;
            }
            switch (Model.Type)
            {
                case PatternType.Regex:
                    TestRegex();
                    break;
                case PatternType.Json:
                    TestJson();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(Model.Type.ToString());
            }
        }

        public void AddCommand()
        {
            var scriptStatement = new ScriptStatement { Param1 = Model.Pattern, Param2 = Model.Result};
            scriptStatement.Name = Model.Type == PatternType.Regex ? nameof(CheckRegexCommand) : nameof(CheckJsonCommand);
            ScriptService.Current.Statements.Add(scriptStatement);
        }

        protected void TestRegex()
        {
            Regex regex = new Regex(Model.Pattern);
            var match = regex.Match(Data);
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

        protected void TestJson()
        {
            try
            {
                IReadOnlyList<JsonElement> result = JsonPath.ExecutePath(Model.Pattern, Data);
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

    public enum PatternType
    {
        Json,
        Regex
    }

    public class PatternModel
    {
        public string Pattern { get; set; }
        public string Result { get; set; }
        public PatternType Type { get; set; }
    }
}