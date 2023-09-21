using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace WhateverDevs.Localization.Editor
{
    /// <summary>
    /// Class to load different Google sheets.
    /// </summary>
    public class GoogleSheetLoader
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
        /// Load the languages from a Google Sheet.
        /// </summary>
        /// <param name="url">Sheet url.</param>
        /// <param name="outputDirectory">Folder in which to save the languages.</param>
        /// <param name="deleteFileWhenFinished">Delete the temporal files when finished?</param>
        public static void LoadLanguages(string url, string outputDirectory, bool deleteFileWhenFinished = true)
        {
            try
            {
                EditorUtility.DisplayProgressBar("Loading languages", "Downloading file...", .25f);

                string sheetUrl = url.Replace("edit?usp=sharing", "export?format=tsv");

                if (File.Exists(TemporalPath)) File.Delete(TemporalPath);

                FileInfo file = DriveFileDownloader.DownloadFileFromURLToPath(sheetUrl, TemporalPath);

                EditorUtility.DisplayProgressBar("Loading languages", "Parsing file...", .5f);

                ParseLocalizationData(File.ReadAllText(file.FullName), outputDirectory);

                if (deleteFileWhenFinished) file.Delete();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// Parses the TSV formatted Sheet.
        /// </summary>
        /// <param name="csvData">The Sheet in TSV format</param>
        /// <param name="directory">Directory in which to save the languages.</param>
        private static void ParseLocalizationData(string csvData, string directory)
        {
            List<SerializableDictionary<string, string>> localizationMap = new();

            List<Dictionary<string, string>> gameParametersData =
                CsvReader.Read(csvData, Separator);

            int col = gameParametersData[0].Count;

            for (int i = 0; i < col - 1; ++i) localizationMap.Add(new SerializableDictionary<string, string>());

            for (int i = 0; i < gameParametersData.Count; ++i)
            {
                for (int j = 1; j < gameParametersData[i].Count; ++j)
                    localizationMap[j - 1][gameParametersData[i].ElementAt(0).Value] =
                        gameParametersData[i].ElementAt(j).Value;
            }

            string folderPath = "Assets/Resources/" + directory;

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            for (int i = 0; i < col - 1; ++i)
            {
                if (gameParametersData[0].ElementAt(i + 1).Key.IsNullOrWhitespace()) continue;

                ScriptableLanguage asset = ScriptableObject.CreateInstance<ScriptableLanguage>();

                AssetDatabase.CreateAsset(asset,
                                          folderPath
                                        + gameParametersData[0].ElementAt(i + 1).Key
                                        + ".asset");

                asset.Language = localizationMap[i];

                EditorUtility.SetDirty(asset);

                AssetDatabase.SaveAssets();

                AssetDatabase.Refresh();
            }
        }
    }
}