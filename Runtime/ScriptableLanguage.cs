using UnityEngine;
using WhateverDevs.Core.Runtime.DataStructures;

namespace WhateverDevs.Localization.Runtime
{
    /// <summary>
    /// Scriptable class to collect all the values for a language
    /// </summary>
    public class ScriptableLanguage : ScriptableObject
    {
        public SerializableDictionary<string, string> Language = new SerializableDictionary<string, string>();
    }
}