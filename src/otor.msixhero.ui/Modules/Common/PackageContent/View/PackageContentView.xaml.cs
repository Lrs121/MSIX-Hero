﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using Otor.MsixHero.Infrastructure.Logging;
using Otor.MsixHero.Ui.Modules.PackageList.Navigation;
using Prism.Regions;

namespace Otor.MsixHero.Ui.Modules.Common.PackageContent.View
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
            this.Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            
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

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            var dir = (string) ((Hyperlink) sender).Tag;
            Process.Start("explorer.exe", "/select," + Path.Combine(dir, "AppxManifest.xml"));
        }
    }
}
