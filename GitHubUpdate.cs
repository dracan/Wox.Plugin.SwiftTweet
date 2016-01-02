using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wox.Plugin.SwiftTweet
{
    class GitHubUpdate
    {
        private static string releaseUrl = "https://api.github.com/repos/NCiher/Wox.Plugin.SwiftTweet/releases";
        
        /// <summary>
        /// Checks if an update is available
        /// </summary>
        /// <returns>True if an update is available otherwise false</returns>
        public static bool updateAvailable()
        {
            string actualRelease;
            WebClient client;
            Stream webStream;
            StreamReader reader;
            string json;
            JArray releases;
            Version latestVersion;
            Version currentVersion;
            int comparisonResult;
            bool newVersionAvailable;
            try
            {
                // get json stream from GitHub API
                newVersionAvailable = false;
                actualRelease = "";
                client = new WebClient();
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; rv:15.0) Gecko/20120716 Firefox/15.0a2");
                webStream = client.OpenRead(releaseUrl);
                if (webStream != null)
                {
                    reader = new StreamReader(webStream);
                    json = reader.ReadToEnd();
                    if (string.IsNullOrEmpty(json) == false)
                    {
                        releases = JArray.Parse(json);
                        if (releases != null)
                        {
                            if (releases.Count > 0)
                            {
                                // get latest release
                                actualRelease = releases[0]["tag_name"].ToString();
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(actualRelease) == false)
                {
                    latestVersion = new Version(actualRelease);
                    // don't compare revision
                    currentVersion = new Version(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major, 
                        System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor, 
                        System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build);
                    comparisonResult = currentVersion.CompareTo(latestVersion);
                    if (comparisonResult < 0) // latest version is higher
                    {
                        newVersionAvailable = true;
                    }
                }

                return newVersionAvailable;
            }
            catch (Exception)
            {
                // Don't raise any exceptions
                return false;
            }
        }
    }
}
