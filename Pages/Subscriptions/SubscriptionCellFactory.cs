using C1.Blazor.Core;
using C1.Blazor.Grid;
using nats_ui.Data;

namespace nats_ui.Pages.Subscriptions
{
    public class SubscriptionCellFactory : GridCellFactory
    {
        public override void PrepareCellStyle(GridCellType cellType, GridCellRange range, C1Style style)
        {
            base.PrepareCellStyle(cellType, range, style);
            if (cellType != GridCellType.Cell)
            {
                return;
            }

            var subscribedColumn = Grid.Columns[nameof(NatsSubscription.Subscribed)];
            var subscribed = (bool) Grid[range.Row, subscribedColumn.Index];
            if (range.Column == subscribedColumn.Index && subscribed)
            {
                style.BackgroundColor = C1Color.Green;
                return;
            }

            var selectedColumn = Grid.Columns[nameof(NatsSubscription.Checked)];
            var isSelected = (bool) Grid[range.Row, selectedColumn.Index];
            if (isSelected)
            {
                style.BackgroundColor = C1Color.Gray;
            }
        }
    }
}