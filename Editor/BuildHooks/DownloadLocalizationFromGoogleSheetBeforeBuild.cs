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
        /// Reference to the localization project settings.
        /// </summary>
        [SerializeField]
        private LocalizerSettings LocalizationProjectSettings;

        /// <summary>
        /// Load the languages.
        /// </summary>
        public override bool RunHook(string buildPath)
        {
            GoogleSheetLoader.LoadLanguages(LocalizationProjectSettings.GoogleSheetsDownloadUrl,
                                            LocalizationProjectSettings.LanguagePackDirectory);

            return true;
        }
    }
}