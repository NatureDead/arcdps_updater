using arcdps_updater.Exceptions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace arcdps_updater
{
    public class ArcdpsUpdater
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly EnvironmentProvider _environmentProvider;
        private const string ArcdpsFileName = "d3d9.dll";
        private const string ArcdpsChecksumFileName = "d3d9.dll.md5sum";

        public ArcdpsUpdater(SettingsProvider settingsProvider, EnvironmentProvider environmentProvider)
        {
            _settingsProvider = settingsProvider;
            _environmentProvider = environmentProvider;
        }

        public void Execute()
        {
            var localDirectory = _settingsProvider.InstallationPath;
            
            if (_environmentProvider.IsElevationRequired(localDirectory) && !_environmentProvider.IsElevated())
                throw new ElevationRequiredException();

            var httpConnector = GetHttpConnector();
            var localFileName = GetLocalFileName();

            if (IsNewestVersion(httpConnector, localFileName, out var version)) return;

            File.Delete(localFileName);
            var tempFileName = string.Empty;

            try
            {
                Console.WriteLine($"Downloading 'arcdps' files");
                tempFileName = httpConnector.DownloadFile(_settingsProvider.TempPath, ArcdpsFileName);
                File.Move(tempFileName, localFileName);
                
                Console.WriteLine($"Updated 'arcdps' to {version}");
            }
            finally
            {
                DeleteIfExists(tempFileName);
            }
        }

        public void Clear()
        {
            var localFileName = GetLocalFileName();            
            File.Delete(localFileName);
            Console.WriteLine($"Removed 'arcdps' files");
        }

        public void CheckVersion()
        {
            IsNewestVersion(GetHttpConnector(), GetLocalFileName(), out _);
        }

        private bool IsNewestVersion(HttpConnector httpConnector, string fileName, out string version)
        {
            var checksumFileName = string.Empty;

            try
            {
                checksumFileName = httpConnector.DownloadFile(_settingsProvider.TempPath, ArcdpsChecksumFileName, out var fileDateTime);
                version = GetVersion(fileDateTime);

                var text = File.ReadAllText(checksumFileName);
                var checksum = text.Split(' ')[0];

                if (File.Exists(fileName))
                {
                    var localFileChecksum = CreateChecksum(fileName);
                    if (string.Compare(checksum, localFileChecksum, true) == 0)
                    {
                        Console.WriteLine($"No newer version for 'arcdps' available ({version})");
                        return true;
                    }
                }

                Console.WriteLine($"A newer version of 'arcdps' available ({version})");
                return false;
            }
            finally
            {
                DeleteIfExists(checksumFileName);
            }
        }

        private HttpConnector GetHttpConnector()
        {
            return new HttpConnector(_settingsProvider.ResourcesAddress);
        }

        private string GetLocalFileName()
        {
            return Path.Combine(_settingsProvider.InstallationPath, ArcdpsFileName);
        }
        
        private string GetVersion(DateTime dateTime)
        {
            return $"version {dateTime.ToString("dd-MM-yyyy")}";
        }

        private void DeleteIfExists(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            File.Delete(fileName);
        }

        private static string CreateChecksum(string fileName)
        {
            using (MD5 md5 = MD5.Create())
            {
                var inputBytes = File.ReadAllBytes(fileName);
                var hashBytes = md5.ComputeHash(inputBytes);

                var stringBuilder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                    stringBuilder.Append(hashBytes[i].ToString("X2"));

                return stringBuilder.ToString();
            }
        }
    }
}
