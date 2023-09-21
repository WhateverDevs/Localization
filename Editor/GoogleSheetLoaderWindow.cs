using System.Linq;
using UnityEditor;
using UnityEngine;
using WhateverDevs.Core.Editor.Utils;
using WhateverDevs.Localization.Runtime;

namespace WhateverDevs.Localization.Editor
{
    /// <summary>
    /// Default implementation of the google sheet loader.
    /// </summary>
    public class
        DefaultGoogleSheetLoaderWindow : GoogleSheetsLoaderWindow<LocalizerConfigurationFile, LocalizerConfiguration>
    {
        [MenuItem("WhateverDevs/Localization/GoogleDrive #&l")]
        public static void ShowWindow() =>
            GetWindow(typeof(DefaultGoogleSheetLoaderWindow), false, "Drive Sheet Parser");
    }

    /// <summary>
    /// Window to load different Google sheets.
    /// </summary>
    public abstract class GoogleSheetsLoaderWindow<TLocalizerConfigurationFile, TLocalizerConfiguration> : EditorWindow
        where TLocalizerConfigurationFile : LocalizerConfigurationFile<TLocalizerConfiguration>, new()
        where TLocalizerConfiguration : LocalizerConfiguration, new()
    {
        private TLocalizerConfigurationFile configurationFile;

        private string downLoadUrl = "";

        private bool deleteFileWhenFinished = true;

        private void OnEnable()
        {
            downLoadUrl = EditorGUIUtility.systemCopyBuffer;

            try
            {
                EditorUtility.DisplayProgressBar("Localization", "Looking for configuration...", .5f);
                configurationFile = AssetManagementUtils.FindAssetsByType<TLocalizerConfigurationFile>().First();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void OnGUI()
        {
            if (configurationFile == null)
            {
                configurationFile = (TLocalizerConfigurationFile)
                    EditorGUILayout.ObjectField(new GUIContent("Configuration"),
                                                configurationFile,
                                                typeof(TLocalizerConfigurationFile),
                                                false);

                return;
            }

            EditorGUILayout.HelpBox("The file in drive must be shared publicly!", MessageType.Warning);

            EditorGUILayout.HelpBox("This tool downloads the file as a tsv with tab separators. "
                                  + "If you have tabs inside your localization text you will fuck up!",
                                    MessageType.Warning);

            downLoadUrl = EditorGUILayout.TextField("Url to file", downLoadUrl);

            deleteFileWhenFinished = EditorGUILayout.Toggle("Delete file when finished", deleteFileWhenFinished);

            if (GUILayout.Button("Download and Parse"))
                GoogleSheetLoader.LoadLanguages(downLoadUrl,
                                                configurationFile.ConfigurationData.LanguagePackDirectory,
                                                deleteFileWhenFinished);
        }
    }
}