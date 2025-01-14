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

using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
using Otor.MsixHero.AppInstaller.Entities;

namespace Otor.MsixHero.Tests
{
    [TestFixture]
    public class AppInstallerTests
    {
        [Test]
        public void TestSerialization()
        {
            var stringXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<AppInstaller
    xmlns=""http://schemas.microsoft.com/appx/appinstaller/2017""
    Version=""1.0.0.0""
    Uri=""http://mywebservice.azurewebsites.net/appset.appinstaller"" >

    <MainBundle
        Name=""Contoso.MainApp""
        Publisher=""CN=Contoso""
        Version=""2.23.12.43""
        Uri=""http://mywebservice.azurewebsites.net/mainapp.msixbundle"" />

    <OptionalPackages>
        <Bundle
            Name=""Contoso.OptionalApp1""
            Publisher=""CN=Contoso""
            Version=""2.23.12.43""
            Uri=""http://mywebservice.azurewebsites.net/OptionalApp1.msixbundle"" />

        <Bundle
            Name=""Contoso.OptionalApp2""
            Publisher=""CN=Contoso""
            Version=""2.23.12.43""
            Uri=""http://mywebservice.azurewebsites.net/OptionalApp2.msixbundle"" />

        <Package
            Name=""Fabrikam.OptionalApp3""
            Publisher=""CN=Fabrikam""
            Version=""10.34.54.23""
            ProcessorArchitecture=""x86""
            Uri=""http://mywebservice.azurewebsites.net/OptionalApp3.msix"" />

    </OptionalPackages>

    <Dependencies>
        <Package Name=""Microsoft.VCLibs.140.00"" Publisher=""CN=Microsoft Corporation, O=Microsoft Corporation, L=Redmond, S=Washington, C=US"" Version=""14.0.24605.0"" ProcessorArchitecture=""x86"" Uri=""http://foobarbaz.com/fwkx86.appx"" />
        <Package Name=""Microsoft.VCLibs.140.00"" Publisher=""CN=Microsoft Corporation, O=Microsoft Corporation, L=Redmond, S=Washington, C=US"" Version=""14.0.24605.0"" ProcessorArchitecture=""x64"" Uri=""http://foobarbaz.com/fwkx64.appx"" />
    </Dependencies>

    <UpdateSettings>
        <OnLaunch HoursBetweenUpdateChecks=""12"" />
    </UpdateSettings>

</AppInstaller>
";

            var xmlSerializer = new XmlSerializer(typeof(AppInstallerConfig2017));

            using (var stream = new StringReader(stringXml))
            {
                var result = (AppInstallerConfig2017) xmlSerializer.Deserialize(stream);

                Assert.AreEqual("http://mywebservice.azurewebsites.net/appset.appinstaller", result.Uri);
                Assert.NotNull(result.Dependencies);
                Assert.AreEqual(2, result.Dependencies.Count);
                Assert.AreEqual("Microsoft.VCLibs.140.00", result.Dependencies[0].Name);
                Assert.AreEqual("14.0.24605.0", result.Dependencies[0].Version);
                Assert.AreEqual("http://foobarbaz.com/fwkx86.appx", result.Dependencies[0].Uri);
                Assert.AreEqual(AppInstallerPackageArchitecture.x86, ((AppInstallerPackageEntry)result.Dependencies[0]).Architecture);
                Assert.AreEqual("CN=Microsoft Corporation, O=Microsoft Corporation, L=Redmond, S=Washington, C=US", result.Dependencies[0].Publisher);

                Assert.NotNull(result.UpdateSettings);
                Assert.NotNull(result.UpdateSettings.OnLaunch);
                Assert.AreEqual(12, result.UpdateSettings.OnLaunch.HoursBetweenUpdateChecks);
                Assert.IsFalse(result.UpdateSettings.OnLaunch.ShowPrompt);
                Assert.IsFalse(result.UpdateSettings.OnLaunch.UpdateBlocksActivation);


            }
        }
    }
}
