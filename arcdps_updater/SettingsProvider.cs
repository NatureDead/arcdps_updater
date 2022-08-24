using System;
using System.Configuration;
using System.IO;

namespace arcdps_updater
{
    public class SettingsProvider
    {
        private const string FileNameKey = "fileName";
        private const string InstallationPathKey = "installationPath";
        private const string ResourcesAddressKey = "resourcesAddress";

        public string FileName { get; }
        public string InstallationPath { get; }
        public string ResourcesAddress { get; }

        public string TempPath { get; } = Path.Combine(Path.GetTempPath(), "natured", "arcdps_updater");

        public SettingsProvider()
        {
            FileName = ConfigurationManager.AppSettings[FileNameKey];
            InstallationPath = ConfigurationManager.AppSettings[InstallationPathKey];
            ResourcesAddress = ConfigurationManager.AppSettings[ResourcesAddressKey];

            Console.WriteLine("settings:");
            Console.WriteLine($"{FileNameKey}: {FileName}");
            Console.WriteLine($"{InstallationPath}: {InstallationPath}");
            Console.WriteLine($"{ResourcesAddressKey}: {ResourcesAddress}");
            Console.WriteLine();

            Initialize();
        }

        private void Initialize()
        {
            Directory.CreateDirectory(TempPath);
        }

        public string GetFqFileName()
        {
            return Path.Combine(InstallationPath, FileName);
        }
    }
}