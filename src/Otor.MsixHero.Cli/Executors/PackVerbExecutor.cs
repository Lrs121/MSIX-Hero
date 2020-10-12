﻿using System;
using System.Threading.Tasks;
using Otor.MsixHero.Cli.Verbs;
using Otor.MsixHero.Infrastructure.Logging;
using Otor.MsixHero.Infrastructure.ThirdParty.Exceptions;
using Otor.MsixHero.Infrastructure.ThirdParty.Sdk;

namespace Otor.MsixHero.Cli.Executors
{
    public class PackVerbExecutor : VerbExecutor<PackVerb>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PackVerbExecutor));

        public PackVerbExecutor(PackVerb verb, IConsole console) : base(verb, console)
        {
        }

        public override async Task<int> Execute()
        {
            var msixSdkWrapper = new MsixSdkWrapper();

            Logger.Info($"Packing [{this.Verb.Directory}] to [{this.Verb.Package}]...");

            try
            {
                await this.Console.WriteInfo($"Packing [{this.Verb.Directory}] to [{this.Verb.Package}]...").ConfigureAwait(false);
                await msixSdkWrapper.PackPackageDirectory(this.Verb.Directory, this.Verb.Package, !this.Verb.NoCompression, !this.Verb.NoValidation).ConfigureAwait(false);

                await this.Console.WriteSuccess($"Package [{this.Verb.Package}] has been created.");
                return 0;
            }
            catch (SdkException e)
            {
                Logger.Error(e);
                await this.Console.WriteError(e.Message);
                return e.ExitCode;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                await this.Console.WriteError(e.Message);
                return 1;
            }
        }
    }
}