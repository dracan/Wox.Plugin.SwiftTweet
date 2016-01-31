using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Wox.Plugin.SwiftTweet
{
    class TwitterImage
    {
        /// <summary>
        /// Downloads the image and returns the local filepath
        /// </summary>
        /// <param name="imageUrl">Twitter image url</param>
        /// <returns>The local path to the downloaded image</returns>
        public static string cacheUserImage(string imageUrl)
        {
            WebClient webclient;
            string path;
            try
            {
                path = "";
                if (string.IsNullOrEmpty(imageUrl) == false)
                {
                    path = TwitterImage.getCompleteCachePath(imageUrl);
                    if (File.Exists(path) == false)
                    {
                        if (string.IsNullOrEmpty(path) == false)
                        {
                            webclient = new WebClient();
                            try
                            {
                                webclient.DownloadFile(imageUrl, path);
                            }
                            catch (Exception) { }
                        }
                    }
                }

                return path;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves the complete cache path including the filename of the image
        /// </summary>
        /// <param name="imageUrl">Twitter image url</param>
        /// <returns>Complete cache path including the filename of the image</returns>
        public static string getCompleteCachePath(string imageUrl)
        {
            string cacheFolder;
            Uri uri;
            string filename;
            string path;
            try
            {
                filename = "";
                path = "";
                cacheFolder = getCacheFolder(true);
                if (string.IsNullOrEmpty(cacheFolder) == false)
                {
                    uri = new Uri(imageUrl);
                    if (uri != null)
                    {
                        filename = System.IO.Path.GetFileName(uri.LocalPath);
                    }
                }

                if (string.IsNullOrEmpty(filename) == false)
                {
                    path = cacheFolder + filename;
                }

                return path;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns the cache image folder
        /// </summary>
        /// <returns></returns>
        protected static string getCacheFolder(bool create)
        {
            string execPath;
            string cacheFolder;
            System.Security.AccessControl.DirectorySecurity access;
            try
            {
                execPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                access = Directory.GetAccessControl(execPath); // check access to folder --> exception if no permission
                cacheFolder = execPath + "\\ImageCache\\";

                if (create == true)
                {
                    if (Directory.Exists(cacheFolder) == false)
                    {
                        Directory.CreateDirectory(cacheFolder);
                    }
                }

                return cacheFolder;
            }
            catch (UnauthorizedAccessException)
            {
                return "";
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes all files and directories from the cache folder
        /// </summary>
        /// <returns></returns>
        public static void initCacheFolder()
        {
            string cacheFolder;
            DirectoryInfo directory;
            try
            {
                cacheFolder = getCacheFolder(false);
                if (string.IsNullOrEmpty(cacheFolder) == false)
                {
                    if (Directory.Exists(cacheFolder) == true)
                    {
                        directory = new DirectoryInfo(cacheFolder);
                        if (directory != null)
                        {
                            foreach (FileInfo file in directory.GetFiles())
                            {
                                try
                                {
                                    file.Delete();
                                }
                                catch (Exception) { }                               
                            }
                            foreach (DirectoryInfo dir in directory.GetDirectories())
                            {
                                try
                                {
                                    dir.Delete(true);
                                }
                                catch (Exception) { }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}