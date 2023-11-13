using TMPro;
using UnityEngine;
using WhateverDevs.Core.Runtime.Ui;
using Zenject;
#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif

namespace WhateverDevs.Localization.Runtime.Ui
{
    /// <summary>
    /// MonoBehaviour that can be used to assign a localized text to a TMP Text through inspector.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTextMeshPro : EasyUpdateText<LocalizedTextMeshPro>
    {
        /// <summary>
        /// Should the value be set on enable?
        /// </summary>
        public bool SetOnEnable = true;

        /// <summary>
        /// Key to display.
        /// </summary>
        public string LocalizationKey;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        protected ILocalizer Localizer;

        /// <summary>
        /// Are the modifiers localizable keys too?
        /// </summary>
        protected bool LocalizableModifiers;

        /// <summary>
        /// Modifiers that can be applied to the display value.
        /// </summary>
        protected string[] Modifiers;

        /// <summary>
        /// Retrieve the reference to the localizer.
        /// </summary>
        [Inject]
        private void Construct(ILocalizer localizerReference) => Localizer = localizerReference;

        /// <summary>
        /// Set the value if on enable checked and subscribe to language change.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (SetOnEnable) OnLanguageChanged();
            SubscribeToLocalizer();
        }

        /// <summary>
        /// Subscribe to the localizer changes.
        /// </summary>
        private void SubscribeToLocalizer()
        {
            Localizer?.UnsubscribeFromLanguageChange(OnLanguageChanged);
            Localizer?.SubscribeToLanguageChange(OnLanguageChanged);
        }

        /// <summary>
        /// Unsubscribe from language change.
        /// </summary>
        protected virtual void OnDisable() => Localizer?.UnsubscribeFromLanguageChange(OnLanguageChanged);

        /// <summary>
        /// Set a new value and refresh.
        /// </summary>
        /// <param name="key">Key to localize.</param>
        /// <param name="modifiersAreLocalizableKeys">Are the modifiers localizable keys too?</param>
        /// <param name="valueModifiers">Modifiers to apply to the key, they will substitute "{number}" instances on the value.</param>
        #if ODIN_INSPECTOR_3
        [Button]
        [HideInEditorMode]
        #endif
        public virtual void SetValue(string key,
                                     bool modifiersAreLocalizableKeys = true,
                                     params string[] valueModifiers)
        {
            LocalizationKey = key;

            Modifiers = valueModifiers;

            LocalizableModifiers = modifiersAreLocalizableKeys;

            OnLanguageChanged();
        }

        /// <summary>
        /// Called every time the language changes.
        /// </summary>
        protected virtual void OnLanguageChanged() =>
            UpdateText(Localizer[LocalizationKey, LocalizableModifiers, Modifiers]);
    }
}