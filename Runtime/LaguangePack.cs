using System;
using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;

namespace WhateverDevs.Localization
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
        public SystemLanguage Language = SystemLanguage.English;
        
        public Dictionary<string, string> strings = new Dictionary<string, string>();

        public bool AddNewString(string key, string text)
        {
            if (!strings.ContainsKey(key))
            {
                strings.Add(key, text);
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
            if (strings.ContainsKey(key)) return strings[key];

            GetLogger().Error("Bad key :" + key);
            return "BAD KEY" + key;
        }
    }
}