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
        /// <param name="key"></param>
        string GetText(string key);

        /// <summary>
        /// Get the localized text in the current language for the given key.
        /// </summary>
        /// <param name="key"></param>
        string this[string key] { get; }

        /// <summary>
        /// Get the current language key.
        /// </summary>
        /// <returns></returns>
        string GetCurrentLanguage();
        
        /// <summary>
        /// Get the current language Id.
        /// </summary>
        /// <returns></returns>
        int GetCurrentLanguageId();

        /// <summary>
        /// Retrieve all the language Ids.
        /// </summary>
        /// <returns>A list of all the language Ids.</returns>
        List<string> GetAllLanguageIds();

        /// <summary>
        /// Set the current language by key.
        /// </summary>
        /// <param name="language"></param>
        void SetLanguage(string language);
        
        /// <summary>
        /// Set the current language by Id.
        /// </summary>
        /// <param name="language"></param>
        void SetLanguage(int language);

        /// <summary>
        /// Subscribe to the language changed event.
        /// </summary>
        /// <param name="callback"></param>
        void SubscribeToLanguageChange(Action<string> callback);
        
        /// <summary>
        /// Subscribe to the language changed event.
        /// </summary>
        void SubscribeToLanguageChange(Action callback);
        
        /// <summary>
        /// Unsubscribe from the language changed event.
        /// </summary>
        /// <param name="callback"></param>
        void UnsubscribeFromLanguageChange(Action<string> callback);
        
        /// <summary>
        /// Unsubscribe from the language changed event.
        /// </summary>
        void UnsubscribeFromLanguageChange(Action callback);
    }
}