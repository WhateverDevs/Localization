using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace WhateverDevs.Localization.Runtime
{
    /// <summary>
    /// Settings to be used by the localizer in this project.
    /// This is different from the localizer configuration because the player shouldn't change this.
    /// This is for the developer to set up the localizer.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/Localization/InternalSettings", fileName = "LocalizerProjectSettings")]
    public class LocalizerSettings : WhateverScriptable<LocalizerSettings>
    {
        /// <summary>
        /// Folder in which the language packs are stored.
        /// This folder is inside the Resources folder.
        /// </summary>
        public string LanguagePackDirectory = "Languages/";

        /// <summary>
        /// Google Sheets Url to download the languages from.
        /// </summary>
        public string GoogleSheetsDownloadUrl = "";
    }
}