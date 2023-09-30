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
        DefaultGoogleSheetLoaderWindow : GoogleSheetsLoaderWindow
    {
        /// <summary>
        /// Show the window.
        /// </summary>
        [MenuItem("WhateverDevs/Localization/GoogleDrive #&l")]
        public static void ShowWindow() =>
            GetWindow(typeof(DefaultGoogleSheetLoaderWindow), false, "Drive Sheet Parser");
    }

    /// <summary>
    /// Window to load different Google sheets.
    /// </summary>
    public abstract class GoogleSheetsLoaderWindow : EditorWindow
    {
        /// <summary>
        /// Reference to the localizer project settings.
        /// </summary>
        private LocalizerSettings projectSettings;

        /// <summary>
        /// Delete the file after downloading?
        /// </summary>
        private bool deleteFileWhenFinished = true;

        /// <summary>
        /// Load the configs.
        /// </summary>
        private void OnEnable()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Localization", "Looking for configuration...", .5f);
                projectSettings = AssetManagementUtils.FindAssetsByType<LocalizerSettings>().First();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// Paint the window.
        /// </summary>
        private void OnGUI()
        {
            if (projectSettings == null)
            {
                projectSettings = (LocalizerSettings)EditorGUILayout.ObjectField(new GUIContent("Configuration"),
                    projectSettings,
                    typeof(LocalizerSettings),
                    false);

                return;
            }

            EditorGUILayout.HelpBox("The file in drive must be shared publicly!", MessageType.Warning);

            EditorGUILayout.HelpBox("This tool downloads the file as a tsv with tab separators. "
                                  + "If you have tabs inside your localization text you will fuck up!",
                                    MessageType.Warning);

            EditorGUILayout.BeginHorizontal();

            {
                projectSettings.GoogleSheetsDownloadUrl =
                    EditorGUILayout.TextField("Url to file", projectSettings.GoogleSheetsDownloadUrl);

                if (GUILayout.Button("Paste"))
                    projectSettings.GoogleSheetsDownloadUrl = EditorGUIUtility.systemCopyBuffer;
            }

            EditorGUILayout.EndHorizontal();

            deleteFileWhenFinished = EditorGUILayout.Toggle("Delete file when finished", deleteFileWhenFinished);

            if (GUILayout.Button("Download and Parse"))
                GoogleSheetLoader.LoadLanguages(projectSettings.GoogleSheetsDownloadUrl,
                                                projectSettings.LanguagePackDirectory,
                                                deleteFileWhenFinished);
        }
    }
}