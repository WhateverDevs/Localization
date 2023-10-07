using System.Linq;
using UnityEditor;
using UnityEditorInternal;
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
        /// Reorderable list to display the scene list.
        /// </summary>
        private ReorderableList reorderableList;

        /// <summary>
        /// Load the configs.
        /// </summary>
        private void OnEnable()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Localization", "Looking for configuration...", .5f);
                projectSettings = AssetManagementUtils.FindAssetsByType<LocalizerSettings>().First();
                LoadListEditor();
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

            reorderableList.DoLayoutList();

            if (GUILayout.Button("Paste into new element"))
                projectSettings.GoogleSheetsDownloadUrls.Add(EditorGUIUtility.systemCopyBuffer);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            deleteFileWhenFinished = EditorGUILayout.Toggle("Delete file when finished", deleteFileWhenFinished);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            if (GUILayout.Button("Download and Parse"))
                GoogleSheetLoader.LoadLanguages(projectSettings.GoogleSheetsDownloadUrls.ToArray(),
                                                projectSettings.LanguagePackDirectory,
                                                deleteFileWhenFinished);
        }

        /// <summary>
        /// Loads the editor for the Exercise List.
        /// </summary>
        private void LoadListEditor() =>
            reorderableList =
                new ReorderableList(projectSettings.GoogleSheetsDownloadUrls, typeof(SceneAsset))
                {
                    drawHeaderCallback = rect => EditorGUI.LabelField(rect, "URL list"),
                    drawElementCallback = (rect, index, _, _) =>
                                          {
                                              projectSettings.GoogleSheetsDownloadUrls[index] =
                                                  EditorGUI.TextField(rect,
                                                                      projectSettings.GoogleSheetsDownloadUrls
                                                                          [index]);

                                              EditorUtility.SetDirty(projectSettings);
                                          },
                    onAddCallback = _ => projectSettings.GoogleSheetsDownloadUrls.Add(null)
                };
    }
}