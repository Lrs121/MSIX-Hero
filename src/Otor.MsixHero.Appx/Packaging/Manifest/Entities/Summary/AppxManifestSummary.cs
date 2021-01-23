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

using System;
using System.Collections.Generic;
using Otor.MsixHero.Appx.Packaging.Installation.Enums;

namespace Otor.MsixHero.Appx.Packaging.Manifest.Entities.Summary
{
    [Serializable]
    public class AppxManifestSummary
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public string ProcessorArchitecture { get; set; }

        public string Publisher { get; set; }

        public string Logo { get; set; }

        public bool IsFramework { get; set; }

        public string DisplayName { get; set; }
        
        public MsixPackageType PackageType { get; set; }

        public string DisplayPublisher { get; set; }

        public string Description { get; set; }

        public string AccentColor { get; set; }

        // public List<OperatingSystemDependency> OperatingSystemDependencies { get; set; }

        // public List<PackageDependency> PackageDependencies { get; set; }

        public Dictionary<string, string> BuildMetaData { get; set; }
    }
}
