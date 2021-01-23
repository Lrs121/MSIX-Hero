﻿// MSIX Hero
// Copyright (C) 2021 Marcin Otorowski
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// Full notice:
// https://github.com/marcinotorowski/msix-hero/blob/develop/LICENSE.md

using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using Otor.MsixHero.App.Controls;
using Otor.MsixHero.App.Modules.Dialogs.Updates.UpdateImpact.ViewModel;
using Otor.MsixHero.App.Modules.Dialogs.Updates.UpdateImpact.ViewModel.Items;

namespace Otor.MsixHero.App.Modules.Dialogs.Updates.UpdateImpact.View
{
    /// <summary>
    /// Interaction logic for UpdateImpactView.
    /// </summary>
    public partial class UpdateImpactView
    {
        private SortAdorner sortAdorner;
        private bool currentSortDescending = true;
        private string currentSortColumn;

        public UpdateImpactView()
        {
            this.InitializeComponent();
        }
        
        private void SetSorting(string columnName, bool descending)
        {
            SortAdorner newSortAdorner = null;

            var listView = this.Dialog.FindDialogTemplateName<ListView>("FileGrid");
            var gridView = (GridView)listView.View;

            foreach (var item in gridView.Columns.Select(c => c.Header).OfType<GridViewColumnHeader>().Where(c => c.Tag is string))
            {
                var layer = AdornerLayer.GetAdornerLayer(item);
                if (layer == null)
                {
                    break;
                }

                if (this.sortAdorner != null)
                {
                    layer.Remove(this.sortAdorner);
                }

                if ((string)item.Tag == columnName)
                {
                    newSortAdorner = new SortAdorner(item, descending ? ListSortDirection.Descending : ListSortDirection.Ascending);
                    layer.Add(newSortAdorner);
                    this.currentSortColumn = columnName;
                    break;
                }
            }

            this.sortAdorner = newSortAdorner;
            var value = ((UpdateImpactViewModel)this.DataContext).Results.CurrentValue;

            value?.FilesView.SortDescriptions.Clear();
            if (this.currentSortColumn != null)
            {
                value?.FilesView.SortDescriptions.Add(new SortDescription(this.currentSortColumn, this.currentSortDescending ? ListSortDirection.Descending : ListSortDirection.Ascending));
            }

            value?.FilesView.Refresh();
        }

        private void GridHeaderOnClick(object sender, RoutedEventArgs e)
        {
            var tag = (string)((GridViewColumnHeader)sender).Tag;

            if (this.currentSortColumn == tag)
            {
                this.currentSortDescending = !this.currentSortDescending;
            }
            else
            {
                this.currentSortDescending = false;
            }

            this.currentSortColumn = tag;

            this.SetSorting(this.currentSortColumn, this.currentSortDescending);
        }

        private void FilesDockVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && this.currentSortColumn == null)
            {
                Application.Current.Dispatcher.InvokeAsync(
                    () =>
                    {
                        this.SetSorting(nameof(FileViewModel.UpdateImpact), true);
                    },
                    DispatcherPriority.ApplicationIdle);
            }
        }

        private void Header1Clicked(object sender, MouseButtonEventArgs e)
        {
            var dataContext = (UpdateImpactViewModel) this.DataContext;
            var current = dataContext.Path1.CurrentValue;
            dataContext.Path1.Browse.Execute(null);
            if (current == dataContext.Path1.CurrentValue)
            {
                return;
            }

            dataContext.Compare.Execute(null);
        }

        private void Header2Clicked(object sender, MouseButtonEventArgs e)
        {
            var dataContext = (UpdateImpactViewModel)this.DataContext;
            var current = dataContext.Path2.CurrentValue;
            dataContext.Path2.Browse.Execute(null);
            if (current == dataContext.Path1.CurrentValue)
            {
                return;
            }

            dataContext.Compare.Execute(null);
        }
    }
}
