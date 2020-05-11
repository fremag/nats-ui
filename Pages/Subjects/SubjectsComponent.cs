using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C1.Blazor.Grid;
using C1.DataCollection;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using NLog;

namespace nats_ui.Pages.Subjects
{
    public class SubjectsComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        protected class SubjectModel
        {
            public string Subject { get; set; }
            public override string ToString() => $"{Subject}";
        }

        [Inject]
        private NatsService NatsService { get; set; }

        protected SubjectModel Model { get; } = new SubjectModel();
        protected C1DataCollection<NatsSubject> Subjects { get; private set; }
        protected SubjectCellFactory GridCellFactory { get; } = new SubjectCellFactory();

        protected override Task OnInitializedAsync()
        {
            NatsService.SubjectCreated += OnSubjectCreated;
            NatsService.SubjectRemoved += OnSubjectRemoved;
            Subjects = new C1DataCollection<NatsSubject>(new List<NatsSubject>(NatsService.Configuration.Subjects));
            return Task.CompletedTask;
        }


        private void OnSubjectRemoved(NatsSubject subject)
        {
            for (int i = Subjects.Count - 1; i >= 0; i--)
            {
                if (Subjects[i].Equals(subject))
                {
                    Subjects.RemoveAsync(i);
                    return;
                }
            }
        }

        private void OnSubjectCreated(NatsSubject subject)
        {
            Subjects.InsertAsync(0, subject);
        }

        protected void CreateSubject()
        {
            Logger.Info($"{nameof(CreateSubject)}: {Model}");
            if (Subjects.Contains(Model.Subject))
            {
                return;
            }

            NatsService.Create(new NatsSubject(Model.Subject)
            {
                Selected = false,
                Subscribed = false
            });
        }

        protected void RemoveSubjects()
        {
            Logger.Info(nameof(RemoveSubjects));
            foreach (var subject in Subjects.OfType<NatsSubject>().Where(natsSubject => natsSubject.Selected).ToArray())
            {
                Logger.Info($"RemoveSubject: {subject}");
                NatsService.Remove(subject);
            }

            InvokeAsync(StateHasChanged);
        }
        
        protected void SelectedSubjectChanged(object sender, GridCellRangeEventArgs e)
        {
            var selected = GetSelected(e.CellRange);
            if (selected == null)
            {
                return;
            }

            Model.Subject = selected.Subject;
            InvokeAsync(StateHasChanged);
        }

        private NatsSubject GetSelected(GridCellRange cellRange)
        {
            if (cellRange == null)
            {
                return null;
            }

            var selected = Subjects[cellRange.Row] as NatsSubject;
            return selected;
        }

        protected void OnCellTaped(object sender, GridInputEventArgs e)
        {
            var natsSubject = GetSelected(e.CellRange);
            if (natsSubject == null)
            {
                return;
            }

            if (e.CellRange.Column == 0)
            {
                natsSubject.Selected = !natsSubject.Selected;
            }
        }

        protected void OnCellDoubleTaped(object sender, GridInputEventArgs e)
        {
            var natsSubject = GetSelected(e.CellRange);
            if (natsSubject == null)
            {
                return;
            }

            if (natsSubject.Subscribed)
            {
                Unsubscribe(natsSubject);
            }
            else
            {
                Subscribe(natsSubject);
            }
        
        }

        private void Subscribe(NatsSubject natsSubject)
        {
            NatsService.Subscribe(natsSubject);
        }

        private void Unsubscribe(NatsSubject natsSubject)
        {
            NatsService.Unsubscribe(natsSubject);
        }
    }
}