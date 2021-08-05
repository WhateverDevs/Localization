using TMPro;
using UnityEngine;
using WhateverDevs.Core.Runtime.Ui;
using Zenject;

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
        [Inject]
        public ILocalizer Localizer;

        /// <summary>
        /// Set the value if on enable checked and subscribe to language change.
        /// </summary>
        private void OnEnable()
        {
            if (SetOnEnable) OnLanguageChanged();
            Localizer.SubscribeToLanguageChange(OnLanguageChanged);
        }

        /// <summary>
        /// Unsubscribe from language change.
        /// </summary>
        private void OnDisable() => Localizer.UnsubscribeFromLanguageChange(OnLanguageChanged);

        /// <summary>
        /// Set a new value and refresh.
        /// </summary>
        /// <param name="key"></param>
        public void SetValue(string key)
        {
            LocalizationKey = key;
            OnLanguageChanged();
        }

        /// <summary>
        /// Called every time the language changes.
        /// </summary>
        private void OnLanguageChanged() => UpdateText(Localizer[LocalizationKey]);
    }
}