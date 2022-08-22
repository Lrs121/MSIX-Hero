﻿// MSIX Hero
// Copyright (C) 2022 Marcin Otorowski
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

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Otor.MsixHero.App.Events;
using Otor.MsixHero.App.Hero.Commands.Tools;
using Otor.MsixHero.App.Hero.Events.Base;
using Prism.Events;
using Prism.Regions;

namespace Otor.MsixHero.App.Modules.Tools.Views
{
    public partial class ToolsView : INavigationAware
    {
        private readonly IEventAggregator eventAggregator;

        public ToolsView(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            eventAggregator.GetEvent<UiExecutedEvent<SetToolFilterCommand>>().Subscribe(this.OnSetToolFilter, ThreadOption.UIThread);
            InitializeComponent();
        }

        private void OnSetToolFilter(UiExecutedPayload<SetToolFilterCommand> obj)
        {
            var host = this.Host;

            for (var i = 0; i < host.Children.Count; i++)
            {
                if (!(host.Children[i] is TextBlock text))
                {
                    continue;
                }

                var nextElement = i + 1 < host.Children.Count ? host.Children[i + 1] as WrapPanel : null;
                if (nextElement == null)
                {
                    text.Visibility = Visibility.Visible;
                }
                else
                {
                    var anyVisible = false;
                    bool allVisible;

                    if (string.IsNullOrEmpty(obj.Request.SearchKey) || text.Text.IndexOf(obj.Request.SearchKey, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        text.Visibility = Visibility.Visible;
                        allVisible = true;
                    }
                    else
                    {
                        text.Visibility = Visibility.Collapsed;
                        allVisible = false;
                    }

                    foreach (var button in nextElement.Children.OfType<Button>())
                    {
                        if (allVisible || string.IsNullOrEmpty(obj.Request.SearchKey))
                        {
                            anyVisible = true;
                            button.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            var buttonTexts = ((StackPanel)button.Content).Children.OfType<TextBlock>();

                            var hasText = buttonTexts.Any(b => b.Text?.IndexOf(obj.Request.SearchKey, StringComparison.OrdinalIgnoreCase) > -1);
                            anyVisible |= hasText;
                            button.Visibility = hasText ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }

                    if (allVisible || string.IsNullOrEmpty(obj.Request.SearchKey) || anyVisible)
                    {
                        text.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        text.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.eventAggregator.GetEvent<TopSearchWidthChangeEvent>().Publish(new TopSearchWidthChangeEventPayLoad(250.0));
        }
        
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}