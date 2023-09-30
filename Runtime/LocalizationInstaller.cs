using UnityEngine;
using WhateverDevs.Localization.Runtime.TextPostProcessors;
using Zenject;

namespace WhateverDevs.Localization.Runtime
{
    /// <summary>
    /// Default installer for the localization package.
    /// </summary>
    [CreateAssetMenu(fileName = "LocalizationInstaller", menuName = "WhateverDevs/DI/Localization")]
    public class LocalizationInstaller : ScriptableObjectInstaller
    {
        /// <summary>
        /// Settings to use in the project.
        /// </summary>
        [SerializeField]
        private LocalizerSettings LocalizerProjectSettings;

        /// <summary>
        /// Inject the settings into the localizer and the localizer into any class that needs it.
        /// </summary>
        public override void InstallBindings()
        {
            foreach (LocalizedTextPostProcessor postProcessor in LocalizerProjectSettings.TextPostProcessors)
                Container.QueueForInject(postProcessor);

            Container.Bind<LocalizerSettings>()
                     .FromInstance(LocalizerProjectSettings)
                     .AsSingle()
                     .WhenInjectedInto<Localizer>()
                     .Lazy();

            Container.Bind<ILocalizer>().To<Localizer>().AsSingle().Lazy();
        }
    }
}