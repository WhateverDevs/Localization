using UnityEngine;
using Zenject;

namespace WhateverDevs.Localization.Runtime
{
    [CreateAssetMenu(fileName = "LocalizationInstaller", menuName = "WhateverDevs/DI/Localization")]
    public class LocalizationInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings() => Container.Bind<ILocalizer>().To<Localizer>().AsSingle().Lazy();
    }
}