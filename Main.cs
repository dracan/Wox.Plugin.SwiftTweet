using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Wox.Plugin.SwiftTweet
{
    public class Main : IPlugin, ISettingProvider
    {
        private Twitter twitter;
        public const string twitterIconPath = "Resources\\TwitterLogo_#55acee.png";

        #region "Prerequisites"
        public void Init(PluginInitContext context)
        {
            try
            {
                getTwitterAccess();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Twitter getTwitterAccess()
        {
            string accessToken;
            string accessTokenSecret;
            try
            {
                if (twitter == null)
                {
                    // Get saved token to perform Twitter access
                    accessToken = Properties.Settings.Default.accessToken;
                    accessTokenSecret = Properties.Settings.Default.accessTokenSecret;
                    if (string.IsNullOrEmpty(accessToken) == false && string.IsNullOrEmpty(accessTokenSecret) == false)
                    {
                        twitter = new Twitter(accessToken, accessTokenSecret);
                        // Check access
                        if (twitter.checkAuthorization(false) == false)
                        {
                            twitter = null;
                        }
                        return twitter;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return twitter;
                }
            }
            catch (Exception)
            {;
                throw;
            }
        }
        #endregion

        #region "Build result list"
        /// <summary>
        /// Get result list
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();
            Result result;
            try
            {
                if (twitter == null)
                {
                    getTwitterAccess();
                }

                // Start building results
                if (twitter != null)
                {
                    if (query.Search.Length <= 140)
                    {
                        results.Add(buildTweetResult(query.Search)); // Tweet
                    }
                }
                else
                {
                    // No access
                    result = new Result("No Twitter access granted", twitterIconPath, "Please grant access to Twitter using the settings panel of wox");
                    results.Add(result);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return results;
        }

        /// <summary>
        /// Return result entry for sending a tweet
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private Result buildTweetResult(string query)
        {
            Result result;
            try
            {
                result = new Result
                {
                    IcoPath = twitterIconPath,
                    Title = "Publish tweet",
                    SubTitle = "Tweet \"" + query + "\"",
                    Action = (c) =>
                    {
                        return twitter.tweet(query);
                    }
                };

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        /// <summary>
        /// Initialize settings panel
        /// </summary>
        /// <returns></returns>
        public Control CreateSettingPanel()
        {
            return new frmSettings();
        }
    }
}
