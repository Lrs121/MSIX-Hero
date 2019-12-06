﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using otor.msixhero.lib.BusinessLayer.Appx;
using otor.msixhero.lib.BusinessLayer.Reducers;
using otor.msixhero.lib.BusinessLayer.State;
using otor.msixhero.lib.Domain.Commands;
using otor.msixhero.lib.Domain.Commands.Developer;
using otor.msixhero.lib.Domain.Commands.Grid;
using otor.msixhero.lib.Domain.Commands.Manager;
using otor.msixhero.lib.Domain.Commands.Signing;
using otor.msixhero.lib.Domain.Commands.UI;
using otor.msixhero.lib.Infrastructure.Helpers;
using otor.msixhero.lib.Infrastructure.Logging;

namespace otor.msixhero.lib.Infrastructure.Commanding
{
    public class CommandExecutor : ICommandExecutor
    {
        private static readonly ILog Logger = LogManager.GetLogger();

        private readonly IDictionary<Type, Func<BaseCommand, IReducer>> reducerFactories = new Dictionary<Type, Func<BaseCommand, IReducer>>();
        private readonly IAppxPackageManagerFactory appxPackageManagerFactory;
        private readonly IInteractionService interactionService;
        private readonly IBusyManager busyManager;
        private IWritableApplicationStateManager writableApplicationStateManager;

        public CommandExecutor(
            IAppxPackageManagerFactory appxPackageManagerFactory,
            IInteractionService interactionService,
            IBusyManager busyManager)
        {
            this.appxPackageManagerFactory = appxPackageManagerFactory;
            this.interactionService = interactionService;
            this.busyManager = busyManager;

            this.ConfigureReducers();
        }

        public void SetStateManager(IWritableApplicationStateManager stateManager)
        {
            this.writableApplicationStateManager = stateManager;
        }

