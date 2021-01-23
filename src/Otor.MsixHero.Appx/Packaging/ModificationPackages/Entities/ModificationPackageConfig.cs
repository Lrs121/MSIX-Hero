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

using System.IO;

namespace Otor.MsixHero.Appx.Packaging.ModificationPackages.Entities
{
    public class ModificationPackageConfig
    {
        public string ParentName { get; set; }

        public string ParentPublisher { get; set; }

        public string Name { get; set; }
        
        public string DisplayName { get; set; }

        public string DisplayPublisher { get; set; }

        public string Publisher { get; set; }

        public string Version { get; set; }
        
        public string ParentPackagePath { get; set; }

        public bool IncludeVfsFolders { get; set; }

        public DirectoryInfo IncludeFolder { get; set; }

        public FileInfo IncludeRegistry { get; set; }
    }
}
