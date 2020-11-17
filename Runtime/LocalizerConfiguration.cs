using System;
using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Runtime.Configuration;

namespace WhateverDevs.Localization
{
    [CreateAssetMenu(menuName = "WhateverDevs/Localization/Configuration",
                     fileName = "LocalizerConfiguration")]
    public class
        LocalizerConfiguration : ConfigurationScriptableHolderUsingFirstValidPersister<LocalizerConfigurationData>
    {
    }

    /// <summary>
    ///     LocalizerConfigurationData configuration data.
    /// </summary>
    [Serializable]
    public class LocalizerConfigurationData : ConfigurationData
    {
        public List<SystemLanguage> Languages;

        public string LanguagePackDirectory = "Languages/";
    }
}