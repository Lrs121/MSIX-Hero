﻿// MSIX Hero
// Copyright (C) 2021 Marcin Otorowski
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

using System;
using System.Xml.Serialization;
using Otor.MsixHero.Infrastructure.Processes.Ipc;
using Otor.MsixHero.Lib.Proxy.Diagnostic.Dto;
using Otor.MsixHero.Lib.Proxy.Packaging.Dto;
using Otor.MsixHero.Lib.Proxy.Signing.Dto;

namespace Otor.MsixHero.Lib.Proxy
{
    [Serializable]
    [XmlInclude(typeof(GetInstalledPackagesDto))]
    [XmlInclude(typeof(CheckIfInstalledDto))]
    [XmlInclude(typeof(RunDto))]
    [XmlInclude(typeof(MountRegistryDto))]
    [XmlInclude(typeof(DismountRegistryDto))]
    [XmlInclude(typeof(GetUsersForPackageDto))]
    [XmlInclude(typeof(RemoveDto))]
    [XmlInclude(typeof(RemoveCurrentUserDto))]
    [XmlInclude(typeof(GetLogsDto))]
    [XmlInclude(typeof(GetByIdentityDto))]
    [XmlInclude(typeof(GetByManifestPathDto))]
    [XmlInclude(typeof(InstallCertificateDto))]
    [XmlInclude(typeof(TrustDto))]
    public abstract class ProxyObject : IProxyObject
    {
    }

    // ReSharper disable once UnusedTypeParameter
    public abstract class ProxyObject<T> : ProxyObject, IProxyObjectWithOutput<T>
    {
    }
}