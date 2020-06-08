using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JsonPathway;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using NLog;

namespace nats_ui.Pages.Inspector
{
    public class InspectorComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        protected string Data { get; set; }
        
        [Inject]
        protected InspectorService Inspector { get; set; }

        public PatternModel Model { get; set; } = new PatternModel();
        
        protected override Task OnInitializedAsync()
        {
            Data = Inspector.Data;
            return Task.CompletedTask;
        }

        protected void ApplyRegex()
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

        protected void ApplyJson()
        {
            IReadOnlyList<JsonElement> result = JsonPath.ExecutePath(Model.Pattern, Data);
            Model.Result = JsonSerializer.Serialize(result, new JsonSerializerOptions {WriteIndented = true});
        }
    }

    public class PatternModel
    {
        public string Pattern { get; set; }
        public string Result { get; set; }
    }
}