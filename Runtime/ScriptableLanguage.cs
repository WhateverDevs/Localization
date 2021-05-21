using System.Collections.Generic;
using UnityEngine;

namespace WhateverDevs.Localization.Runtime
{
    /// <summary>
    /// Scriptable class to collect all the values for a language
    /// </summary>
    public class ScriptableLanguage : ScriptableObject
    {
        public List<LanguagePair> Language = new List<LanguagePair>();

        public void SetValues(string[] values, string[] keys)
        {
            for (int i = 0; i < values.Length; ++i)
            {
                LanguagePair pair = new LanguagePair
                                    {
                                        Key = keys[i],
                                        Value = values[i]
                                    };

                Language.Add(pair);
            }
        }
    }
}