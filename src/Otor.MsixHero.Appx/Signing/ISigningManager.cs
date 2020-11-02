﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Otor.MsixHero.Appx.Signing.DeviceGuard;
using Otor.MsixHero.Appx.Signing.Entities;
using Otor.MsixHero.Infrastructure.Processes.SelfElevation;
using Otor.MsixHero.Infrastructure.Progress;

namespace Otor.MsixHero.Appx.Signing
{
    public interface ISigningManager : ISelfElevationAware
    {
        Task InstallCertificate(string certificateFilePath, CancellationToken cancellationToken = default, IProgress<ProgressData> progress = default);
        
        Task ImportCertificateFromMsix(string msixFile, CancellationToken cancellationToken = default, IProgress<ProgressData> progress = null);
        
        Task<string> CreateSelfSignedCertificate(
            DirectoryInfo outputDirectory, 
            string publisherName, 
            string publisherDisplayName, 
            string password, 
            DateTime? expirationDate,
            CancellationToken cancellationToken = default, 
            IProgress<ProgressData> progress = null);

        Task<List<PersonalCertificate>> GetCertificatesFromStore(CertificateStoreType certificateStoreType, bool onlyValid = true, CancellationToken cancellationToken = default, IProgress<ProgressData> progress = null);

        Task ExtractCertificateFromMsix(string msixFile, string outputFile, CancellationToken cancellationToken = default, IProgress<ProgressData> progress = null);

        Task ExtractCertificateFromMsix(string msixFile, CancellationToken cancellationToken = default, IProgress<ProgressData> progress = null);

        Task<PersonalCertificate> GetCertificateFromMsix(string msixFile, CancellationToken cancellationToken = default, IProgress<ProgressData> progress = null);

        Task SignPackageWithPfx(string package, bool updatePublisher, string pfxPath, SecureString password, string timestampUrl = null, IncreaseVersionMethod increaseVersion = IncreaseVersionMethod.None, CancellationToken cancellationToken = default, IProgress<ProgressData> progress = null);

        Task SignPackageWithInstalled(string package, bool updatePublisher, PersonalCertificate certificate, string timestampUrl = null, IncreaseVersionMethod increaseVersion = IncreaseVersionMethod.None, CancellationToken cancellationToken = default, IProgress<ProgressData> progress = null);

        Task SignPackageWithDeviceGuard(string package, bool updatePublisher, DeviceGuardConfig config, bool useDgssV1 = false, string timestampUrl = null, IncreaseVersionMethod increaseVersion = IncreaseVersionMethod.None, CancellationToken cancellationToken = default, IProgress<ProgressData> progress = null);

        Task<TrustStatus> IsTrusted(string certificateFileOrSignedFile, CancellationToken cancellationToken = default);
        
        Task Trust(string certificateFileOrSignedFile, CancellationToken cancellationToken = default);
    }
}