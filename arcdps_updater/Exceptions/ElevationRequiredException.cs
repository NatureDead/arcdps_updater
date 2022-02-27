using System;

namespace arcdps_updater.Exceptions
{
    [Serializable]
    internal class ElevationRequiredException : Exception
    {
        public ElevationRequiredException() : base("The program needs to be executed as admin.")
        {
        }
    }
}