using System;
using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Configuration;
using Zenject;

namespace WhateverDevs.Localization.Runtime
{
    /// <summary>
    /// Localizer class
    /// </summary>
    public class Localizer : Loggable<Localizer>, ILocalizer
    {
        /// <summary>
        /// Configuration
        /// </summary>
        private LocalizerConfigurationData configuration;

        /// <summary>
        /// List of all the languages
        /// </summary>
        private readonly List<LanguagePack> languagePacks = new List<LanguagePack>();

        /// <summary>
        /// Flag to know
        /// </summary>
        private bool languagesLoaded;

        /// <summary>
        /// CurrentLanguage
        /// </summary>
        private int currentLanguage;

        /// <summary>
        /// Reference to the configuration manager.
        /// </summary>
        private IConfigurationManager configurationManager;

        /// <summary>
        /// Event raised when the language is changed.
        /// </summary>
        private Action<string> languageChanged;

        /// <summary>
        /// Cache of language Ids.
        /// </summary>
        private List<string> languageIds;

        /// <summary>
        /// Init.
        /// </summary>
        [Inject]
        public void Construct(IConfigurationManager configurationManagerReference)
        {
            configurationManager = configurationManagerReference;

            if (!configurationManager.GetConfiguration(out configuration))
                GetLogger().Error("Error retrieving localizer configuration.");

            LoadValues();

            SetLanguage(configuration.SelectedLanguage);
        }

        /// <summary>
        /// Load all the languages from the scriptables objects
        /// </summary>
        private void LoadValues()
        {
            if (languagesLoaded) return;
            string auxPath = configuration.LanguagePackDirectory;

            ScriptableLanguage[] tempObject = Resources.LoadAll<ScriptableLanguage>(auxPath);

            for (int i = 0; i < tempObject.Length; ++i)
            {
                ScriptableLanguage tempScript = tempObject[i];

                if (tempScript == null) continue;

                LanguagePack temp = new LanguagePack {Language = tempScript.name};

                for (int j = 0; j < tempScript.Language.Count; ++j)
                {
                    string key = tempScript.Language[j].Key;
                    string value = tempScript.Language[j].Value;
                    temp.AddNewString(key, value);
                }

                languagePacks.Add(temp);
            }

            languagesLoaded = true;
        }

        /// <summary>
        /// Getting the value for a key in the current language
        /// </summary>
        public string GetText(string key)
        {
            if (languagesLoaded) return languagePacks[currentLanguage].GetString(key);

            GetLogger().Error("The languages are not loaded yet!!");
            return "The languages are not loaded yet!!";
        }

        public string this[string key] => GetText(key);

        public string GetCurrentLanguage() => languagePacks[currentLanguage].Language;

        public int GetCurrentLanguageId() => currentLanguage;

        /// <summary>
        /// Retrieve all the language Ids.
        /// </summary>
        /// <returns>A list of all the language Ids.</returns>
        public List<string> GetAllLanguageIds()
        {
            if (languageIds != null) return languageIds;

            languageIds = new List<string>();

            for (int i = 0; i < languagePacks.Count; ++i) languageIds.Add(languagePacks[i].Language);

            return languageIds;
        }

        public void SetLanguage(string language)
        {
            for (int i = 0; i < languagePacks.Count; ++i)
            {
                if (languagePacks[i].Language != language) continue;
                SetLanguage(i);
                return;
            }

            Logger.Error("Language " + language + " does not exist!");
        }

        public void SetLanguage(int language)
        {
            currentLanguage = language;

            configuration.SelectedLanguage = languagePacks[currentLanguage].Language;
            if (!configurationManager.SetConfiguration(configuration)) Logger.Error("Error saving configuration!");

            languageChanged?.Invoke(GetCurrentLanguage());
        }

        /// <summary>
        /// Subscribe to the language changed event.
        /// </summary>
        /// <param name="callback"></param>
        public void SubscribeToLanguageChange(Action<string> callback) => languageChanged += callback;

        /// <summary>
        /// Subscribe to the language changed event.
        /// </summary>
        public void SubscribeToLanguageChange(Action callback) => languageChanged += language => callback?.Invoke();
    }
}