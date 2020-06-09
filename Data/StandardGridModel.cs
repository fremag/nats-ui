using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using C1.Blazor.Grid;
using C1.DataCollection;

namespace nats_ui.Data
{
    public delegate void ItemClickedDelegate<T>(string colName, T item);
    public delegate void ItemDoubleClickedDelegate<T>(string colName, T item);
    
    public class StandardGridModel<T> : IEnumerable<T> where T : class, ICheckable
    {
        public C1DataCollection<T> Items { get; private set; }
        public event Action<T> SelectedItemChanged;
        public event ItemClickedDelegate<T> ItemClicked;
        public event ItemDoubleClickedDelegate<T> ItemDoubleClicked;
        
        public IEnumerator<T> GetEnumerator() => Items.OfType<T>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void SetData(IEnumerable<T> data)
        {
            Items = new C1DataCollection<T>(new List<T>(data));
        }

        public void Insert(int index, T item)
        {
            if (index >= Items.Count)
            {
                Add(item);
                return;
            }
            Items.InsertAsync(index, item);
        }
            
        public void Add(T item)
        {
            Items.AddAsync(item);
        }

        public void Remove(int index)
        {
            Items.RemoveAsync(index);
        }

        public void Swap(int indexA, int indexB)
        {
            if (indexA < 0 || indexB < 0 || indexA >= Items.Count || indexB >= Items.Count)
            {
                return;
            }
            
            var itemA = Items[indexA];
            var itemB = Items[indexB];
            Items.ReplaceAsync(indexA, itemB);
            Items.ReplaceAsync(indexB, itemA);
        }
        
        private T GetSelected(GridCellRange cellRange)
        {
            if (cellRange == null || cellRange.Row < 0 || cellRange.Row >= Items.Count)
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
                selected.Checked = !selected.Checked;
            }

            var grid = (FlexGrid) sender;
            var colName = grid.Columns[e.CellRange.Column].Binding;
            ItemClicked?.Invoke(colName, selected);
        }

        public void OnCellDoubleTaped(object sender, GridInputEventArgs e)
        {
            var selected = GetSelected(e.CellRange);
            if (selected == null)
            {
                return;
            }
            var grid = (FlexGrid) sender;
            var colName = grid.Columns[e.CellRange.Column].Binding;
            ItemDoubleClicked?.Invoke(colName, selected);
        }

        public void Remove(in int index)
        {
            Items.RemoveAsync(index);
        }

        public IEnumerable<(int i, T item)> GetCheckedItems()
        {
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                if (Items[i] is T item && item.Checked)
                {
                    yield return (i, item);
                }
            }
        }

        public void CheckAll()
        {
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                if (Items[i] is T item)
                {
                    item.Checked = true;
                }
            }
        }

        public void Update(T item)
        {
            var idx = Items.IndexOf(item);
            Items.ReplaceAsync(idx, item);
        }

        public void Remove(T item)
        {
            int idx = Items.IndexOf(item);
            if (idx >= 0)
            {
                Remove(idx);
            }
        }
    }
}