using System;
using TMPro;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using Zenject;

namespace WhateverDevs.Localization.Runtime.Ui
{
    /// <summary>
    /// MonoBehaviour that can be used to assign a localized text to a TMP Text through inspector.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTextMeshPro : LoggableMonoBehaviour<LocalizedTextMeshPro>
    {
        /// <summary>
        /// Key to display.
        /// </summary>
        public string LocalizationKey;

        /// <summary>
        /// Reference to the TMP text.
        /// </summary>
        private TMP_Text Text
        {
            get
            {
                if (text == null) text = GetComponent<TMP_Text>();
                return text;
            }
        }

        /// <summary>
        /// Backfield for Text.
        /// </summary>
        private TMP_Text text;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        public ILocalizer Localizer;

        private void OnEnable()
        {
            OnLanguageChanged();
            Localizer.SubscribeToLanguageChange(OnLanguageChanged);
        }

        /// <summary>
        /// Called every time the language changes.
        /// </summary>
        private void OnLanguageChanged() => Text.SetText(Localizer[LocalizationKey]);
    }
}