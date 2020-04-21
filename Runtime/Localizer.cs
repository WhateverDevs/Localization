using System;
using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localizer.Runtime;

namespace WhateverDevs.Localization
{
    /// <summary>
    ///     Localizer class
    /// </summary>
    public class Localizer : Loggable<Localizer>, ILocalizer
    {
        #region Params
        /// <summary>
        /// Configuration
        /// </summary>
        protected LocalizerConfiguration configuration;
        
        /// <summary>
        ///     List of all the languages
        /// </summary>
        private readonly List<LanguagePack> _LanguagesPack = new List<LanguagePack>();

        /// <summary>
        ///     Flag to know
        /// </summary>
        private bool _LanguagesLoaded;

        /// <summary>
        ///     CurrentLanguage
        /// </summary>
        private int _CurrentLanguage = 0;

        public int CurrentLanguage => _CurrentLanguage;

        #endregion

        #region Load

        /// <summary>
        ///     Init
        /// </summary>
        public void Init() => LoadValues();

        /// <summary>
        ///     Load all the languages from the scriptables objects
        /// </summary>
        private void LoadValues()
        {
            if (_LanguagesLoaded == false)
            {
                string auxPath = configuration.ConfigurationData.languagePackDirectory;

                object[] tempObject = Resources.LoadAll(auxPath, typeof(ScriptableLanguage));

                for (int i = 0; i < tempObject.Length; ++i)
                {
                    ScriptableLanguage tempScript = tempObject[i] as ScriptableLanguage;

                    if (tempScript != null)
                    {
                        LanguagePack temp = new LanguagePack();

                        for (int j = 0; j < tempScript.Language.Count; ++j)
                        {
                            string key = tempScript.Language[j].Key;
                            string value = tempScript.Language[j].Value;
                            temp.AddNewString(key, value);
                        }

                        _LanguagesPack.Add(temp);
                    }
                }

                _LanguagesLoaded = true;
            }
        }

        #endregion

        /// <summary>
        ///     Getting the value for a key in the current language
        /// </summary>
        public string GetText(string _key)
        {
            if (_LanguagesLoaded) return _LanguagesPack[_CurrentLanguage].GetString(_key);

            GetLogger().Error("The languages are not loaded yet!!");
            return "The languages are not loaded yet!!";
        }

        public string this[string key] => GetText(key);
    }
}