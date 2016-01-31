using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TweetSharp;
using TWHelper;
using System.Diagnostics;
using System.Net;

namespace Wox.Plugin.SwiftTweet
{
    class Twitter
    {
        private TwitterService service;
        private Authentication auth;
        private const string tweetUrl = "http://twitter.com/statuses/";

        /// <summary>
        /// Used to authorize with already existing accessToken (Twitter access already granted)
        /// </summary>
        /// <param name="accessToken">Twitter access token</param>
        /// <param name="accessTokenSecret">Twitter access token secret</param>
        public Twitter(string accessToken, string accessTokenSecret)
        {
            try
            {
                // Start Twitter authentication
                auth = new Authentication(Authentication.ApplicationName.SwiftTweet, accessToken, accessTokenSecret);
                service = auth.getTwitterService();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Used to grant initial access to Twitter
        /// </summary>
        public Twitter()
        {
            try
            {
                auth = new Authentication(TWHelper.Authentication.ApplicationName.SwiftTweet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Authentication
        /// <summary>
        /// Get the Twitter authorization URL
        /// </summary>
        /// <returns>Twitter authorization url</returns>
        public Uri getAuthorizationUri()
        {
            try
            {
                return auth.getAuthorizationUri();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the access token using the entered Twitter PIN code
        /// </summary>
        /// <param name="pin">Twitter PIN provides by the Twitter website using the authorization url</param>
        /// <returns>Twitter access token</returns>
        public OAuthAccessToken getAccessToken(string pin)
        {
            try
            {
                return auth.getAccessToken(pin);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Checks if the Twitter access is available
        /// </summary>
        /// <param name="showMessage">True to show a message if the access to Twitter is not available</param>
        /// <returns>True if the access to Twitter is available otherwise false</returns>
        public bool checkAuthorization(bool showMessage)
        {
            VerifyCredentialsOptions options;
            TwitterUser user;
            bool authSuccessful;
            try
            {
                authSuccessful = false;
                // Check service available
                if (service != null)
                {
                    // Check user
                    options = new VerifyCredentialsOptions();
                    user = service.VerifyCredentials(options);
                    if (user != null)
                    {
                        authSuccessful = true;
                    }
                }

                if (authSuccessful == false && showMessage == true)
                {
                    throw new Exception("Twitter authorization failed");
                }
                else
                {
                    return authSuccessful;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        /// <summary>
        /// Send a tweet
        /// </summary>
        /// <param name="text">The content of the tweet</param>
        /// <returns>True if the tweet was successfully send otherwise false</returns>
        public bool tweet(string text)
        {
            bool success;
            SendTweetOptions options;
            TwitterStatus status;
            try
            {
                success = false;
                if (service != null)
                {
                    options = new SendTweetOptions();
                    options.Status = text;
                    status = service.SendTweet(options);
                    if (status != null)
                    {
                        if (string.IsNullOrEmpty(status.IdStr) == false)
                        {
                            success = true;
                        }
                    }
                }

                return success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Search for tweets
        /// </summary>
        /// <param name="query">The query to search for</param>
        /// <returns>List of Twitter status (tweets)</returns>
        public IEnumerable<TwitterStatus> search(string query)
        {
            SearchOptions options;
            TwitterSearchResult searchResult;
            IEnumerable<TwitterStatus> results = null;
            try
            {
                if (service != null)
                {
                    options = new SearchOptions();
                    options.Q = query;
                    options.Count = 10;
                    searchResult = service.Search(options);
                    if (searchResult != null)
                    {
                        results = searchResult.Statuses;
                    }
                }

                return results;               
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Opens a tweet in the default browser using the status id
        /// </summary>
        /// <param name="tweetId">Twitter status (tweet) id</param>
        /// <returns>True if the tweet was successfully opened otherwise false</returns>
        public bool openTweet(string tweetId)
        {
            bool success;
            string url;
            try
            {
                success = false;
                if (string.IsNullOrEmpty(tweetId) == false)
                {
                    url = tweetUrl + tweetId;
                    Process.Start(url);
                    success = true;
                }
                                
                return success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Checks if an internet connection is available
        /// </summary>
        /// <returns>True if internet is available otherwise false</returns>
        public static bool checkForInternetConnection()
        {
            WebClient client;
            System.IO.Stream stream;
            bool connectionAvailable;
            try
            {
                connectionAvailable = false;
                client = new WebClient();
                if (client != null)
                {
                    stream = client.OpenRead("http://www.google.com");
                    if (stream != null)
                    {
                        connectionAvailable = true;
                    }
                }

                return connectionAvailable;
            }
            catch
            {
                return false;
            }
        }
    }
}
