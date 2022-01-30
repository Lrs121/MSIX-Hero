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
using System.Threading.Tasks;
using Otor.MsixHero.Appx.Editor.Commands.Concrete.Registry;
using Otor.MsixHero.Appx.Editor.Executors.Concrete.Registry;
using Otor.MsixHero.Cli.Verbs.Edit.Registry;
using Otor.MsixHero.Registry.Converter;

namespace Otor.MsixHero.Cli.Executors.Edit.Registry
{
    public class DeleteRegistryValueVerbExecutor : BaseEditVerbExecutor<DeleteRegistryValueEditVerb>
    {
        public DeleteRegistryValueVerbExecutor(string package, DeleteRegistryValueEditVerb verb, IConsole console) : base(package, verb, console)
        {
        }

        protected override async Task<int> ExecuteOnExtractedPackage(string directoryPath)
        {
            var command = new DeleteRegistryValue
            {
                RegistryKey = this.Verb.RegistryKey,
                RegistryValueName = this.Verb.RegistryValueName
            };

            var target = RegistryPathConverter.ToCanonicalRegistryPath(command.RegistryKey);
            try
            {
                await new DeleteRegistryValueExecutor(directoryPath).Execute(command).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await this.Console.WriteError(string.Format(Resources.Localization.CLI_Executor_DeleteRegistryValue_Error_Format, this.Verb.RegistryValueName, target.Item1, target.Item2)).ConfigureAwait(false);
                await this.Console.WriteError(e.Message).ConfigureAwait(false);
                return StandardExitCodes.ErrorGeneric;
            }

            await this.Console.WriteSuccess(string.Format(Resources.Localization.CLI_Executor_DeleteRegistryValue_Success_Format, this.Verb.RegistryValueName, target.Item1, target.Item2));
            return StandardExitCodes.ErrorSuccess;
        }
    }
}