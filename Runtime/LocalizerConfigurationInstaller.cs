using UnityEngine;
using Zenject;

namespace WhateverDevs.Localizer.Runtime
{
    /// <summary>
    ///     Extenject installer for the Exercise System Manager.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/Localizer/ConfigurationInstaller",
        fileName = "LocalizerConfigurationInstaller")]
    public class LocalizerConfigurationInstaller : ScriptableObjectInstaller
    {
        /// <summary>
        ///     Configurations that need a persister injection.
        /// </summary>
        public LocalizerConfiguration ConfigurationsToInstall;

        /// <summary>
        ///     Define injections.
        /// </summary>
        public override void InstallBindings()
        {
            // Inject the configuration manager to all classes that need that interface.
            Container.Bind<LocalizerConfiguration>().FromInstance(ConfigurationsToInstall);
        }
    }
}