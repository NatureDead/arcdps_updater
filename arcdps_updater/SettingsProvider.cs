using System;
using System.Configuration;
using System.IO;

namespace arcdps_updater
{
    public class SettingsProvider
    {
        private const string InstalltionPathKey = "installationPath";
        private const string ResourcesAddressKey = "resourcesAddress";

        public string InstallationPath { get; }
        public string ResourcesAddress { get; }
        public string TempPath { get; } = Path.Combine(Path.GetTempPath(), "natured", "arcdps_updater");

        public SettingsProvider()
        {
            InstallationPath = ConfigurationManager.AppSettings[InstalltionPathKey];
            ResourcesAddress = ConfigurationManager.AppSettings[ResourcesAddressKey];

            Console.WriteLine("settings:");
            Console.WriteLine($"{InstalltionPathKey}: {InstallationPath}");
            Console.WriteLine($"{ResourcesAddressKey}: {ResourcesAddress}");
            Console.WriteLine();

            Initialize();
        }

        private void Initialize()
        {
            Directory.CreateDirectory(TempPath);
        }
    }
}