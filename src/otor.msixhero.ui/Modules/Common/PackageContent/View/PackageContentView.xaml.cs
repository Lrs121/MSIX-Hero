﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using otor.msixhero.lib.Infrastructure;
using otor.msixhero.lib.Infrastructure.Logging;
using otor.msixhero.ui.Modules.PackageList.Navigation;
using Prism.Regions;

namespace otor.msixhero.ui.Modules.Common.PackageContent.View
{
    /// <summary>
    /// Interaction logic for PackageContentView.xaml
    /// </summary>
    public partial class PackageContentView : INavigationAware
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PackageContentView));

        public PackageContentView()
        {
            InitializeComponent();
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            var navigationParameters = new PackageListNavigation(navigationContext);
            return navigationParameters.SelectedManifests.Count == 1;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void HyperlinkOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var psi = new ProcessStartInfo((string)((Hyperlink)sender).Tag)
                {
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }
    }
}
