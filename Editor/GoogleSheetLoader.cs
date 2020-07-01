using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WhateverDevs.Localization;
using WhateverDevs.Localizer.Runtime;

/// <summary>
///     Class to load different Google sheets
/// </summary>
[ExecuteInEditMode]
public class GoogleSheetsLoader : EditorWindow
{
    #region Localization

    /// <summary>
    ///     LocalizationMap
    /// </summary>
    private static List<List<LanguagePair>> localizationMap;

    private LocalizerConfigurationData configurationData;
    
    private static string GoogleDriveFormat = "";
        
    private static string LocalizationDriveID = "";
        
    private static string[] LocalizationSheetID = {"0"};

    #endregion

    private static readonly Dictionary<string, string> _loadedSheet = new Dictionary<string, string>();

    /// <summary>
    ///     ServerCall www
    /// </summary>
    private static WWW serverCall;

    /// <summary>
    ///     Load Languages
    /// </summary>
    /// <returns>IEnumerator</returns>
    [MenuItem("Tools/Localization/GoogleDrive")]
    public static void LoadLanguages()
    {
        localizationMap = new List<List<LanguagePair>>();

        float progress = 0.0f;

        EditorUtility.DisplayProgressBar("Loading languages", "Loading...", progress);

        //for (int i = 0; i < (int) Localizer.eLanguage.COUNT; ++i) localizationMap.Add(new List<LanguagePair>());

        for (int i = 0; i < LocalizationSheetID.Length; ++i)
        {
            progress = i / (float) LocalizationSheetID.Length;

            LoadGoogleSheet(LocalizationDriveID, LocalizationSheetID[i]);

            ParseLocalizationData(serverCall.text);

            EditorUtility.DisplayProgressBar("Loading languages", "Loading...", progress);
        }

        EditorUtility.DisplayProgressBar("Loading languages", "Loading...", 1.0f);
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    ///     Coroutine which downloads the Google Sheet in CSV format and parses it on success
    /// </summary>
    /// <param name="docId">The document's unique identifier on Google Drive</param>
    /// <param name="sheetId">The sheet's unique identifier in the Google Sheets document</param>
    public static void LoadGoogleSheet(string docId, string sheetId)
    {
        _loadedSheet.Clear();

        string downloadUrl = string.Format(GoogleDriveFormat, docId, sheetId);

        serverCall = new WWW(downloadUrl);

        while (serverCall.isDone == false)
        {
        }

        if (!string.IsNullOrEmpty(serverCall.error)) Debug.LogError("Unable to fetch CSV data from Google");
    }

    /// <summary>
    ///     Parses the downloaded CSV formatted Google Sheet
    /// </summary>
    /// <param name="csvData">The Google Sheet in CSV format</param>
    private static void ParseLocalizationData(string csvData)
    {
        if (string.IsNullOrEmpty(csvData)) return;

        List<Dictionary<string, string>> gameParametersData = CsvReader.Read(csvData);

        /*LanguagePair[] item = new LanguagePair[(int) Localizer.eLanguage.COUNT];

        for (int i = 0; i < gameParametersData.Count; ++i)
        {
            for (int j = 0; j < gameParametersData[i].Count; ++j)
            {
                for (int z = 0; z < (int) Localizer.eLanguage.COUNT; ++z)
                    if (gameParametersData[i].ElementAt(j).Key == "KEY")
                    {
                        item = new LanguagePair[(int) Localizer.eLanguage.COUNT];

                        for (int x = 0; x < item.Length; ++x)
                        {
                            item[x] = new LanguagePair();
                            item[x].Key = gameParametersData[i].ElementAt(j).Value;
                        }
                    }
                    else if (gameParametersData[i].ElementAt(j).Key == ((Localizer.eLanguage) z).ToString())
                    {
                        item[z].Value = gameParametersData[i].ElementAt(j).Value;

                        localizationMap[z].Add(item[z]);
                    }
            }
        }

        for (int i = 0; i < (int) Localizer.eLanguage.COUNT; ++i)
        {
            ScriptableLanguage asset = CreateInstance<ScriptableLanguage>();

            AssetDatabase.CreateAsset(asset,
                                      "Assets/Resources/ScriptableResources/Languages/"
                                    + (Localizer.eLanguage) i
                                    + ".asset");

            asset.Language = localizationMap[i];

            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }*/
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