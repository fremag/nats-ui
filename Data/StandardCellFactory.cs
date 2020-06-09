using C1.Blazor.Core;
using C1.Blazor.Grid;

namespace nats_ui.Data
{
    public class StandardCellFactory : GridCellFactory
    {
        public override void PrepareCellStyle(GridCellType cellType, GridCellRange range, C1Style style)
        {
            base.PrepareCellStyle(cellType, range, style);
            if (cellType != GridCellType.Cell)
            {
                return;
            }

            int selectedColumnIdx = Grid.Columns.IndexOf(nameof(Session.Checked));
            if (selectedColumnIdx == -1)
            {
                return;
            } 
            var isSelected = (bool) Grid[range.Row, selectedColumnIdx];
            if (isSelected)
            {
                style.BackgroundColor = C1Color.Gray;
            }
        }
    }
}