using System;
using System.IO;
using System.Net;

namespace arcdps_updater
{
    public class HttpConnector
    {
        private readonly string _address;

        public HttpConnector(string address)
        {
            if (!address.EndsWith("/"))
                address += "/";

            _address = address;
        }

        public string DownloadFile(string targetDirectory, string fileName, out DateTime fileDateTime)
        {
            var targetFileName = Path.Combine(targetDirectory, fileName);
            
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(_address + fileName);
            httpWebRequest.Method = WebRequestMethods.File.DownloadFile;

            using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (var responseStream = httpWebResponse.GetResponseStream())
            using (var fileStream = File.Create(targetFileName))
            {
                fileDateTime = httpWebResponse.LastModified;
                responseStream.CopyTo(fileStream);
            }

            return targetFileName;
        }

        public string DownloadFile(string targetDirectory, string fileName)
        {
            return DownloadFile(targetDirectory, fileName, out _);
        }
    }
}