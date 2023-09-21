using UnityEngine;
using WhateverDevs.Core.Runtime.Build;
using WhateverDevs.Localization.Runtime;

namespace WhateverDevs.Localization.Editor.BuildHooks
{
    /// <summary>
    /// Build hook that refreshes the languages just before building.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/Localization/BuildHook",
                     fileName = "DownloadLocalizationFromGoogleSheetBeforeBuild")]
    public class DownloadLocalizationFromGoogleSheetBeforeBuild : BuildProcessorHook
    {
        /// <summary>
        /// Url to refresh from.
        /// </summary>
        public string Url;

        /// <summary>
        /// Reference to the configuration file.
        /// </summary>
        public LocalizerConfigurationFile LocalizationConfigurationFile;

        /// <summary>
        /// Load the languages.
        /// </summary>
        public override bool RunHook(string buildPath)
        {
            GoogleSheetLoader.LoadLanguages(Url, LocalizationConfigurationFile.ConfigurationData.LanguagePackDirectory);
            return true;
        }
    }
}