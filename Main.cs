using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using TweetSharp;

namespace Wox.Plugin.SwiftTweet
{
    public class Main : IPlugin, ISettingProvider
    {
        private Twitter twitter;
        private const string twitterIconPath = "Resources\\TwitterLogo_#55acee.png";
        private const int splitTweetPos = 80;

        private enum validCommands
        {
            tweet = 1,
            search = 2
        }

        #region "Prerequisites"
        public void Init(PluginInitContext context)
        {
            try
            {
                // init cache
                TwitterImage.initCacheFolder();
                // init Twitter access
                getTwitterAccess();
                // perform update check
                if (GitHubUpdate.updateAvailable() == true)
                {
                    context.API.ShowMsg("Update for SwiftTweet plugin available", "Use \"wpm uninstall SwiftTweet\" and \"wpm install SwiftTweet\"", twitterIconPath);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Initialize the Twitter helper class and get the Twitter access
        /// </summary>
        /// <returns>Twitter helper class</returns>
        private Twitter getTwitterAccess()
        {
            string accessToken;
            string accessTokenSecret;
            try
            {
                if (twitter == null)
                {
                    // Get saved token to perform Twitter access
                    accessToken = Settings.getAccessToken();
                    accessTokenSecret = Settings.getAccessTokenSecret();
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
            {
                throw;
            }
        }
        #endregion

        #region "Build result list"
        /// <summary>
        /// Get result list
        /// Sub routines will be called to build the specific results for each command (search, tweet and so on)
        /// </summary>
        /// <param name="query">Query entered by the user in the wox search panel</param>
        /// <returns>Wox results</returns>
        public List<Result> Query(Query query)
        {
            List<Result> results;
            Result result;
            validCommands command;
            bool isValidCommand;
            try
            {
                results = new List<Result>();
                if (twitter == null)
                {
                    getTwitterAccess();
                }

                // Start building results
                if (twitter != null)
                {
                    // Check for valid command --> see enum "validCommands"
                    isValidCommand = System.Enum.IsDefined(typeof(validCommands), query.FirstSearch);
                    if (isValidCommand == true)
                    {
                        command = (validCommands)System.Enum.Parse(typeof(validCommands), query.FirstSearch, true);
                        switch (command)
                        {
                            case validCommands.tweet:
                                if (query.SecondToEndSearch.Length <= 140)
                                {
                                    results.Add(buildTweetResult(query.SecondToEndSearch)); // Tweet
                                }
                                break;

                            case validCommands.search:
                                if (query.SecondToEndSearch.Length > 0)
                                {
                                    results = buildSearchResult(query.SecondToEndSearch); // Search for tweets
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        // No valid command
                        result = new Result("Twitter commands: " + getConcValidCommands(), twitterIconPath, "Use one of the following commands: " + getConcValidCommands());
                        results.Add(result);
                    }
                }
                else
                {
                    if (Twitter.checkForInternetConnection() == false)
                    {
                        // No internet connection
                        result = new Result("No internet connection available", twitterIconPath);
                        results.Add(result);
                    }
                    else
                    {
                        // No access
                        result = new Result("No Twitter access granted", twitterIconPath, "Please grant access to Twitter using the settings panel of wox");
                        results.Add(result);
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
        /// Get result entries for sending a tweet
        /// </summary>
        /// <param name="query">Query entered by the user in the wox search panel without the keywords</param>
        /// <returns>Results for sending a tweet</returns>
        protected Result buildTweetResult(string query)
        {
            Result result;
            try
            {
                result = new Result
                {
                    IcoPath = twitterIconPath,
                    Title = "Send tweet",
                    SubTitle = "Tweet \"" + query + "\"",
                    Action = (c) =>
                    {
                        return twitter.tweet(query); // call the helper method to send the tweet
                    }
                };

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get results entries for twitter search
        /// </summary>
        /// <param name="query">Query entered by the user in the wox search panel without the keywords</param>
        /// <returns>Results of the twitter query</returns>
        protected List<Result> buildSearchResult(string query)
        {
            Result result;
            List<Result> results;
            IEnumerable<TwitterStatus> searchResults;
            string text;
            string textEnd;
            try
            {
                results = new List<Result>();
                // execute search
                searchResults = twitter.search(query);
                if (searchResults != null)
                {
                    // handle image cache
                    foreach (TwitterStatus twitterStatus in searchResults)
                    {
                        TwitterImage.cacheUserImage(twitterStatus.Author.ProfileImageUrl);
                    }

                    // process results
                    foreach (TwitterStatus twitterStatus in searchResults)
                    {
                        text = twitterStatus.Text.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " "); // remove carriege returns and line feeds
                        textEnd = "";
                        // split the tweet if it's too long to display it completly within the first result line
                        // the rest of the tweet will be displayed in the subtitle of the wox result list
                        if (text.Length > splitTweetPos)
                        {
                            textEnd = text.Substring(splitTweetPos);
                            textEnd += "\t\t";
                            text = text.Substring(0, splitTweetPos);
                        }

                        // build up the result entry
                        result = new Result
                        {
                            IcoPath = TwitterImage.getCompleteCachePath(twitterStatus.Author.ProfileImageUrl),
                            Title = text,
                            SubTitle = textEnd + twitterStatus.User.Name + " | @" + twitterStatus.User.ScreenName + " | " + twitterStatus.CreatedDate.ToString(),
                            Action = (c) =>
                            {
                                return twitter.openTweet(twitterStatus.IdStr);
                            }
                        };

                        results.Add(result);
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
        /// Get one string with all valid twitter commands
        /// </summary>
        /// <returns>String with all valid twitter commands</returns>
        protected string getConcValidCommands()
        {
            Array enumValues;
            string concCommands;
            try
            {
                concCommands = "";
                enumValues = Enum.GetValues(typeof(validCommands));
                foreach (var value in enumValues)
                {
                    concCommands += value.ToString() + " | ";
                }

                if (concCommands.Length > 3)
                {
                    concCommands = concCommands.Remove(concCommands.Length - 3, 3);
                }

                return concCommands;
            }
            catch (Exception){throw;}
        }
        #endregion

        /// <summary>
        /// Initialize settings panel
        /// </summary>
        /// <returns>Settings form</returns>
        public Control CreateSettingPanel()
        {
            return new frmSettings();
        }
    }
}
