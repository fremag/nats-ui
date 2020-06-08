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
        protected PatternModel Model { get; set; } = new PatternModel();
        
        protected override Task OnInitializedAsync()
        {
            Data = Inspector.Data;
            return Task.CompletedTask;
        }

        public void Test()
        {
            if (Model.Type == PatternType.Regex.ToString())
            {
                TestRegex();
            }
            else
            {
                TestJson();
            }
        }
        

        public void Add()
        {
            if (Model.Type == PatternType.Regex.ToString())
            {
                ScriptService.Current.Statements.Add(new ScriptStatement { Name = nameof(CheckRegexCommand), Param1 = Model.Pattern, Param2 = Model.Result});
            }
            else
            {
                ScriptService.Current.Statements.Add(new ScriptStatement { Name = nameof(CheckJsonCommand), Param1 = Model.Pattern, Param2 = Model.Result});
            }
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
            IReadOnlyList<JsonElement> result = JsonPath.ExecutePath(Model.Pattern, Data);
            Model.Result = JsonSerializer.Serialize(result, new JsonSerializerOptions {WriteIndented = true});
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
        public string Type { get; set; }
    }
}