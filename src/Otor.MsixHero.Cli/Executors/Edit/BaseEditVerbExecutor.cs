﻿using Otor.MsixHero.Appx.Packaging;
using Otor.MsixHero.Infrastructure.Helpers;
using Otor.MsixHero.Infrastructure.ThirdParty.Sdk;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Otor.MsixHero.Appx.Editor.Facades;
using Otor.MsixHero.Appx.Editor.Helpers;
using Otor.MsixHero.Cli.Verbs.Edit;

namespace Otor.MsixHero.Cli.Executors.Edit
{
    public abstract class BaseEditVerbExecutor<T> : VerbExecutor<T> where T : BaseEditVerb
    {
        private readonly string package;

        protected BaseEditVerbExecutor(string package, T verb, IConsole console) : base(verb, console)
        {
            this.package = package;
        }
        
        public override async Task<int> Execute()
        {
            try
            {
                await this.OnBegin().ConfigureAwait(false);

                if (!File.Exists(this.package) && !Directory.Exists(this.package))
                {
                    await this.Console.WriteError($"The path {this.package} does not exist.");
                    return 10;
                }

                var validation = await this.Validate().ConfigureAwait(false);
                if (validation != 0)
                {
                    return validation;
                }

                if (File.Exists(this.package))
                {
                    // This is a file...
                    if (string.Equals(Path.GetFileName(this.package), FileConstants.AppxManifestFile, StringComparison.OrdinalIgnoreCase))
                    {
                        // .. a manifest file
                        var result = await this.ExecuteOnExtractedPackage(Path.GetDirectoryName(this.package)).ConfigureAwait(false);
                        await this.OnFinished().ConfigureAwait(false);
                        return result;
                    }

                    if (string.Equals(".msix", Path.GetExtension(this.package)))
                    {
                        // .. an MSIX package
                        var msixMgr = new MakeAppxWrapper();
                        var tempFolder = Path.Combine(Path.GetTempPath(), "msixhero-" + Guid.NewGuid().ToString("N").Substring(0, 8));

                        try
                        {
                            await this.Console.WriteInfo($"Opening {Path.GetFileName(this.package)}...").ConfigureAwait(false);

                            // 1) Unpack first
                            await msixMgr.UnpackPackage(this.package, tempFolder, false).ConfigureAwait(false);

                            // 2) Make edit
                            var result = await this.ExecuteOnExtractedPackage(tempFolder).ConfigureAwait(false);
                            if (result != StandardExitCodes.ErrorSuccess)
                            {
                                await this.Console.WriteWarning($"The package has not been updated due to previous errors.").ConfigureAwait(false);
                                return result;
                            }

                            // 3) Add branding
                            XDocument document;
                            await using (var fs = File.OpenRead(Path.Combine(tempFolder, "AppxManifest.xml")))
                            {
                                document = await XDocument.LoadAsync(fs, LoadOptions.None, CancellationToken.None).ConfigureAwait(false);
                            }

                            var inject = new MsixHeroBrandingInjector();
                            await inject.Inject(document).ConfigureAwait(false);
                            var writer = new AppxDocumentWriter(document);
                            await writer.WriteAsync(Path.Combine(tempFolder, "AppxManifest.xml")).ConfigureAwait(false);

                            if (result == StandardExitCodes.ErrorSuccess)
                            {
                                await this.Console.WriteInfo($"Saving {Path.GetFileName(this.package)}...").ConfigureAwait(false);
                                // 3) Pack again
                                await msixMgr.PackPackageDirectory(tempFolder, this.package, false, false);
                                await this.OnFinished().ConfigureAwait(false);
                            }

                            return result;
                        }
                        finally
                        {
                            if (Directory.Exists(tempFolder))
                            {
                                ExceptionGuard.Guard(() => Directory.Delete(tempFolder, true));
                            }
                        }
                    }
                }
                else if (Directory.Exists(this.package))
                {
                    // this is extracted directory
                    var manifestPath = Path.Combine(this.package, FileConstants.AppxManifestFile);
                    if (File.Exists(manifestPath))
                    {
                        var result = await this.ExecuteOnExtractedPackage(this.package).ConfigureAwait(false);
                        
                        XDocument document;
                        await using (var fs = File.OpenRead(manifestPath))
                        {
                            document = await XDocument.LoadAsync(fs, LoadOptions.None, CancellationToken.None).ConfigureAwait(false);
                        }

                        var inject = new MsixHeroBrandingInjector();
                        await inject.Inject(document).ConfigureAwait(false);
                        var writer = new AppxDocumentWriter(document);
                        await writer.WriteAsync(manifestPath).ConfigureAwait(false);

                        await this.OnFinished().ConfigureAwait(false);
                        return result;
                    }
                }

                await this.Console.WriteError($"The path {this.package} is neither a directory with extracted MSIX, an .MSIX package or a manifest file.").ConfigureAwait(false);
                return StandardExitCodes.ErrorParameter;
            }
            catch (Exception e)
            {
                await this.Console.WriteError(e.Message).ConfigureAwait(false);
                return StandardExitCodes.ErrorGeneric;
            }
        }

        protected abstract Task<int> ExecuteOnExtractedPackage(string directoryPath);

        protected virtual Task<int> Validate()
        {
            return Task.FromResult(0);
        }

        protected virtual Task OnFinished()
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnBegin()
        {
            return Task.CompletedTask;
        }
    }
}
