using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBTek.Crypto;

namespace Wox.Plugin.SwiftTweet
{
    class Settings
    {
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
