using System.Linq;
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

        protected class UrlData : ICheckable
        {
            public string Url { get; set; }
            public bool Checked { get; set; }
        }
        
        protected class SubjectData : ICheckable
        {
            public string Subject { get; set; }
            public bool Checked { get; set; }
        }

        [Inject]
        private NatsService NatsService { get; set; }

        protected SessionModel Model { get; } = new SessionModel();
        protected string SelectedName { get; set; }
        
        protected StandardGridModel<Session> Sessions { get; } = new StandardGridModel<Session>();
        protected StandardGridModel<UrlData> Urls { get; } = new StandardGridModel<UrlData>();
        protected StandardGridModel<SubjectData> Subjects { get; } = new StandardGridModel<SubjectData>();
        protected SessionCellFactory GridCellFactory { get; } = new SessionCellFactory();
 
        protected override Task OnInitializedAsync()
        {
            Sessions.SetData(NatsService.Configuration.Sessions);
            Urls.SetData(new UrlData[0]);
            Subjects.SetData(new SubjectData[0]);
            Sessions.SelectedItemChanged += OnSelectedItemChanged;
            Sessions.ItemDoubleClicked += OnItemDoubleClicked;
            Sessions.ItemClicked += OnItemClicked;
            return Task.CompletedTask;
        }

        protected void CreateSession()
        {
            Logger.Info($"{nameof(CreateSession)}: {Model}");
            var session = new Session(Model.Name)
            {
                Checked = false,
            };

            NatsService.Create(session, out _);
            NatsService.Save();
            Sessions.SetData(NatsService.Configuration.Sessions);
            OnSelectedItemChanged(session);
        }

        protected void RemoveSession(Session session)
        {
            Logger.Info($"{nameof(RemoveSession)}: {session}");
            NatsService.Remove(session);
            Sessions.Remove(session);
        }
        
        private void OnSelectedItemChanged(Session session)
        {
            Model.Name = session.Name;
            SelectedName = session.Name;
            Subjects.SetData(session.Subscriptions.Select(sub => sub.Subject).Select(subj => new SubjectData {Subject = subj}));
            Urls.SetData(session.Connections.Select(conn => conn.Url).Select(url => new UrlData {Url = url}));
            InvokeAsync(StateHasChanged);
        }

        private void OnItemDoubleClicked(string colName, Session session)
        {
            NatsService.Init(session);
        }

        private void OnItemClicked(string colName, Session session)
        {
            switch (colName)
            {
                case nameof(Session.Trash):
                    RemoveSession(session);
                    break;
                case nameof(Session.Run):
                    NatsService.Init(session);
                    break;
            }
        }
    }
}