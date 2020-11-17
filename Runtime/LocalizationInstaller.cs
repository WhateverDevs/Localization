using UnityEngine;
using Zenject;

namespace WhateverDevs.Localization
{
    [CreateAssetMenu(fileName = "LocalizationInstaller", menuName = "WhateverDevs/Localization/Installer")]
    public class LocalizationInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings() => Container.Bind<ILocalizer>().To<Localizer>().AsSingle().Lazy();
    }
}