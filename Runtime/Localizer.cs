using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace WhateverDevs.Localization
{
    /// <summary>
    /// Localizer class
    /// </summary>
    public class Localizer
    {
        public enum eLanguage
        {
            SPA,
            ENG,
            FRE, 
            GER,
            COUNT
        }

        #region Params

        /// <summary>
        /// List of all the languages
        /// </summary>
        private static List<LanguagePack> _LanguagesPack = new List<LanguagePack>();

        /// <summary>
        /// Path where to save the scriptables with the info
        /// </summary>
        protected static string _LanguagePackDirectory = "ScriptableResources/Languages/";// should be variable??

        /// <summary>
        /// Flag to know 
        /// </summary>
        private static bool _LanguagesLoaded = false;

        /// <summary>
        /// CurrentLanguage
        /// </summary>
        private static eLanguage m_CurrentLanguage = eLanguage.ENG;
        public eLanguage CurrentLanguage => m_CurrentLanguage;

        #endregion



        #region Load

        /// <summary>
        /// Load all the languages from the scriptables objects
        /// </summary>
        public void LoadValues()
        {
            if (_LanguagesLoaded == false)
            {
                for (int i = 0; i < (int) eLanguage.COUNT; ++i)
                {
                    string auxPath = _LanguagePackDirectory + eLanguage.GetName(typeof(eLanguage), (eLanguage) i);

                    ScriptableLanguage tempScript =
                        Resources.Load(auxPath, typeof(ScriptableLanguage)) as ScriptableLanguage;

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
        /// Getting the value for a key in the current language 
        /// </summary>
        public string GetText(string _key)
        {
            if (_LanguagesLoaded)
            {
                return _LanguagesPack[(int) m_CurrentLanguage].GetString(_key);
            }
            else
            {
                return "The languages are not loaded yet!!";
            }
        }
    }
}

