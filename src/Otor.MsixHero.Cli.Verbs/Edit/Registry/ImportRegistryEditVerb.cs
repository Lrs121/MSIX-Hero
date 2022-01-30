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

using CommandLine;
using Otor.MsixHero.Cli.Verbs.Resources;

namespace Otor.MsixHero.Cli.Verbs.Edit.Registry
{
    [Verb("importRegistry", HelpText = "CLI_Verbs_Edit_ImportRegistry_VerbName", ResourceType = typeof(Localization))]
    public class ImportRegistryEditVerb : BaseEditVerb, IBaseEditRegistryVerb
    {
        [Option('f', "file", HelpText = "CLI_Verbs_Edit_ImportRegistry_Prop_File", Required = true, SetName = "File", ResourceType = typeof(Localization))]
        public string FilePath { get; set; }

        [Option('k', "key", HelpText = "CLI_Verbs_Edit_ImportRegistry_Prop_RegistryKey", Required = true, SetName = "Local", ResourceType = typeof(Localization))]
        public string RegistryKey { get; set; }
    }
}