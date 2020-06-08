using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        public RegexModel RegexModel { get; set; } = new RegexModel();
        
        protected override Task OnInitializedAsync()
        {
            Data = Inspector.Data;
            return Task.CompletedTask;
        }

        protected void ApplyRegex()
        {
            Regex regex = new Regex(RegexModel.Pattern);
            var match = regex.Match(Data);
            if (match.Success && match.Groups.Count > 1)
            {
                var capture = match.Groups[1];
                RegexModel.Result = capture.Value;
            }
            else
            {
                RegexModel.Result = $"Failed: {match.Name} / {match.Value}";
            }
        }
    }

    public class RegexModel
    {
        public string Pattern { get; set; }
        public string Result { get; set; }
    }
}