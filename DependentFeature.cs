using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.SwiftTweet
{
    class DependentFeature
    {
        private List<dependentFeatures> cacheSupportedFeatures = new List<dependentFeatures>();
        private List<dependentFeatures> cacheUnsupportedFeatures = new List<dependentFeatures>();
        private dependentFeatures feature;

        internal dependentFeatures Feature
        {
            get
            {
                return feature;
            }

            set
            {
                feature = value;
            }
        }

        public enum dependentFeatures
        {
        }

        public DependentFeature(dependentFeatures feature)
        {
            Feature = feature;
        }

        /// <summary>
        /// Retrieves the version of Wox
        /// </summary>
        /// <returns>Version of Wox</returns>
        private Version getWoxVersion()
        {
            try
            {
                return System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check wether a feature is supported by Wox version running this plugin
        /// </summary>
        /// <param name="feature">Feature to check</param>
        /// <returns>True if the feature is supported by Wox otherwise false</returns>
        public bool isDependentFeatureSupported()
        {
            bool isSupported;
            Version minVersion;
            Version woxVersion;
            try
            {
                isSupported = false;

                if (cacheSupportedFeatures.Contains(Feature) == true)
                {
                    isSupported = true;
                }
                else if (cacheUnsupportedFeatures.Contains(Feature) == true)
                {
                    isSupported = false;
                }
                else
                {
                    woxVersion = getWoxVersion();
                    switch (Feature)
                    {
                        // Implement case for a dependent feature
                        default:
                            break;
                    }
                }

                return isSupported;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
