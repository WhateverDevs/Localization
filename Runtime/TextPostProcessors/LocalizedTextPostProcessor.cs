using WhateverDevs.Core.Behaviours;

namespace WhateverDevs.Localization.Runtime.TextPostProcessors
{
    /// <summary>
    /// Base class for a post processor that can modify a text after it's been localized.
    /// </summary>
    public abstract class LocalizedTextPostProcessor : WhateverScriptable<LocalizedTextPostProcessor>
    {
        /// <summary>
        /// Method called to post process a text.
        /// </summary>
        /// <param name="text">Text to post process.</param>
        /// <param name="extraParams">Extra parameters provided. May vary per application.</param>
        /// <returns>True if the text was successfully postprocessed.</returns>
        public abstract bool PostProcessText(ref string text, params object[] extraParams);
    }
}