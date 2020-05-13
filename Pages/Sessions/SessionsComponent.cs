using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using NLog;

namespace nats_ui.Pages.Sessions
{
    public class SessionsComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        protected class SessionModel
        {
            public string Name { get; set; }
            public override string ToString() => $"{Name}";
        }

        [Inject]
        private NatsService NatsService { get; set; }

        protected SessionModel Model { get; } = new SessionModel();
        protected StandardGridModel<Session> Sessions { get; } = new StandardGridModel<Session>();
        protected SessionCellFactory GridCellFactory { get; } = new SessionCellFactory();

        protected override Task OnInitializedAsync()
        {
            Sessions.SetData(NatsService.Configuration.Sessions);
            Sessions.SelectedItemChanged += OnSelectedItemChanged;
            Sessions.ItemDoubleClicked += OnItemDoubleClicked;
            return Task.CompletedTask;
        }

        protected void CreateSession()
        {
            Logger.Info($"{nameof(CreateSession)}: {Model}");
            var session = new Session(Model.Name)
            {
                Checked = false,
            };
            
            if (NatsService.Create(session, out _))
            {
                Sessions.Insert(0, session);
            }
        }

        protected void RemoveSession()
        {
            Logger.Info(nameof(RemoveSession));
            foreach(var (i, session) in Sessions.GetCheckedItems())
            {
                Logger.Info($"{nameof(RemoveSession)}: {session}");
                NatsService.Remove(session);
                Sessions.Remove(i);
            }
        }
        
        private void OnSelectedItemChanged(Session session)
        {
            Model.Name = session.Name;
            InvokeAsync(StateHasChanged);
        }

        private void OnItemDoubleClicked(int index, Session session)
        {
            NatsService.Init(session);
        }

        protected void Save()
        {
            NatsService.Save();
        }
    }
}