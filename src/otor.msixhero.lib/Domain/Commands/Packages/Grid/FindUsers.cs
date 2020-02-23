﻿using System.Collections.Generic;
using System.Xml.Serialization;
using otor.msixhero.lib.Domain.Appx.Packages;
using otor.msixhero.lib.Domain.Appx.Users;

namespace otor.msixhero.lib.Domain.Commands.Packages.Grid
{
    public class FindUsers : SelfElevatedCommand<List<User>>
    {
        public FindUsers()
        {
        }

        public FindUsers(InstalledPackage package, bool forceElevation)
        {
            this.FullProductId = package.PackageId;
            this.ForceElevation = forceElevation;
        }

        public FindUsers(string fullProductId, bool forceElevation)
        {
            this.FullProductId = fullProductId;
            this.ForceElevation = forceElevation;
        }

        [XmlElement]
        public string FullProductId { get; set; }

        [XmlElement]
        public bool ForceElevation { get; set; }

        [XmlIgnore]
        public override bool RequiresElevation
        {
            get
            {
                return this.ForceElevation;
            }
        }
    }
}