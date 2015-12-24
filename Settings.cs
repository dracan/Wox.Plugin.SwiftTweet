using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBTek.Crypto;

namespace Wox.Plugin.SwiftTweet
{
    class Settings
    {
        /// <summary>
        /// Save the Twitter access token. Before saving the token will be encrypted.
        /// </summary>
        /// <param name="token">Twitter access token</param>
        /// <param name="tokenSecret">Twitter access token secret</param>
        public static void saveAccessToken(string token, string tokenSecret)
        {
            TripleDES encoder;
            try
            {
                encoder = new TripleDES();
                Properties.Settings.Default.accessToken = encoder.EncodeString(token);
                Properties.Settings.Default.accessTokenSecret = encoder.EncodeString(tokenSecret);
                Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the decrypted Twitter access token
        /// </summary>
        /// <returns>Decrypted twitter access token</returns>
        public static string getAccessToken()
        {
            TripleDES decoder;
            try
            {
                decoder = new TripleDES();
                return decoder.DecodeString(Properties.Settings.Default.accessToken);
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Get the decrypted Twitter access token secret
        /// </summary>
        /// <returns>Decrypted twitter access token secret</returns>
        public static string getAccessTokenSecret()
        {
            TripleDES decoder;
            try
            {
                decoder = new TripleDES();
                return decoder.DecodeString(Properties.Settings.Default.accessTokenSecret);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
