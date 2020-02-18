using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace WhateverDevs.Localization
{
    /// <summary>
    /// GoogleSheetsLoader from a SpreadSheet to a List of Localizer.Pair
    /// </summary>
    [ExecuteInEditMode]
    public class LocalSheetLoader : EditorWindow
    {
        /// <summary>
        /// LocalizationMap
        /// </summary>
        private static List<List<LanguagePair>> localizationMap;
        
        private static Dictionary<string, string> _loadedSheet = new Dictionary<string, string>();
        
        
        /// <summary>
        /// Load Languages
        /// </summary>
        /// <returns>IEnumerator</returns>
        [MenuItem("Tools/Localization/LocalFile")]
        public static void LoadLanguages()
        {
            localizationMap = new List<List<LanguagePair>>();

            float progress = 0.0f;

            EditorUtility.DisplayProgressBar("Loading languages", "Loading...", progress);

            for (int i = 0; i < (int) Localizer.eLanguage.COUNT; ++i)
            {
                localizationMap.Add(new List<LanguagePair>());
            }

            string allText = System.IO.File.ReadAllText(Application.dataPath+ "/Scripts/ExcelTest.csv");
            
            ParseLocalizationData(allText);
            
        }

        /// <summary>
        /// Parses the downloaded CSV formatted Google Sheet
        /// </summary>
        /// <param name="csvData">The Google Sheet in CSV format</param>
        private static void ParseLocalizationData(string csvData)
        {
            List<Dictionary<string, string>> gameParametersData = CsvReader.Read(csvData);

            LanguagePair[] item = new LanguagePair[(int) Localizer.eLanguage.COUNT];

            for (int i = 0; i < gameParametersData.Count; ++i)
            {
                for (int j = 0; j < gameParametersData[i].Count; ++j)
                {
                    for (int z = 0; z < (int) Localizer.eLanguage.COUNT; ++z)
                    {
                        if (gameParametersData[i].ElementAt(j).Key == "KEY")
                        {
                            item = new LanguagePair[(int) Localizer.eLanguage.COUNT];

                            for (int x = 0; x < item.Length; ++x)
                            {
                                item[x] = new LanguagePair();
                                item[x].Key = gameParametersData[i].ElementAt(j).Value;
                            }
                        }
                        else if (gameParametersData[i].ElementAt(j).Key == "SPA" && z == 0)
                        {
                            item[z].Value = gameParametersData[i].ElementAt(j).Value;

                            localizationMap[z].Add(item[z]);
                        }
                        else if (gameParametersData[i].ElementAt(j).Key == "ENG" && z == 1)
                        {
                            item[z].Value = gameParametersData[i].ElementAt(j).Value;

                            localizationMap[z].Add(item[z]);
                        }

                        else if (gameParametersData[i].ElementAt(j).Key == "FRE" && z == 2)
                        {
                            item[z].Value = gameParametersData[i].ElementAt(j).Value;
    
                            localizationMap[z].Add(item[z]);
                        }
                        else if (gameParametersData[i].ElementAt(j).Key == "GER" && z == 3)
                        {
                            item[z].Value = gameParametersData[i].ElementAt(j).Value;
    
                            localizationMap[z].Add(item[z]);
                        }
                    }
                }
            }

            for (int i = 0; i < (int) Localizer.eLanguage.COUNT; ++i)
            {
                ScriptableLanguage asset = ScriptableObject.CreateInstance<ScriptableLanguage>();

                AssetDatabase.CreateAsset(asset,
                    "Assets/Resources/ScriptableResources/Languages/" + ((Localizer.eLanguage) i).ToString() + ".asset");

                asset.Language = localizationMap[i];

                EditorUtility.SetDirty(asset);

                AssetDatabase.SaveAssets();

                AssetDatabase.Refresh();

                EditorUtility.FocusProjectWindow();

                Selection.activeObject = asset;
            }
        }

        /// <summary>
        /// Stores the loaded parameter configuration locally
        /// </summary>
        /// <param name="paramName">The configuration parameter name</param>
        /// <param name="paramValue">The configuration parameter value</param>
        private void ApplyDataFromRow(string paramName, string paramValue)
        {
            if (_loadedSheet.ContainsKey(paramName))
            {
                _loadedSheet[paramName] = paramValue;
            }
            else
            {
                _loadedSheet.Add(paramName, paramValue);
            }
        }
    }
}

