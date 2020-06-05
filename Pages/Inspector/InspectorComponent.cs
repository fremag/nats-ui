using System.Threading.Tasks;
using BlazorStrap;
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
        
        protected override Task OnInitializedAsync()
        {
            Data = Inspector.Data;
            return Task.CompletedTask;
        }

        const string Plus = "oi oi-plus";
        const string Minus = "oi oi-minus";
        protected string RegexText { get; set; } = Plus;
        protected string JsonText { get; set; } = Plus;
        protected void OnRegexShowEvent(BSCollapseEvent ev) => RegexText = Minus;
        protected void OnRegexHideEvent(BSCollapseEvent ev) => RegexText = Plus;
        protected void OnJsonShowEvent(BSCollapseEvent ev) => JsonText = Minus;
        protected void OnJsonHideEvent(BSCollapseEvent ev) => JsonText = Plus;
    }
}