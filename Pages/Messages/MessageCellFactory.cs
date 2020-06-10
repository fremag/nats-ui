using C1.Blazor.Core;
using nats_ui.Data;

namespace nats_ui.Pages.Messages
{
    public class MessageCellFactory : StandardCellFactory<NatsMessage>
    {
        protected override void PrepareCellStyle(string colName, NatsMessage item, C1Style cellType)
        {
            
        }
    }
}