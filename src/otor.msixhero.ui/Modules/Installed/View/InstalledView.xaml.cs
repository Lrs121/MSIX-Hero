﻿using System.Windows.Controls;

namespace MSI_Hero.Modules.Installed.View
{
    /// <summary>
    /// Interaction logic for InstalledView.xaml
    /// </summary>
    public partial class InstalledView : UserControl
    {
        public InstalledView()
        {
            InitializeComponent();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*var viewModel = ((ListView)sender).DataContext as PackageListViewModel;
            if (viewModel == null)
            {
                return;
            }

            if (e.AddedItems != null)
            {
                foreach (var newItem in e.AddedItems.OfType<PackageViewModel>())
                {
                    viewModel.SelectedPackages.Add(newItem);
                }
            }

            if (e.RemovedItems != null)
            {
                foreach (var oldItem in e.RemovedItems.OfType<PackageViewModel>())
                {
                    viewModel.SelectedPackages.Remove(oldItem);
                }
            }

            if (this.TabMaintenance.IsSelected || this.TabDeveloper.IsSelected)
            {
                return;
            }

            if (viewModel.SelectedPackages.Any())
            {
                this.TabMaintenance.IsSelected = true;
            }*/
        }
    }
}
