using C1.Blazor.Core;
using C1.Blazor.Grid;

namespace nats_ui.Data
{
    public abstract class StandardCellFactory<T> : GridCellFactory where T : ICheckable
    {
        protected abstract void PrepareCellStyle(string colName, T item, C1Style cellType);

        public override void PrepareCellStyle(GridCellType cellType, GridCellRange range, C1Style style)
        {
            base.PrepareCellStyle(cellType, range, style);
            if (cellType != GridCellType.Cell)
            {
                return;
            }

            var item = (T) Grid.Rows[range.Row].DataItem;
            string colName = Grid.Columns[range.Column].Binding;
            
            if (colName == nameof(ICheckable.Checked))
            {
                if (item.Checked)
                {
                    style.BackgroundColor = C1Color.Gray;
                }

                return;
            }

            PrepareCellStyle(colName, item, style);
        }
    }
}