        public void Execute(BaseCommand action)
        {
            try
            {
                this.ExecuteAsync(action, CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (AggregateException e)
            {
                throw e.Flatten().GetBaseException();
            }
        }

        public T GetExecute<T>(BaseCommand<T> action)
        {
            try
            {
                return this.GetExecuteAsync(action, CancellationToken.None).Result;
            }
            catch (AggregateException e)
            {
                throw e.Flatten().GetBaseException();
            }
        }

        public async Task ExecuteAsync(BaseCommand action, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!this.reducerFactories.TryGetValue(action.GetType(), out var reducerFactory))
                {
                    return;
                }

                var lazyReducer = reducerFactory(action);


                bool elevate;
                if (action is ISelfElevatedCommand selfElevateCommand)
                {
                    elevate = selfElevateCommand.RequiresElevation;

                    if (this.writableApplicationStateManager.CurrentState.IsElevated)
                    {
                        elevate = false;
                    }
                }
                else
                {
                    elevate = false;
                }

                var packageManager = elevate ? this.appxPackageManagerFactory.GetRemote() : this.appxPackageManagerFactory.GetLocal();
                await lazyReducer.Reduce(this.interactionService, packageManager, cancellationToken).ConfigureAwait(false);

                if (elevate && !this.writableApplicationStateManager.CurrentState.IsSelfElevated)
                {
                    this.writableApplicationStateManager.CurrentState.IsSelfElevated = true;
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Warn("Operation cancelled by the user.");
                throw;
            }
            catch (Win32Exception e)
            {
                Logger.Error(e, "Win32 error during command execution.");
                // If error code is 1223 it means that the user did not press YES in UAC.
                var message = e.NativeErrorCode == 1223 ? "This operation requires administrative rights." : e.Message;
                var result = this.interactionService.ShowError(message, extendedInfo: e.ToString());
                if (result == InteractionResult.Retry)
                {
                    Logger.Info("Retrying...");
                    await this.ExecuteAsync(action, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "General error during command execution.");
                var result = this.interactionService.ShowError(e.Message, extendedInfo: e.ToString());
                if (result == InteractionResult.Retry)
                {
                    Logger.Info("Retrying...");
                    await this.ExecuteAsync(action, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task<object> GetExecuteAsync(BaseCommand command, CancellationToken cancellationToken = default)
        {
            var resultType = GenericArgumentResolver.GetResultType(command.GetType(), typeof(BaseCommand<>));
            if (resultType == null)
            {
                throw new ArgumentException("The argument does not implement interface BaseCommand<T>.");
            }

            if (!this.reducerFactories.TryGetValue(command.GetType(), out var reducerFactory))
            {
                return default;
            }

            var lazyReducer = reducerFactory(command);
            var reducerResultType = GenericArgumentResolver.GetResultType(lazyReducer.GetType(), typeof(IReducer<>));
            if (reducerResultType == null)
            {
                throw new InvalidOperationException("The reducer does not implement the interface IReducer<,>.");
            }

            if (reducerResultType != resultType)
            {
                throw new InvalidOperationException("Mismatch between generic type of the command and from the reducer.");
            }

            var genericType = typeof(IReducer<>);
            var reducerGenericType = genericType.MakeGenericType(resultType);
            
            try
            {
                bool elevate;

                if (command is ISelfElevatedCommand selfElevateCommand)
                {
                    elevate = selfElevateCommand.RequiresElevation;

                    if (this.writableApplicationStateManager.CurrentState.IsElevated)
                    {
                        elevate = false;
                    }
                }
                else
                {
                    elevate = false;
                }

                var packageManager = elevate
                    ? this.appxPackageManagerFactory.GetRemote()
                    : this.appxPackageManagerFactory.GetLocal();

                
                return await Task.Run(() =>
                    {
                        var methodName = nameof(IReducer<object>.GetReduced);
                        // ReSharper disable once PossibleNullReferenceException
                        try
                        {
                            var invocationResult = reducerGenericType.GetMethod(methodName).Invoke(lazyReducer, new object[] { this.interactionService, packageManager, cancellationToken });

                            var taskType = typeof(Task<>).MakeGenericType(resultType);

                            // ReSharper disable once PossibleNullReferenceException
                            return taskType.GetProperty(nameof(Task<bool>.Result)).GetValue(invocationResult);
                        }
                        catch (AggregateException e)
                        {
                            throw e.GetBaseException();
                        }
                    }, 
                    cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                Logger.Warn("Operation cancelled by the user.");
                throw;
            }
            catch (Win32Exception e)
            {
                Logger.Error(e, "Win32 error during command execution.");
                // If error code is 1223 it means that the user did not press YES in UAC.
                var message = e.NativeErrorCode == 1223 ? "This operation requires administrative rights." : e.Message;
                var result = this.interactionService.ShowError(message, extendedInfo: e.ToString());

                if (result == InteractionResult.Retry)
                {
                    Logger.Info("Retrying..");
                    return await this.GetExecuteAsync(command, cancellationToken).ConfigureAwait(false);
                }

                return default;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Generic error during command execution.");
                var result = this.interactionService.ShowError(e.Message, extendedInfo: e.ToString());
                if (result == InteractionResult.Retry)
                {
                    Logger.Info("Retrying..");
                    return await this.GetExecuteAsync(command, cancellationToken).ConfigureAwait(false);
                }

                return default;
            }
        }

        public async Task<T> GetExecuteAsync<T>(BaseCommand<T> action, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!this.reducerFactories.TryGetValue(action.GetType(), out var reducerFactory))
                {
                    return default;
                }

                var lazyReducer = reducerFactory(action);
                var lazyReducerOutput = lazyReducer as IReducer<T>;
                if (lazyReducerOutput == null)
                {
                    throw new NotSupportedException("This reducer does not support output.");
                }

                bool elevate;

                if (action is ISelfElevatedCommand selfElevateCommand)
                {
                    elevate = selfElevateCommand.RequiresElevation;

                    if (this.writableApplicationStateManager.CurrentState.IsElevated)
                    {
                        elevate = false;
                    }
                }
                else
                {
                    elevate = false;
                }

                var packageManager = elevate
                    ? this.appxPackageManagerFactory.GetRemote()
                    : this.appxPackageManagerFactory.GetLocal();

                var result = await lazyReducerOutput.GetReduced(this.interactionService, packageManager, cancellationToken).ConfigureAwait(false);

                if (elevate && !this.writableApplicationStateManager.CurrentState.IsSelfElevated)
                {
                    this.writableApplicationStateManager.CurrentState.IsSelfElevated = true;
                }

                return result;
            }
            catch (OperationCanceledException)
            {
                Logger.Warn("Operation cancelled by the user.");
                throw;
            }
            catch (Win32Exception e)
            {
                Logger.Error(e, "Win32 error during command execution.");
                // If error code is 1223 it means that the user did not press YES in UAC.
                var message = e.NativeErrorCode == 1223 ? "This operation requires administrative rights." : e.Message;
                var result = this.interactionService.ShowError(message, extendedInfo: e.ToString());

                if (result == InteractionResult.Retry)
                {
                    Logger.Info("Retrying..");
                    return await this.GetExecuteAsync(action, cancellationToken).ConfigureAwait(false);
                }

                return default;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Generic error during command execution.");
                var result = this.interactionService.ShowError(e.Message, extendedInfo: e.ToString());
                if (result == InteractionResult.Retry)
                {
                    Logger.Info("Retrying..");
                    return await this.GetExecuteAsync(action, cancellationToken).ConfigureAwait(false);
                }

                return default;
            }
        }

        private void ConfigureReducers()
        {
            this.reducerFactories[typeof(SetPackageFilter)] = action => new SetPackageFilterReducer((SetPackageFilter)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(SetPackageContext)] = action => new SetPackageContextReducer((SetPackageContext)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(GetPackages)] = action => new GetPackagesReducer((GetPackages)action, this.writableApplicationStateManager, this.busyManager);
            this.reducerFactories[typeof(SelectPackages)] = action => new SelectPackagesReducer((SelectPackages)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(GetRegistryMountState)] = action => new GetRegistryMountStateReducer((GetRegistryMountState)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(RunPackage)] = action => new RunPackageReducer((RunPackage)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(RunToolInPackage)] = action => new RunToolInPackageReducer((RunToolInPackage)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(RemovePackages)] = action => new RemovePackageReducer((RemovePackages)action, this.writableApplicationStateManager, this.busyManager);
            this.reducerFactories[typeof(FindUsers)] = action => new FindUsersReducer((FindUsers)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(GetUsersOfPackage)] = action => new GetUsersOfPackageReducer((GetUsersOfPackage)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(SetPackageSidebarVisibility)] = action => new SetPackageSidebarVisibilityReducer((SetPackageSidebarVisibility)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(MountRegistry)] = action => new MountRegistryReducer((MountRegistry)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(UnmountRegistry)] = action => new UnmountRegistryReducer((UnmountRegistry)action, this.writableApplicationStateManager, this.busyManager);
            this.reducerFactories[typeof(SetPackageSorting)] = action => new SetPackageSortingReducer((SetPackageSorting)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(SetPackageGrouping)] = action => new SetPackageGroupingReducer((SetPackageGrouping)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(GetLogs)] = action => new GetLogsReducer((GetLogs)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(AddPackage)] = action => new AddPackageReducer((AddPackage)action, this.writableApplicationStateManager, this.busyManager);
            this.reducerFactories[typeof(GetPackageDetails)] = action => new GetPackageDetailsReducer((GetPackageDetails)action, this.writableApplicationStateManager);
            this.reducerFactories[typeof(InstallCertificate)] = action => new InstallCertificateReducer((InstallCertificate)action, this.writableApplicationStateManager, this.busyManager);
        }
    }
}
