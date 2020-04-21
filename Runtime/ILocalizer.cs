namespace WhateverDevs.Localization
{
    /// <summary>
    ///     Interface that defines how a configuration manager should work.
    /// </summary>
    public interface ILocalizer
    {
        /// <summary>
        ///     List of all the configurations this manager handles.
        /// </summary>
        string GetText(string key);
    }
}