using C1.Blazor.Core;
using C1.Blazor.Grid;
using nats_ui.Data;

namespace nats_ui.Pages.Messages
{
    public class MessageCellFactory : GridCellFactory
    {
        public override void PrepareCellStyle(GridCellType cellType, GridCellRange range, C1Style style)
        {
            base.PrepareCellStyle(cellType, range, style);
            if (cellType != GridCellType.Cell)
            {
                return;
            }

            var selectedColumn = Grid.Columns[nameof(NatsMessage.Selected)];
            var isSelected = (bool) Grid[range.Row, selectedColumn.Index];
            if (isSelected)
            {
                style.BackgroundColor = C1Color.Gray;
            }
        }
    }
}