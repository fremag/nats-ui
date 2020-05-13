using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using C1.Blazor.Grid;
using C1.DataCollection;

namespace nats_ui.Data
{
    public class StandardGridModel<T> : IEnumerable<T> where T : class, ISelectable
    {
        public C1DataCollection<T> Items { get; private set; }
        public event Action<T> SelectedItemChanged;
        public event Action<int, T> ItemClicked;
        public event Action<int, T> ItemDoubleClicked;
        
        public IEnumerator<T> GetEnumerator() => Items.OfType<T>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void SetData(IEnumerable<T> data)
        {
            Items = new C1DataCollection<T>(new List<T>(data));
        }

        public void Insert(int index, T message)
        {
            Items.InsertAsync(index, message);
        }
            
        private T GetSelected(GridCellRange cellRange)
        {
            if (cellRange == null)
            {
                return null;
            }

            var selected = Items[cellRange.Row] as T;
            return selected;
        }
            
        public void SelectionChanged(object sender, GridCellRangeEventArgs e)
        {
            var selected = GetSelected(e.CellRange);
            if (selected == null)
            {
                return;
            }
            
            SelectedItemChanged?.Invoke(selected);
        }
            
        public void OnCellTaped(object sender, GridInputEventArgs e)
        {
            var selected = GetSelected(e.CellRange);
            if (selected == null)
            {
                return;
            }
            if (e.CellRange.Column == 0)
            {
                selected.Selected = !selected.Selected;
            }
            ItemClicked?.Invoke(e.CellRange.Column, selected);
        }

        public void OnCellDoubleTaped(object sender, GridInputEventArgs e)
        {
            var selected = GetSelected(e.CellRange);
            if (selected == null)
            {
                return;
            }
            ItemDoubleClicked?.Invoke(e.CellRange.Column, selected);
        }

        public void Remove(in int index)
        {
            Items.RemoveAsync(index);
        }

        public IEnumerable<(int i, T item)> GetSelectedItems()
        {
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                if (Items[i] is T item && item.Selected)
                {
                    yield return (i, item);
                }
            }
        }
    }
}