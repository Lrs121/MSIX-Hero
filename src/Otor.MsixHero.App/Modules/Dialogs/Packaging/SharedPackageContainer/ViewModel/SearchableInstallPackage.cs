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

using Otor.MsixHero.App.Modules.PackageManagement.PackageList.ViewModels;
using Otor.MsixHero.Appx.Packaging.Installation.Entities;

namespace Otor.MsixHero.App.Modules.Dialogs.Packaging.SharedPackageContainer.ViewModel;

public class SearchableInstallPackage : InstalledPackageViewModel
{
    private bool _isEnabled = true;
    private bool _isVisible = true;

    public SearchableInstallPackage(InstalledPackage package) : base(package)
    {
    }
    
    public bool IsVisible
    {
        get => this._isVisible;
        set => this.SetField(ref this._isVisible, value);
    }

    public bool IsEnabled
    {
        get => this._isEnabled;
        set => this.SetField(ref this._isEnabled, value);
    }
}