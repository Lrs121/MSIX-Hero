﻿using Otor.MsixHero.App.Mvvm.Changeable;
using Otor.MsixHero.AppInstaller.Entities;
using Otor.MsixHero.Appx.Editor;

namespace Otor.MsixHero.App.Modules.Dialogs.AppInstaller.Editor.ViewModel.Tabs
{
    public enum AppInstallerBasePackageViewModelType
    {
        Package,
        Bundle
    }

    public abstract class AppInstallerBasePackageViewModel : ChangeableContainer
    {
        protected AppInstallerBasePackageViewModel()
        {
            this.Name = new ValidatedChangeableProperty<string>("Package name", AppxValidatorFactory.ValidatePackageName());
            this.Publisher = new ValidatedChangeableProperty<string>("Package publisher", AppxValidatorFactory.ValidateSubject());
            this.Version = new ValidatedChangeableProperty<string>("Package version", "1.0.0", AppxValidatorFactory.ValidateVersion());
            this.Uri = new ValidatedChangeableProperty<string>("Package URI", ValidatorFactory.ValidateUri(true));
            this.AddChildren(this.Name, this.Publisher, this.Version, this.Uri);
        }

        protected AppInstallerBasePackageViewModel(AppInstallerBaseEntry baseEntry)
        {
            this.Name = new ValidatedChangeableProperty<string>("Package name", baseEntry.Name, AppxValidatorFactory.ValidatePackageName());
            this.Publisher = new ValidatedChangeableProperty<string>("Package publisher", baseEntry.Publisher, AppxValidatorFactory.ValidateSubject());
            this.Version = new ValidatedChangeableProperty<string>("Package version", baseEntry.Version, AppxValidatorFactory.ValidateVersion());
            this.Uri = new ValidatedChangeableProperty<string>("Package URI", baseEntry.Uri, ValidatorFactory.ValidateUri(true));
            this.AddChildren(this.Name, this.Publisher, this.Version, this.Uri);
        }

        public abstract AppInstallerBasePackageViewModelType Type { get; }

        public ChangeableProperty<string> Header { get; } = new ChangeableProperty<string>();
        
        public ValidatedChangeableProperty<string> Name { get; }
        
        public ValidatedChangeableProperty<string> Publisher { get; }

        public ValidatedChangeableProperty<string> Version { get; }

        public ValidatedChangeableProperty<string> Uri { get; }
        
        public ChangeableProperty<AppInstallerPackageArchitecture> Architecture { get; protected set; }

        public bool HasArchitecture => this.Architecture != null;

        public abstract AppInstallerBaseEntry ToModel();
    }
}