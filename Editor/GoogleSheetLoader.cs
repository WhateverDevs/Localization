using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WhateverDevs.Localization.Runtime;

namespace WhateverDevs.Localization.Editor
{
    /// <summary>
    ///     Class to load different Google sheets
    /// </summary>
    public class GoogleSheetsLoader : EditorWindow
    {
        private LocalizerConfiguration configuration;

        private string downLoadUrl = "";

        private bool deleteFileWhenFinished = true;

        private const string TemporalPath = "Temp/LocalizationFile.csv";

        [MenuItem("WhateverDevs/Localization/GoogleDrive")]
        public static void ShowWindow() => GetWindow(typeof(GoogleSheetsLoader));

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("This tool currently parses csv files, not the actual sheet.",
                                    MessageType.Warning);

            GUILayout.Label("CSV Parser", EditorStyles.boldLabel);

            if (configuration == null)
            {
                configuration = (LocalizerConfiguration)
                    EditorGUILayout.ObjectField(new GUIContent("Configuration"),
                                                configuration,
                                                typeof(LocalizerConfiguration),
                                                false);

                return;
            }

            EditorGUILayout.HelpBox("The file in drive must be shared publicly!", MessageType.Warning);
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

                if (File.Exists(TemporalPath)) File.Delete(TemporalPath);

                FileInfo file = DriveFileDownloader.DownloadFileFromURLToPath(downLoadUrl, TemporalPath);

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
            List<List<LanguagePair>> localizationMap = new List<List<LanguagePair>>();
            List<Dictionary<string, string>> gameParametersData = CsvReader.Read(csvData);

            int col = gameParametersData[0].Count;

            for (int i = 0; i < col - 1; ++i) localizationMap.Add(new List<LanguagePair>());

            for (int i = 0; i < gameParametersData.Count; ++i)
            {
                for (int j = 1; j < gameParametersData[i].Count; ++j)
                {
                    LanguagePair item = new LanguagePair
                                        {
                                            Key = gameParametersData[i].ElementAt(0).Value,
                                            Value = gameParametersData[i].ElementAt(j).Value
                                        };

                    localizationMap[j - 1].Add(item);
                }
            }

            string folderPath = "Assets/Resources/"
                              + configuration.ConfigurationData.LanguagePackDirectory;

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