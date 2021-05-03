using System;
using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Configuration;
using Zenject;
using Object = System.Object;

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
        private LocalizerConfigurationData configuration;

        /// <summary>
        ///     List of all the languages
        /// </summary>
        private readonly List<LanguagePack> languagePacks = new List<LanguagePack>();

        /// <summary>
        ///     Flag to know
        /// </summary>
        private bool languagesLoaded;

        /// <summary>
        ///     CurrentLanguage
        /// </summary>
        private int currentLanguage;

        public int CurrentLanguage
        {
            get => currentLanguage;
            set => currentLanguage = value;
        }

        #endregion

        #region Load

        /// <summary>
        ///     Init
        /// </summary>
        [Inject]
        public void Construct(IConfigurationManager configurationManager)
        {
            if (!configurationManager.GetConfiguration(out configuration))
                GetLogger().Error("Error retrieving localizer configuration.");

            LoadValues();
        }

        /// <summary>
        ///     Load all the languages from the scriptables objects
        /// </summary>
        private void LoadValues()
        {
            if (languagesLoaded) return;
            string auxPath = configuration.LanguagePackDirectory;

            // TODO : Figure out this warning.
            object[] tempObject = Resources.LoadAll(auxPath, typeof(ScriptableLanguage));

            for (int i = 0; i < tempObject.Length; ++i)
            {
                ScriptableLanguage tempScript = tempObject[i] as ScriptableLanguage;

                if (tempScript == null) continue;

                LanguagePack temp = new LanguagePack
                                    {
                                        Language = (SystemLanguage) Enum.Parse(typeof(SystemLanguage), tempScript.name)
                                    };

                for (int j = 0; j < tempScript.Language.Count; ++j)
                {
                    string key = tempScript.Language[j].Key;
                    string value = tempScript.Language[j].Value;
                    temp.AddNewString(key, value);
                }

                languagePacks.Add(temp);
            }

            languagesLoaded = true;
        }

        #endregion

        /// <summary>
        ///     Getting the value for a key in the current language
        /// </summary>
        public string GetText(string key)
        {
            if (languagesLoaded) return languagePacks[currentLanguage].GetString(key);

            GetLogger().Error("The languages are not loaded yet!!");
            return "The languages are not loaded yet!!";
        }

        public string this[string key] => GetText(key);
    }
}