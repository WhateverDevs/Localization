﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Configuration;
using WhateverDevs.Core.Runtime.DataStructures.Integration;
using WhateverDevs.Localization.Runtime.TextPostProcessors;
using Zenject;

namespace WhateverDevs.Localization.Runtime
{
    /// <summary>
    /// Localizer class
    /// </summary>
    [UsedImplicitly]
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

            if (!languagesLoaded) LoadValuesFromResources();
        }

        /// <summary>
        /// Load all the languages from the scriptables objects
        /// </summary>
        private void LoadValuesFromResources() =>
            LoadNewLanguageValues(Resources.LoadAll<ScriptableLanguage>(projectSettings.LanguagePackDirectory)
                                           .ToList());

        /// <summary>
        /// Load new languages to use.
        /// </summary>
        /// <param name="newLanguages">New languages to be loaded.</param>
        public void LoadNewLanguageValues(List<ScriptableLanguage> newLanguages)
        {
            languagePacks = newLanguages;

            Logger.Info("Loaded " + languagePacks.Count + " languages:");

            foreach (ScriptableLanguage language in languagePacks)
                Logger.Info("-->" + language.name + " - " + language.Language.Count + " keys.");

            if (languagePacks.Count == 0)
            {
                Logger.Error("No languages found! Errors will follow.");

                return;
            }

            languagesLoaded = true;

            SetLanguage(GetAllLanguageIds().Contains(configuration.SelectedLanguage)
                            ? configuration.SelectedLanguage
                            : languagePacks[0].name);
        }

        /// <summary>
        /// Retrieve the languages from the Google Sheet again.
        /// This method is very similar to the one on GoogleSheetLoader
        /// but it doesn't save the scriptable objects to the Resources folder since it's only used on runtime.
        /// </summary>
        public void ReDownloadLanguagesFromGoogleSheet()
        {
            languagesLoaded = false;

            List<ScriptableLanguage> newLanguages = new();

            List<string> newTsvData = projectSettings.GoogleSheetsDownloadUrls
                                                     .Select(url =>
                                                                 DriveFileDownloader
                                                                    .GetTextFromUrl(url.Replace("edit?usp=sharing",
                                                                         "export?format=tsv")))
                                                     .ToList();

            const string separator = "\t";

            foreach (string languageSet in newTsvData)
            {
                List<Dictionary<string, string>> gameParametersData =
                    CsvReader.ReadColumnsFromCsv(languageSet, separator);

                List<string> languages =
                    gameParametersData[0].Keys.Where(key => !key.IsNullEmptyOrWhiteSpace()).ToList();

                string keyName = languages[0];

                for (int i = 1; i < languages.Count; i++)
                {
                    string language = languages[i];

                    ScriptableLanguage languageAsset =
                        newLanguages.FirstOrDefault(asset => asset.name == language);

                    if (languageAsset == null)
                    {
                        languageAsset = ScriptableObject.CreateInstance<ScriptableLanguage>();
                        languageAsset.name = language;
                    }

                    foreach (Dictionary<string, string> dictionary in gameParametersData)
                        if (dictionary.TryGetValue(keyName, out string key)
                         && !key.IsNullEmptyOrWhiteSpace())
                        {
                            if (!dictionary.TryGetValue(language, out string value) || value.IsNullEmptyOrWhiteSpace())
                            {
                                StaticLogger.Error("Key " + key + " has no value in language " + language + "!");
                                continue;
                            }

                            languageAsset.Language[key] = value;
                        }

                    newLanguages.AddIfNew(languageAsset);
                }
            }

            LoadNewLanguageValues(newLanguages);
        }

        /// <summary>
        /// Get the localized text in the current language for the given key.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        /// <param name="modifiersAreLocalizableKeys">Are the modifiers localizable keys too?</param>
        /// <param name="valueModifiers">Modifiers to apply to the key, they will substitute "{number}" instances on the value.</param>
        public string this[string key,
                           bool modifiersAreLocalizableKeys = true,
                           params string[] valueModifiers] =>
            GetText(key, modifiersAreLocalizableKeys, valueModifiers);

        /// <summary>
        /// Get the localized text in the current language for the given key.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        /// <param name="modifiersAreLocalizableKeys">Are the modifiers localizable keys too?</param>
        /// <param name="valueModifiers">Modifiers to apply to the key, they will substitute "{number}" instances on the value.</param>
        public string GetText(string key,
                              bool modifiersAreLocalizableKeys = true,
                              params string[] valueModifiers) =>
            GetText(key, currentLanguage, modifiersAreLocalizableKeys, valueModifiers);

        /// <summary>
        /// Get the localized text in the given language for the given key.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        /// <param name="language">Language to retrieve it from.</param>
        /// <param name="modifiersAreLocalizableKeys">Are the modifiers localizable keys too?</param>
        /// <param name="valueModifiers">Modifiers to apply to the key, they will substitute "{number}" instances on the value.</param>
        public string GetText(string key,
                              string language,
                              bool modifiersAreLocalizableKeys = true,
                              params string[] valueModifiers) =>
            GetText(key, GetLanguageIndex(language), modifiersAreLocalizableKeys, valueModifiers);

        /// <summary>
        /// Get the localized text in the given language for the given key.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        /// <param name="languageIndex">Language to retrieve it from.</param>
        /// <param name="modifiersAreLocalizableKeys">Are the modifiers localizable keys too?</param>
        /// <param name="valueModifiers">Modifiers to apply to the key, they will substitute "{number}" instances on the value.</param>
        public string GetText(string key,
                              int languageIndex,
                              bool modifiersAreLocalizableKeys = true,
                              params string[] valueModifiers)
        {
            if (key.IsNullEmptyOrWhiteSpace()) return key;

            if (languagesLoaded)
                return PostProcessRetrievedText(languagePacks[languageIndex][key],
                                                modifiersAreLocalizableKeys,
                                                valueModifiers);

            Logger.Error("The languages are not loaded yet!");
            return key;
        }

        /// <summary>
        /// Post process the retrieved text with the list of modifiers
        /// and the extra text postprocessors.
        /// </summary>
        /// <param name="text">Text to post process.</param>
        /// <param name="modifiersAreLocalizableKeys">Are the modifiers also localizable keys?</param>
        /// <param name="valueModifiers">Modifiers to apply to the key, they will substitute "{number}" instances on the value.</param>
        /// <returns>The post processed text.</returns>
        private string PostProcessRetrievedText(string text,
                                                bool modifiersAreLocalizableKeys,
                                                IReadOnlyList<string> valueModifiers)
        {
            if (valueModifiers != null)
                for (int i = 0; i < valueModifiers.Count; i++)
                {
                    string modifier = valueModifiers[i];

                    text = text.Replace("{" + i + "}", modifiersAreLocalizableKeys ? GetText(modifier) : modifier);
                }

            object[] extraParams = PreparePostProcessorExtraParams();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (LocalizedTextPostProcessor postProcessor in projectSettings.TextPostProcessors)
                if (!postProcessor.PostProcessText(ref text, extraParams))
                    Logger.Error("Error post processing \"" + text + "\" with " + postProcessor.name + ".");

            return text;
        }

        /// <summary>
        /// Prepare the extra params for the post processors.
        /// This may be overriden per application.
        /// </summary>
        protected virtual object[] PreparePostProcessorExtraParams() => new object[] { };

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