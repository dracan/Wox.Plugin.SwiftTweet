using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TweetSharp;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Wox.Infrastructure.Logger;

namespace Wox.Plugin.SwiftTweet
{
    public partial class frmSettings : UserControl
    {
        private Twitter twitter;

        /// <summary>
        /// Initilize settings window
        /// </summary>
        public frmSettings()
        {
            try
            {
                InitializeComponent();
                checkTwitterAuth();
            }
            catch (Exception e)
            {
                Log.Error(e);
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Performs the Twitter authorization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="re"></param>
        private void btnAuthorise_onClick(object sender, RoutedEventArgs re)
        {
            Uri authUrl;
            string pin;
            OAuthAccessToken accessToken;
            Twitter twitter;
            try
            {
                // Gernerating and opening the Twitter authorization URL
                twitter = new Twitter();
                if (Twitter.checkForInternetConnection() == true)
                {
                    authUrl = twitter.getAuthorizationUri();
                    if (authUrl != null)
                    {
                        if (string.IsNullOrEmpty(authUrl.ToString()) == false)
                        {
                            Process.Start(authUrl.ToString());
                        }
                        else
                        {
                            throw new Exception("Generating Twitter authorization URL failed.");
                        }
                    }
                    else
                    {
                        throw new Exception("Generating Twitter authorization URL failed.");
                    }

                    // Get the access token using the entered PIN
                    pin = Interaction.InputBox("Plese enter the twitter authorization PIN code provided by the twitter website.", "Enter Twitter PIN code");
                    if (string.IsNullOrEmpty(pin) == false)
                    {
                        accessToken = twitter.getAccessToken(pin);

                        if (accessToken != null)
                        {
                            // Save the access token for later usage
                            Settings.saveAccessToken(accessToken.Token, accessToken.TokenSecret);
                            checkTwitterAuth();
                        }
                        else
                        {
                            throw new Exception("Generating Twitter access token failed.");
                        }
                    }
                }
                else
                {
                    throw new Exception("No internet connection available.");
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Checks the twitter authorization and performs the related ui changes
        /// </summary>
        private void checkTwitterAuth()
        {
            string accessToken;
            string accessTokenSecret;
            bool authorized;
            try
            {
                // Set label for authorization status
                lblAuthStatusValue.Content = "Access not granted";
                accessToken = Settings.getAccessToken();
                accessTokenSecret = Settings.getAccessTokenSecret();
                twitter = new Twitter(accessToken, accessTokenSecret);
                if (twitter != null)
                {
                    authorized = twitter.checkAuthorization(false);
                    if (authorized == true)
                    {
                        lblAuthStatusValue.Content = "Access granted";
                        btnAuthorise.IsEnabled = false;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Opens the project website
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="re"></param>
        private void txbProjectURL_onRequestNavigate(object sender, RequestNavigateEventArgs re)
        {
            try
            {
                Process.Start(re.Uri.AbsoluteUri);
            }
            catch (Exception e)
            {
                Log.Error(e);
                MessageBox.Show(e.Message);
            }
        }
    }
}
