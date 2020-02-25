﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using otor.msixhero.lib.Domain.Appx.Packages;

namespace otor.msixhero.lib.Domain.Commands.Packages.Grid
{

    [Serializable]
    public class GetPackages : SelfElevatedCommand<List<InstalledPackage>>
    {
        public GetPackages()
        {
            this.Context = PackageContext.CurrentUser;
        }

        public GetPackages(PackageContext context)
        {
            this.Context = context;
        }

        [XmlElement]
        public PackageContext Context { get; set; }

        public override SelfElevationType RequiresElevation => this.Context == PackageContext.AllUsers ? SelfElevationType.RequireAdministrator : SelfElevationType.HighestAvailable;
    }
}
