using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures.Integration;
using WhateverDevs.Localization.Runtime;

namespace WhateverDevs.Localization.Editor
{
    /// <summary>
    /// Class to load different Google sheets.
    /// </summary>
    public class GoogleSheetLoader : Loggable<GoogleSheetLoader>
    {
        /// <summary>
        /// Temp path for the downloaded file.
        /// </summary>
        private const string TemporalPath = "Temp/LocalizationFile.csv";

        /// <summary>
        /// Separator to use with Google Sheets.
        /// </summary>
        private const string Separator = "\t";

        /// <summary>
        /// Load the languages from several Google Sheets.
        /// </summary>
        /// <param name="urls">Sheet urls.</param>
        /// <param name="outputDirectory">Folder in which to save the languages.</param>
        /// <param name="deleteFileWhenFinished">Delete the temporal files when finished?</param>
        public static void LoadLanguages(string[] urls, string outputDirectory, bool deleteFileWhenFinished = true)
        {
            try
            {
                List<string> tsvData = new();

                for (int i = 0; i < urls.Length; i++)
                {
                    string url = urls[i];

                    EditorUtility.DisplayProgressBar("Loading languages",
                                                     "Downloading file - " + url,
                                                     (i + 1f) / urls.Length);

                    string sheetUrl = url.Replace("edit?usp=sharing", "export?format=tsv");

                    if (File.Exists(TemporalPath)) File.Delete(TemporalPath);

                    FileInfo file = DriveFileDownloader.DownloadFileFromURLToPath(sheetUrl, TemporalPath);

                    tsvData.Add(File.ReadAllText(file.FullName));

                    if (deleteFileWhenFinished) file.Delete();
                }

                ParseLocalizationData(tsvData, outputDirectory);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// Parses several TSV formatted Sheets.
        /// </summary>
        /// <param name="tsvData">The Sheets in TSV format</param>
        /// <param name="directory">Directory in which to save the languages.</param>
        private static void ParseLocalizationData(IEnumerable<string> tsvData, string directory)
        {
            string folderPath = "Assets/Resources/" + directory;

            Utils.DeleteDirectory(folderPath);

            Directory.CreateDirectory(folderPath);

            foreach (string languageSet in tsvData)
            {
                List<Dictionary<string, string>> gameParametersData =
                    CsvReader.ReadColumnsFromCsv(languageSet, Separator);

                List<string> languages =
                    gameParametersData[0].Keys.Where(key => !key.IsNullEmptyOrWhiteSpace()).ToList();

                string keyName = languages[0];

                for (int i = 1; i < languages.Count; i++)
                {
                    string language = languages[i];

                    EditorUtility.DisplayProgressBar("Loading languages",
                                                     language,
                                                     (i + 1f) / languages.Count);

                    string filePath = folderPath + language + ".asset";

                    ScriptableLanguage asset = AssetDatabase.LoadAssetAtPath<ScriptableLanguage>(filePath);

                    if (asset != null)
                        StaticLogger.Info("Adding keys to existing asset for language " + language + ".");
                    else
                    {
                        StaticLogger.Info("Creating new asset for language " + language + ".");
                        asset = ScriptableObject.CreateInstance<ScriptableLanguage>();
                        AssetDatabase.CreateAsset(asset, filePath);
                    }

                    foreach (Dictionary<string, string> dictionary in gameParametersData)
                        if (dictionary.TryGetValue(keyName, out string key)
                         && !key.IsNullEmptyOrWhiteSpace())
                        {
                            if (!dictionary.TryGetValue(language, out string value) || value.IsNullEmptyOrWhiteSpace())
                            {
                                StaticLogger.Error("Key " + key + " has no value in language " + language + "!");
                                continue;
                            }

                            asset.Language[key] = value;
                        }

                    EditorUtility.SetDirty(asset);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}