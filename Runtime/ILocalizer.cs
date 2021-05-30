using System;

namespace WhateverDevs.Localization.Runtime
{
    /// <summary>
    ///     Interface that defines how a configuration manager should work.
    /// </summary>
    public interface ILocalizer
    {
        /// <summary>
        ///     List of all the configurations this manager handles.
        /// </summary>
        string GetText(string key);

        string this[string key] { get; }

        string GetCurrentLanguage();
        
        int GetCurrentLanguageId();

        void SetLanguage(string language);
        
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
    }
}