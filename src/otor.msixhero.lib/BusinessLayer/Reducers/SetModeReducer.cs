﻿using System.Threading;
using System.Threading.Tasks;
using otor.msixhero.lib.BusinessLayer.Appx;
using otor.msixhero.lib.BusinessLayer.State;
using otor.msixhero.lib.Domain.Commands;
using otor.msixhero.lib.Domain.Commands.Generic;
using otor.msixhero.lib.Domain.Events;
using otor.msixhero.lib.Infrastructure;

namespace otor.msixhero.lib.BusinessLayer.Reducers
{
    public class SetModeReducer : BaseReducer
    {
        private readonly SetMode command;

        public SetModeReducer(SetMode command, IWritableApplicationStateManager state) : base(command, state)
        {
            this.command = command;
        }

        public override Task Reduce(IInteractionService interactionService, IAppxPackageManager packageManager, CancellationToken cancellationToken = default)
        {
            if (this.StateManager.CurrentState.Mode != this.command.Mode)
            {
                this.StateManager.CurrentState.Mode = this.command.Mode;
                this.StateManager.EventAggregator.GetEvent<ApplicationModeChangedEvent>().Publish(this.command.Mode);
            }

            return Task.FromResult(true);
        }
    }
}