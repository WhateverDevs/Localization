using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Configuration.
        /// </summary>
        private LocalizerConfiguration configuration;

        /// <summary>
        /// Project settings for the localizer.
        /// </summary>
        private LocalizerSettings projectSettings;

        /// <summary>
        /// List of all the languages.
        /// </summary>
        private List<ScriptableLanguage> languagePacks;

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
        /// Event raised when the language is changed.
        /// </summary>
        private Action languageChangedNoParam;

        /// <summary>
        /// Cache of language Ids.
        /// </summary>
        private List<string> languageIds;

        /// <summary>
        /// Init.
        /// </summary>
        [Inject]
        public void Construct(IConfigurationManager configurationManagerReference, LocalizerSettings settings)
        {
            projectSettings = settings;
            configurationManager = configurationManagerReference;

            if (!configurationManager.GetConfiguration(out configuration))
                GetLogger().Error("Error retrieving localizer configuration.");

            languageChanged += _ => languageChangedNoParam?.Invoke();

            LoadValues();

            if (languagePacks.Count == 0)
            {
                Logger.Error("No languages found! Errors will follow.");

                return;
            }

            SetLanguage(GetAllLanguageIds().Contains(configuration.SelectedLanguage)
                            ? configuration.SelectedLanguage
                            : languagePacks[0].name);

            languagesLoaded = true;
        }

        /// <summary>
        /// Load all the languages from the scriptables objects
        /// </summary>
        private void LoadValues()
        {
            if (languagesLoaded) return;
            string auxPath = projectSettings.LanguagePackDirectory;

            languagePacks = Resources.LoadAll<ScriptableLanguage>(auxPath).ToList();

            Logger.Info("Loaded " + languagePacks.Count + " languages:");

            foreach (ScriptableLanguage language in languagePacks) Logger.Info("-->" + language.name);
        }

        /// <summary>
        /// Get the localized text in the current language for the given key.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        public string this[string key] => GetText(key);

        /// <summary>
        /// Get the localized text in the current language for the given key.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        public string GetText(string key) => GetText(key, currentLanguage);

        /// <summary>
        /// Get the localized text in the given language for the given key.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        /// <param name="language">Language to retrieve it from.</param>
        /// <returns></returns>
        public string GetText(string key, string language) => GetText(key, GetLanguageIndex(language));

        /// <summary>
        /// Get the localized text in the given language for the given key.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        /// <param name="languageIndex">Language to retrieve it from.</param>
        /// <returns></returns>
        public string GetText(string key, int languageIndex)
        {
            if (languagesLoaded) return languagePacks[languageIndex][key];

            Logger.Error("The languages are not loaded yet!");
            return key;
        }

        /// <summary>
        /// Get a list of localized texts in the current language for the given keys.
        /// </summary>
        /// <param name="keys">Keys to retrieve.</param>
        /// <returns>A list of the localized texts.</returns>
        public List<string> GetTexts(List<string> keys) => keys.Select(key => GetText(key)).ToList();

        /// <summary>
        /// Get a list of localized texts in the given language for the given keys.
        /// </summary>
        /// <param name="keys">Keys to retrieve.</param>
        /// <param name="language">Language to retrieve them from.</param>
        /// <returns>A list of the localized texts.</returns>
        public List<string> GetTexts(List<string> keys, string language) =>
            keys.Select(key => GetText(key, GetLanguageIndex(language))).ToList();

        /// <summary>
        /// Get a list of localized texts in the given language for the given keys.
        /// </summary>
        /// <param name="keys">Keys to retrieve.</param>
        /// <param name="languageIndex">Language to retrieve them from.</param>
        /// <returns>A list of the localized texts.</returns>
        public List<string> GetTexts(List<string> keys, int languageIndex) =>
            keys.Select(key => GetText(key, languageIndex)).ToList();

        /// <summary>
        /// Get a list of localized texts in the given languages for the given keys.
        /// </summary>
        /// <param name="keys">Keys to retrieve.</param>
        /// <param name="languages">Languages to retrieve them from.</param>
        /// <returns>A dictionary with the language as the key and a list of the localized texts as the value.</returns>
        public Dictionary<string, List<string>> GetTexts(List<string> keys, List<string> languages) =>
            languages.ToDictionary(language => language, language => GetTexts(keys, language));

        /// <summary>
        /// Get a list of localized texts in the given languages for the given keys.
        /// </summary>
        /// <param name="keys">Keys to retrieve.</param>
        /// <param name="languageIndexes">Languages to retrieve them from.</param>
        /// <returns>A dictionary with the language as the key and a list of the localized texts as the value.</returns>
        public Dictionary<string, List<string>> GetTexts(List<string> keys, List<int> languageIndexes) =>
            languageIndexes.ToDictionary(index => languagePacks[index].name, index => GetTexts(keys, index));

        /// <summary>
        /// Get the current language key.
        /// </summary>
        /// <returns>A localizable string key for the name of the current language.</returns>
        public string GetCurrentLanguage() => languagePacks[currentLanguage].name;

        /// <summary>
        /// Get the current language index.
        /// </summary>
        /// <returns>The index of the current language.</returns>
        public int GetCurrentLanguageIndex() => currentLanguage;

        /// <summary>
        /// Get the index of the given language key.
        /// </summary>
        /// <param name="language">Language to get.</param>
        /// <returns>Its index.</returns>
        public int GetLanguageIndex(string language)
        {
            for (int i = 0; i < languagePacks.Count; ++i)
                if (languagePacks[i].name == language)
                    return i;

            Logger.Error("Language " + language + " does not exist!");

            return -1;
        }

        /// <summary>
        /// Retrieve all the language Ids.
        /// </summary>
        /// <returns>A list of all the language Ids.</returns>
        public List<string> GetAllLanguageIds()
        {
            if (languageIds != null) return languageIds;

            languageIds = new List<string>();

            for (int i = 0; i < languagePacks.Count; ++i) languageIds.Add(languagePacks[i].name);

            return languageIds;
        }

        /// <summary>
        /// Set the current language by key.
        /// </summary>
        /// <param name="language">Language to set.</param>
        public void SetLanguage(string language)
        {
            for (int i = 0; i < languagePacks.Count; ++i)
            {
                if (languagePacks[i].name != language) continue;
                SetLanguage(i);
                return;
            }

            Logger.Error("Language " + language + " does not exist!");
        }

        /// <summary>
        /// Set the current language by index.
        /// </summary>
        /// <param name="language">Language to set.</param>
        public void SetLanguage(int language)
        {
            currentLanguage = language;

            configuration.SelectedLanguage = languagePacks[currentLanguage].name;
            if (!configurationManager.SetConfiguration(configuration)) Logger.Error("Error saving configuration!");

            languageChanged?.Invoke(GetCurrentLanguage());
        }

        /// <summary>
        /// Subscribe to the language changed event.
        /// </summary>
        /// <param name="callback">Called each time the language changes.</param>
        public void SubscribeToLanguageChange(Action<string> callback) => languageChanged += callback;

        /// <summary>
        /// Subscribe to the language changed event.
        /// </summary>
        /// <param name="callback">Called each time the language changes.</param>
        public void SubscribeToLanguageChange(Action callback) => languageChangedNoParam += callback;

        /// <summary>
        /// Unsubscribe from the language changed event.
        /// </summary>
        /// <param name="callback">Callback to unsubscribe.</param>
        public void UnsubscribeFromLanguageChange(Action<string> callback) => languageChanged -= callback;

        /// <summary>
        /// Unsubscribe from the language changed event.
        /// </summary>
        /// <param name="callback">Callback to unsubscribe.</param>
        public void UnsubscribeFromLanguageChange(Action callback) => languageChangedNoParam -= callback;
    }
}