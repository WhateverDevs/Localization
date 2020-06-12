using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WhateverDevs.Localization
{
    /// <summary>
    ///     GoogleSheetsLoader from a SpreadSheet to a List of Localizer.Pair
    /// </summary>
    [ExecuteInEditMode]
    public class LocalSheetLoader : EditorWindow
    {
        /// <summary>
        ///     LocalizationMap
        /// </summary>
        private static List<List<LanguagePair>> localizationMap;

        private static readonly Dictionary<string, string> _loadedSheet = new Dictionary<string, string>();

        private string fileToLoad = "/ExcelTest.csv";

        private string destinationPath = "Assets/Resources/Languages/";
        
        private const string key = "Key";

        [MenuItem("Tools/Localization/LocalFile")]
        public static void ShowWindow() => GetWindow(typeof(LocalSheetLoader));

        private void OnGUI()
        {
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
            localizationMap = new List<List<LanguagePair>>();

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
            
            for (int i = 0; i < col-1; ++i) localizationMap.Add(new List<LanguagePair>());

            LanguagePair item;

            for (int i = 0; i < gameParametersData.Count; ++i)
            {
                for (int j = 1; j < gameParametersData[i].Count; ++j)
                {
                    item = new LanguagePair();
                    item.Key = gameParametersData[i].ElementAt(0).Value;
                    item.Value = gameParametersData[i].ElementAt(j).Value;
                    localizationMap[j-1].Add(item);
                }
            }

            for (int i = 0; i < col-1; ++i)
            {
                ScriptableLanguage asset = CreateInstance<ScriptableLanguage>();

                AssetDatabase.CreateAsset(asset,destinationPath + gameParametersData[0].ElementAt(i+1).Key + ".asset");

                asset.Language = localizationMap[i];

                EditorUtility.SetDirty(asset);

                AssetDatabase.SaveAssets();

                AssetDatabase.Refresh();

                EditorUtility.FocusProjectWindow();

                Selection.activeObject = asset;
            }
        }

        /// <summary>
        ///     Stores the loaded parameter configuration locally
        /// </summary>
        /// <param name="paramName">The configuration parameter name</param>
        /// <param name="paramValue">The configuration parameter value</param>
        private void ApplyDataFromRow(string paramName, string paramValue)
        {
            if (_loadedSheet.ContainsKey(paramName))
                _loadedSheet[paramName] = paramValue;
            else
                _loadedSheet.Add(paramName, paramValue);
        }
    }
}