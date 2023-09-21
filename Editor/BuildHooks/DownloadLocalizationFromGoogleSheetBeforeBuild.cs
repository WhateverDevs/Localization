using UnityEngine;
using WhateverDevs.Core.Runtime.Build;

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
        /// Directory in which to output the localization (inside assets).
        /// </summary>
        public string OutputDirectory = "Languages/";

        /// <summary>
        /// Load the languages.
        /// </summary>
        public override bool RunHook(string buildPath)
        {
            GoogleSheetLoader.LoadLanguages(Url, OutputDirectory);
            return true;
        }
    }
}