using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TweetSharp;
using TWHelper;

namespace Wox.Plugin.SwiftTweet
{
    class Twitter
    {
        public TwitterService service;
        public Authentication auth;

        /// <summary>
        /// Used to authorize with already existing accessToken (Twitter access already granted)
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="accessTokenSecret"></param>
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
        /// Retrieves the Twitter authorization URL
        /// </summary>
        /// <returns></returns>
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
        /// Retrieves the access token using the entered Twitter PIN code
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
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
        /// Send tweet
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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
    }
}
