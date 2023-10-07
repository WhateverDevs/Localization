using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Localization.Runtime.TextPostProcessors;

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
        [Tooltip("Lower URLs will replace keys of the higher ones if they have the same key.")]
        public List<string> GoogleSheetsDownloadUrls;

        /// <summary>
        /// List of text post processors to be used by the localizer.
        /// </summary>
        public List<LocalizedTextPostProcessor> TextPostProcessors;
    }
}