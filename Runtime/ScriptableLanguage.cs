using System.Collections.Generic;
using UnityEngine;

namespace WhateverDevs.Localization
{
    /// <summary>
    ///     Scriptabale class to collect all the values for a language
    /// </summary>
    public class ScriptableLanguage : ScriptableObject
    {
        public List<LanguagePair> Language = new List<LanguagePair>();

        public void SetValues(string[] _values, string[] _keys)
        {
            for (int i = 0; i < _values.Length; ++i)
            {
                LanguagePair pair = new LanguagePair
                                    {
                                        Key = _keys[i],
                                        Value = _values[i]
                                    };

                Language.Add(pair);
            }
        }
    }
}