using System.Collections.Generic;
using UnityEngine;

namespace WhateverDevs.Localization
{
    /// <summary>
    /// Class to save the key with the proper Language values
    /// </summary>
    [System.Serializable]
    public class LanguagePair
    {
        public string Key = "";
        public string Value = "";
    }


    /// <summary>
    /// Class to collect all the info 
    /// </summary>
    [System.Serializable]
    public class LanguagePack
    {
        public Dictionary<string, string> strings = new Dictionary<string, string>();

        public bool AddNewString(string key, string text)
        {
            if (!strings.ContainsKey(key))
            {
                strings.Add(key, text);
                return true;
            }
            else
            {
                Debug.LogError("This key already exist : " + key); //todo modify to use the logger??
                return false;
            }
        }


        /// <summary>
        /// Try to get the value for an specific key
        /// </summary>
        /// <param name="key">key para obtener el texto en función del idioma</param>
        public string GetString(string key)
        {
            if (strings.ContainsKey(key))
            {
                return strings[key];
            }

            return "BAD KEY" + key;
        }
    }
}