﻿using System.Threading;
using System.Threading.Tasks;
using otor.msixhero.lib.BusinessLayer.Appx;
using otor.msixhero.lib.BusinessLayer.State;
using otor.msixhero.lib.Domain.Appx.Packages;
using otor.msixhero.lib.Domain.Commands.Packages.Developer;
using otor.msixhero.lib.Infrastructure;

namespace otor.msixhero.lib.BusinessLayer.Reducers
{
    internal class GetRegistryMountStateReducer : SelfElevationReducer<RegistryMountState>
    {
        private readonly GetRegistryMountState action;

        public GetRegistryMountStateReducer(GetRegistryMountState action, IWritableApplicationStateManager stateManager) : base(action, stateManager)
        {
            this.action = action;
        }

        public override Task<RegistryMountState> GetReduced(IInteractionService interactionService, IAppxPackageManager packageManager, CancellationToken cancellationToken = default)
        {
            return packageManager.GetRegistryMountState(this.action.InstallLocation, this.action.PackageName, cancellationToken);
        }
    }
}
