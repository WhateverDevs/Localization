using System;
using System.Collections.Generic;
using WhateverDevs.Core.Runtime.Common;

namespace WhateverDevs.Localization.Runtime
{
    /// <summary>
    ///     Class to save the key with the proper Language values
    /// </summary>
    [Serializable]
    public class LanguagePair
    {
        public string Key = "";
        public string Value = "";
    }

    /// <summary>
    ///     Class to collect all the info
    /// </summary>
    [Serializable]
    public class LanguagePack : Loggable<LanguagePack>
    {
        public string Language;
        
        public Dictionary<string, string> Strings = new Dictionary<string, string>();

        public bool AddNewString(string key, string text)
        {
            if (!Strings.ContainsKey(key))
            {
                Strings.Add(key, text);
                return true;
            }

            GetLogger().Error("This key already exist : " + key);
            return false;
        }

        /// <summary>
        ///     Try to get the value for an specific key
        /// </summary>
        /// <param name="key">key to get the localized text</param>
        public string GetString(string key)
        {
            if (Strings.ContainsKey(key)) return Strings[key];

            GetLogger().Error("Bad key :" + key);
            return key;
        }
    }
}