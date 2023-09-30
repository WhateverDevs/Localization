using UnityEngine;

namespace WhateverDevs.Localization.Runtime.TextPostProcessors
{
    /// <summary>
    /// Text post processor that replaces a substring with another substring.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/Localization/TextPostProcessors/ReplaceSubstring",
                     fileName = "ReplaceSubstring")]
    public class ReplaceSubstring : LocalizedTextPostProcessor
    {
        /// <summary>
        /// Original to replace.
        /// </summary>
        [SerializeField]
        [TextArea]
        private string Original;

        /// <summary>
        /// Replacement substring.
        /// </summary>
        [SerializeField]
        [TextArea]
        private string Replacement;

        /// <summary>
        /// Replace the substring.
        /// </summary>
        /// <param name="text">Text to post process.</param>
        /// <param name="extraParams">Extra parameters provided. May vary per application.</param>
        /// <returns>True if the text was successfully postprocessed.</returns>
        public override bool PostProcessText(ref string text, params object[] extraParams)
        {
            text = text.Replace(Original, Replacement);
            return true;
        }
    }
}