using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace arcdps_updater
{
    public class EnvironmentProvider
    {
        public EnvironmentProvider()
        {
        }

        public bool IsElevated()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public bool IsElevationRequired(string directory)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder. 
                // This will raise an exception if the path is read only or do not have access to view the permissions. 
                DirectorySecurity ds = Directory.GetAccessControl(directory);
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
        }
    }
}
