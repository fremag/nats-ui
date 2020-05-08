using System.Collections.Generic;
using System.Threading.Tasks;
using C1.DataCollection;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using NLog;

namespace nats_ui.Pages
{
    public class NatsSessionComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        [Inject]
        private NatsService NatsService { get; set; }

        protected NatsSessionModel NatsSession { get; } = new NatsSessionModel();
        protected C1DataCollection<NatsSessionModel> sessions;
        
        protected void CreateSession()
        {
            Logger.Info($"CreateSession: {NatsSession}");
            NatsService.Create(NatsSession.Clone());
        }
        
        protected override async Task OnInitializedAsync()
        {
            NatsService.SessionCreated += OnSessionCreated;
            sessions = new C1DataCollection<NatsSessionModel>(new List<NatsSessionModel>(NatsService.Configuration.Sessions));
        }

        private void OnSessionCreated(NatsSessionModel session)
        {
            sessions.InsertAsync(0, session);
        }
    }
}