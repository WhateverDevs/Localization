using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace WhateverDevs.Localization.Editor
{
    /// <summary>
    ///     GoogleSheetsLoader from a SpreadSheet to a List of Localizer.Pair
    /// </summary>
    public class LocalSheetLoader : EditorWindow
    {
        /// <summary>
        ///     LocalizationMap
        /// </summary>
        private static List<SerializableDictionary<string, string>> localizationMap;

        private static readonly Dictionary<string, string> LoadedSheet = new Dictionary<string, string>();

        private string fileToLoad = "/Data/Language/LanguageExcel.csv";

        private string destinationPath = "Assets/Resources/Languages/";

        [MenuItem("WhateverDevs/Localization/LocalFile")]
        public static void ShowWindow() => GetWindow(typeof(LocalSheetLoader));

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("This is obsolete, we should probably review all the code before using.",
                                    MessageType.Error);

            GUILayout.Label("CSV Parser", EditorStyles.boldLabel);
            fileToLoad = EditorGUILayout.TextField("Csv file to load", fileToLoad);
            destinationPath = EditorGUILayout.TextField("Destination file to load", destinationPath);
            if (GUILayout.Button("Parse")) ParseCsvToScriptableLanguages();
        }

        /// <summary>
        ///     Parse CSV to scriptable
        /// </summary>
        private void ParseCsvToScriptableLanguages()
        {
            localizationMap = new List<SerializableDictionary<string, string>>();

            string allText = File.ReadAllText(Application.dataPath + fileToLoad);

            ParseLocalizationData(allText);
        }

        /// <summary>
        ///     Parses the CSV formatted Sheet
        /// </summary>
        /// <param name="csvData">The Sheet in CSV format</param>
        private void ParseLocalizationData(string csvData)
        {
            List<Dictionary<string, string>> gameParametersData = CsvReader.Read(csvData);

            int col = gameParametersData[0].Count;

            for (int i = 0; i < col - 1; ++i) localizationMap.Add(new SerializableDictionary<string, string>());

            for (int i = 0; i < gameParametersData.Count; ++i)
            {
                for (int j = 1; j < gameParametersData[i].Count; ++j)
                    localizationMap[j - 1][gameParametersData[i].ElementAt(0).Value] =
                        gameParametersData[i].ElementAt(j).Value;
            }

            for (int i = 0; i < col - 1; ++i)
            {
                ScriptableLanguage asset = CreateInstance<ScriptableLanguage>();

                AssetDatabase.CreateAsset(asset,
                                          destinationPath + gameParametersData[0].ElementAt(i + 1).Key + ".asset");

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