﻿using System;
using System.Linq;
using NUnit.Framework;
using otor.msixhero.lib.BusinessLayer.Winget;
using otor.msixhero.lib.Domain.Winget;

namespace otor.msixhero.lib.tests.Winget
{
    [TestFixture]
    public class InstallerTypeDetectorTests
    {
        [Test]
        public void TestNsis()
        {
            var sd = new InstallerTypeDetector();

            var resources = typeof(InstallerTypeDetectorTests).Assembly.GetManifestResourceNames();

            var nsis = resources.First(r => r.IndexOf("nsis", StringComparison.OrdinalIgnoreCase) != -1);

            using (var s = typeof(InstallerTypeDetectorTests).Assembly.GetManifestResourceStream(nsis))
            {
                var detect = sd.DetectSetupType(s).Result;
                Assert.AreEqual(YamlInstallerType.nullsoft, detect);
            }
        }

        [Test]
        public void TestInno()
        {
            var sd = new InstallerTypeDetector();

            var resources = typeof(InstallerTypeDetectorTests).Assembly.GetManifestResourceNames();
            var inno = resources.First(r => r.IndexOf("inno", StringComparison.OrdinalIgnoreCase) != -1);
            using (var s = typeof(InstallerTypeDetectorTests).Assembly.GetManifestResourceStream(inno))
            {
                var detect = sd.DetectSetupType(s).Result;
                Assert.AreEqual(YamlInstallerType.inno, detect);
            }
        }
    }
}