using System;
using UnityEngine;
using WhateverDevs.Core.Runtime.Configuration;

namespace WhateverDevs.Localization.Runtime
{
    /// <summary>
    /// Default holder implementation of the localizer configuration.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/Localization/UserConfiguration", fileName = "LocalizerConfiguration")]
    public class
        LocalizerConfigurationFile : LocalizerConfigurationFile<LocalizerConfiguration>
    {
    }

    /// <summary>
    /// Abstract class that will allow for overrides without breaking the editor tools.
    /// </summary>
    /// <typeparam name="TLocalizerConfiguration">Localizer configuration implementation.</typeparam>
    public abstract class LocalizerConfigurationFile<TLocalizerConfiguration> :
        ConfigurationScriptableHolderUsingFirstValidPersister<TLocalizerConfiguration> where TLocalizerConfiguration :
        LocalizerConfiguration, new()
    {
    }

    /// <summary>
    /// Class that stores the localization configuration.
    /// </summary>
    [Serializable]
    public class LocalizerConfiguration : ConfigurationData
    {
        /// <summary>
        /// Currently selected language.
        /// </summary>
        public string SelectedLanguage = "English";
    }
}