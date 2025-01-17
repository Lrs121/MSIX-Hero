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
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Otor.MsixHero.App.Helpers;

namespace Otor.MsixHero.App.Mvvm.Converters
{
    public class GroupDateConverter : IValueConverter
    {
        private GroupDateConverter()
        {
        }

        public static GroupDateConverter Instance = new GroupDateConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue)
            {
                return Binding.DoNothing;
            }

            if (!(value is DateTime date))
            {
                return value;
            }

            return HumanizedDateHelper.GetHumanizedDate(date);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}