﻿using System.Threading;
using System.Threading.Tasks;
using otor.msixhero.lib.BusinessLayer.Appx;
using otor.msixhero.lib.BusinessLayer.State;
using otor.msixhero.lib.Domain.Appx.Manifest.Full;
using otor.msixhero.lib.Domain.Commands.Grid;
using otor.msixhero.lib.Infrastructure;

namespace otor.msixhero.lib.BusinessLayer.Reducers
{
    internal class GetPackageDetailsReducer : BaseReducer<AppxPackage>
    {
        private readonly GetPackageDetails command;

        public GetPackageDetailsReducer(GetPackageDetails command, IWritableApplicationStateManager stateManager) : base(command, stateManager)
        {
            this.command = command;
        }

        public override async Task<AppxPackage> GetReduced(IInteractionService interactionService, IAppxPackageManager packageManager, CancellationToken cancellationToken = default)
        {
            var details = await packageManager.Get(this.command.PackageFullName, cancellationToken).ConfigureAwait(false);
            return details;
        }
    }
}
