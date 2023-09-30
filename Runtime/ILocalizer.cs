using System;
using System.Collections.Generic;

namespace WhateverDevs.Localization.Runtime
{
    /// <summary>
    ///     Interface that defines how a localization manager should work.
    /// </summary>
    public interface ILocalizer
    {
        /// <summary>
        /// Get the localized text in the current language for the given key.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        /// <param name="modifiersAreLocalizableKeys">Are the modifiers localizable keys too?</param>
        /// <param name="valueModifiers">Modifiers to apply to the key, they will substitute "{number}" instances on the value.</param>
        string this[string key,
                    bool modifiersAreLocalizableKeys = true,
                    params string[] valueModifiers] { get; }

        /// <summary>
        /// Get the localized text in the current language for the given key.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        /// <param name="modifiersAreLocalizableKeys">Are the modifiers localizable keys too?</param>
        /// <param name="valueModifiers">Modifiers to apply to the key, they will substitute "{number}" instances on the value.</param>
        string GetText(string key,
                       bool modifiersAreLocalizableKeys = true,
                       params string[] valueModifiers);

        /// <summary>
        /// Get the localized text in the given language for the given key.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        /// <param name="language">Language to retrieve it from.</param>
        /// <param name="modifiersAreLocalizableKeys">Are the modifiers localizable keys too?</param>
        /// <param name="valueModifiers">Modifiers to apply to the key, they will substitute "{number}" instances on the value.</param>
        string GetText(string key,
                       string language,
                       bool modifiersAreLocalizableKeys = true,
                       params string[] valueModifiers);

        /// <summary>
        /// Get the localized text in the given language for the given key.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        /// <param name="languageIndex">Language to retrieve it from.</param>
        /// <param name="modifiersAreLocalizableKeys">Are the modifiers localizable keys too?</param>
        /// <param name="valueModifiers">Modifiers to apply to the key, they will substitute "{number}" instances on the value.</param>
        string GetText(string key,
                       int languageIndex,
                       bool modifiersAreLocalizableKeys = true,
                       params string[] valueModifiers);

        /// <summary>
        /// Get a list of localized texts in the current language for the given keys.
        /// </summary>
        /// <param name="keys">Keys to retrieve.</param>
        /// <returns>A list of the localized texts.</returns>
        List<string> GetTexts(List<string> keys);

        /// <summary>
        /// Get a list of localized texts in the given language for the given keys.
        /// </summary>
        /// <param name="keys">Keys to retrieve.</param>
        /// <param name="language">Language to retrieve them from.</param>
        /// <returns>A list of the localized texts.</returns>
        List<string> GetTexts(List<string> keys, string language);

        /// <summary>
        /// Get a list of localized texts in the given language for the given keys.
        /// </summary>
        /// <param name="keys">Keys to retrieve.</param>
        /// <param name="languageIndex">Language to retrieve them from.</param>
        /// <returns>A list of the localized texts.</returns>
        List<string> GetTexts(List<string> keys, int languageIndex);

        /// <summary>
        /// Get a list of localized texts in the given languages for the given keys.
        /// </summary>
        /// <param name="keys">Keys to retrieve.</param>
        /// <param name="languages">Languages to retrieve them from.</param>
        /// <returns>A dictionary with the language as the key and a list of the localized texts as the value.</returns>
        Dictionary<string, List<string>> GetTexts(List<string> keys, List<string> languages);

        /// <summary>
        /// Get a list of localized texts in the given languages for the given keys.
        /// </summary>
        /// <param name="keys">Keys to retrieve.</param>
        /// <param name="languageIndexes">Languages to retrieve them from.</param>
        /// <returns>A dictionary with the language as the key and a list of the localized texts as the value.</returns>
        Dictionary<string, List<string>> GetTexts(List<string> keys, List<int> languageIndexes);

        /// <summary>
        /// Get the current language key.
        /// </summary>
        /// <returns>A localizable string key for the name of the current language.</returns>
        string GetCurrentLanguage();

        /// <summary>
        /// Get the current language index.
        /// </summary>
        /// <returns>The index of the current language.</returns>
        int GetCurrentLanguageIndex();

        /// <summary>
        /// Get the index of the given language key.
        /// </summary>
        /// <param name="language">Language to get.</param>
        /// <returns>Its index.</returns>
        int GetLanguageIndex(string language);

        /// <summary>
        /// Retrieve all the language Ids.
        /// </summary>
        /// <returns>A list of all the language Ids.</returns>
        List<string> GetAllLanguageIds();

        /// <summary>
        /// Set the current language by key.
        /// </summary>
        /// <param name="language">Language to set.</param>
        void SetLanguage(string language);

        /// <summary>
        /// Set the current language by index.
        /// </summary>
        /// <param name="language">Language to set.</param>
        void SetLanguage(int language);

        /// <summary>
        /// Subscribe to the language changed event.
        /// </summary>
        /// <param name="callback">Called each time the language changes.</param>
        void SubscribeToLanguageChange(Action<string> callback);

        /// <summary>
        /// Subscribe to the language changed event.
        /// </summary>
        /// <param name="callback">Called each time the language changes.</param>
        void SubscribeToLanguageChange(Action callback);

        /// <summary>
        /// Unsubscribe from the language changed event.
        /// </summary>
        /// <param name="callback">Callback to unsubscribe.</param>
        void UnsubscribeFromLanguageChange(Action<string> callback);

        /// <summary>
        /// Unsubscribe from the language changed event.
        /// </summary>
        /// <param name="callback">Callback to unsubscribe.</param>
        void UnsubscribeFromLanguageChange(Action callback);
    }
}