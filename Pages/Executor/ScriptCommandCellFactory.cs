using C1.Blazor.Core;
using C1.Blazor.Grid;
using nats_ui.Data.Scripts;

namespace nats_ui.Pages.Executor
{
    public class ScriptCommandCellFactory : GridCellFactory
    {
        public override void PrepareCellStyle(GridCellType cellType, GridCellRange range, C1Style style)
        {
            base.PrepareCellStyle(cellType, range, style);
            if (cellType != GridCellType.Cell)
            {
                return;
            }

            var selectedColumn = Grid.Columns[nameof(IScriptCommand.Checked)];

            var value = Grid[range.Row, selectedColumn.Index];
            if (value == null)
            {
                return;
            }
            
            var isSelected = (bool)value;
            if (isSelected)
            {
                style.BackgroundColor = C1Color.Gray;
            }
        }
    }
}