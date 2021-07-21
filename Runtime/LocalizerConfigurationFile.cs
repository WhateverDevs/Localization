using System;
using UnityEngine;
using WhateverDevs.Core.Runtime.Configuration;

namespace WhateverDevs.Localization.Runtime
{
    /// <summary>
    /// Default holder implementation of the localizer configuration.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/Localization/Configuration",
                     fileName = "LocalizerConfiguration")]
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
    ///     LocalizerConfigurationData configuration data.
    /// </summary>
    [Serializable]
    public class LocalizerConfiguration : ConfigurationData
    {
        public string SelectedLanguage;

        public string LanguagePackDirectory = "Languages/";

        public string Separator = "\t";
    }
}