using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DataStructures;

namespace WhateverDevs.Localization.Runtime
{
    /// <summary>
    /// Scriptable class to collect all the values for a language
    /// </summary>
    public class ScriptableLanguage : WhateverScriptable<ScriptableLanguage>
    {
        /// <summary>
        /// Dictionary of all values and texts for this language.
        /// </summary>
        public SerializableDictionary<string, string> Language = new();

        /// <summary>
        /// Get the localized text in the current language for the given key.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        public string this[string key]
        {
            get
            {
                if (Language.TryGetValue(key, out string value)) return value;
                
                Logger.Warn("Key " + key + " not found in language " + name + ".");
                return key;
            }
        }
    }
}