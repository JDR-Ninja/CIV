using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.ComponentModel;

namespace CIV.UserControls
{
    public class ListViewSortable : ListView
    {
        private readonly IDictionary<ListView, GridViewColumnHeader> _sortColumns = new Dictionary<ListView, GridViewColumnHeader>();
        private readonly IDictionary<ListView, SortAdorner> _sortAdorners = new Dictionary<ListView, SortAdorner>();

        public ListViewSortable()
        {
            this.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(Column_Click));
        }

        private void Column_Click(object sender, RoutedEventArgs e)
        {
            // Autre méthode : http://thejoyofcode.com/Sortable_ListView_in_WPF.aspx
            if (e.OriginalSource is GridViewColumnHeader)
            {
                try
                {
                    var listView = (ListView)sender;

                    var column = (GridViewColumnHeader)e.OriginalSource;

                    if (((GridViewColumnHeader)e.OriginalSource).Column.DisplayMemberBinding != null)
                    {
                        var field = (string)((Binding)((GridViewColumnHeader)e.OriginalSource).Column.DisplayMemberBinding).Path.Path;

                        if (!_sortColumns.ContainsKey(listView))
                            _sortColumns.Add(listView, null);

                        if (!_sortAdorners.ContainsKey(listView))
                            _sortAdorners.Add(listView, null);

                        if (_sortColumns[listView] != null)
                        {
                            AdornerLayer.GetAdornerLayer(_sortColumns[listView]).Remove(_sortAdorners[listView]);
                            listView.Items.SortDescriptions.Clear();
                        }

                        var newDir = ListSortDirection.Ascending;
                        if (_sortColumns[listView] == column && _sortAdorners[listView].Direction == newDir)
                        {
                            newDir = ListSortDirection.Descending;
                        }

                        _sortColumns[listView] = column;
                        _sortAdorners[listView] = new SortAdorner(_sortColumns[listView], newDir);
                        AdornerLayer.GetAdornerLayer(_sortColumns[listView]).Add(_sortAdorners[listView]);
                        listView.Items.SortDescriptions.Add(new SortDescription(field, newDir));
                    }
                }
                catch
                {
                    
                }
            }
        }
    }
}
