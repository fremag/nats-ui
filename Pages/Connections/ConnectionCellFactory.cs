using C1.Blazor.Core;
using C1.Blazor.Grid;
using nats_ui.Data;

namespace nats_ui.Pages.Connections
{
    public class ConnectionCellFactory : GridCellFactory
    {
        public override void PrepareCellStyle(GridCellType cellType, GridCellRange range, C1Style style)
        {
            base.PrepareCellStyle(cellType, range, style);
            if (cellType != GridCellType.Cell)
            {
                return;
            }

            var statusColumn = Grid.Columns[nameof(Connection.Status)];
            var status = (ConnectionStatus) Grid[range.Row, statusColumn.Index];
            if (range.Column == statusColumn.Index && status == ConnectionStatus.Connected)
            {
                style.BackgroundColor = C1Color.Green;
                return;
            }

            var selectedColumn = Grid.Columns[nameof(Connection.Selected)];
            var isSelected = (bool) Grid[range.Row, selectedColumn.Index];
            if (isSelected)
            {
                style.BackgroundColor = C1Color.Gray;
            }
        }
    }
}