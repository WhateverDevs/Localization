using System;
using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Runtime.Configuration;

namespace WhateverDevs.Localizer.Runtime
{
    [CreateAssetMenu(menuName = "WhateverDevs/Localizer/Configuration",
        fileName = "LocalizerConfiguration")]
    public class LocalizerConfiguration : ConfigurationScriptableHolderUsingFirstValidPersister<LocalizerConfigurationData>
    {
    }

    /// <summary>
    ///     LocalizerConfigurationData configuration data.
    /// </summary>
    [Serializable]
    public class LocalizerConfigurationData : ConfigurationData
    {
        public List<SystemLanguage> languages;
        
        public string languagePackDirectory = "Languages/";
    }
}