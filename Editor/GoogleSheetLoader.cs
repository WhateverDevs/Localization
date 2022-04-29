using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WhateverDevs.Core.Editor.Utils;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace WhateverDevs.Localization.Editor
{
    /// <summary>
    /// Default implementation of the google sheet loader.
    /// </summary>
    public class DefaultGoogleSheetLoader : GoogleSheetsLoader<LocalizerConfigurationFile, LocalizerConfiguration>
    {
        [MenuItem("WhateverDevs/Localization/GoogleDrive #&l")]
        public static void ShowWindow() => GetWindow(typeof(DefaultGoogleSheetLoader), false, "Drive Sheet Parser");
    }

    /// <summary>
    ///     Class to load different Google sheets
    /// </summary>
    public abstract class GoogleSheetsLoader<TLocalizerConfigurationFile, TLocalizerConfiguration> : EditorWindow
        where TLocalizerConfigurationFile : LocalizerConfigurationFile<TLocalizerConfiguration>, new()
        where TLocalizerConfiguration : LocalizerConfiguration, new()
    {
        private TLocalizerConfigurationFile configurationFile;

        private string downLoadUrl = "";

        private bool deleteFileWhenFinished = true;

        private const string TemporalPath = "Temp/LocalizationFile.csv";

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

            if (GUILayout.Button("Download and Parse")) LoadLanguages();
        }

        /// <summary>
        ///     Load Languages
        /// </summary>
        /// <returns>IEnumerator</returns>
        private void LoadLanguages()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Loading languages", "Downloading file...", .25f);

                string sheetUrl = downLoadUrl.Replace("edit?usp=sharing", "export?format=tsv");

                if (File.Exists(TemporalPath)) File.Delete(TemporalPath);

                FileInfo file = DriveFileDownloader.DownloadFileFromURLToPath(sheetUrl, TemporalPath);

                EditorUtility.DisplayProgressBar("Loading languages", "Parsing file...", .5f);

                ParseLocalizationData(File.ReadAllText(file.FullName));

                if (deleteFileWhenFinished) file.Delete();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        ///     Parses the CSV formatted Sheet
        /// </summary>
        /// <param name="csvData">The Sheet in CSV format</param>
        private void ParseLocalizationData(string csvData)
        {
            List<SerializableDictionary<string, string>> localizationMap =
                new List<SerializableDictionary<string, string>>();

            List<Dictionary<string, string>> gameParametersData =
                CsvReader.Read(csvData, configurationFile.ConfigurationData);

            int col = gameParametersData[0].Count;

            for (int i = 0; i < col - 1; ++i) localizationMap.Add(new SerializableDictionary<string, string>());

            for (int i = 0; i < gameParametersData.Count; ++i)
            {
                for (int j = 1; j < gameParametersData[i].Count; ++j)
                    localizationMap[j - 1][gameParametersData[i].ElementAt(0).Value] =
                        gameParametersData[i].ElementAt(j).Value;
            }

            string folderPath = "Assets/Resources/"
                              + configurationFile.ConfigurationData.LanguagePackDirectory;

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            for (int i = 0; i < col - 1; ++i)
            {
                ScriptableLanguage asset = CreateInstance<ScriptableLanguage>();

                AssetDatabase.CreateAsset(asset,
                                          folderPath
                                        + gameParametersData[0].ElementAt(i + 1).Key
                                        + ".asset");

                asset.Language = localizationMap[i];

                EditorUtility.SetDirty(asset);

                AssetDatabase.SaveAssets();

                AssetDatabase.Refresh();

                EditorUtility.FocusProjectWindow();

                Selection.activeObject = asset;
            }
        }
    }
}