using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using WhateverDevs.Localization;

/// <summary>
/// To support a different format change the ParseSheetData method
/// </summary>
[ExecuteInEditMode]
public class GoogleSheetsLoader : EditorWindow
{
    /// <summary>
    /// Specific end-point for downloading a Google Sheet in CSV format
    /// </summary>
    private static string GoogleDriveFormat = "http://docs.google.com/feeds/download/spreadsheets/Export?key={0}&exportFormat=csv&gid={1}";


    #region Localization

    /// <summary>
    /// LocalizationMap
    /// </summary>
    private static List<List<LanguagePair>> localizationMap;

    // The unique identifier of the google drive file we are using
    private static string LocalizationDriveID = "10SoOgLwD7ga0iy4bFVFhfw_RCPPugFTW9hM9QBAKy3Q";

    // Ids de las paginas que vamos a cargar en el proyecto para los idiomas
    private static string[] LocalizationSheetID = { "0"};

    #endregion


    private static Dictionary<string, string> _loadedSheet = new Dictionary<string, string>();

    /// <summary>
    /// ServerCall www
    /// </summary>
    private static WWW serverCall;

    /// <summary>
    /// Load Languages
    /// </summary>
    /// <returns>IEnumerator</returns>
    [MenuItem("Tools/Localization/GoogleDrive")]
    public static void LoadLanguages()
    {
        localizationMap = new List<List<LanguagePair>>();

        float progress = 0.0f;

        EditorUtility.DisplayProgressBar("Cargando idiomas", "Cargando...", progress);

        for (int i = 0; i < (int)Localizer.eLanguage.COUNT; ++i)
        {
            localizationMap.Add(new List<LanguagePair>());
        }

        for (int i = 0; i < LocalizationSheetID.Length; ++i)
        {
            progress = i / (float)LocalizationSheetID.Length;

            LoadGoogleSheet(LocalizationDriveID, LocalizationSheetID[i]);

            ParsheLocalizationData(serverCall.text);

            EditorUtility.DisplayProgressBar("Cargando idiomas", "Cargando...", progress);
        }

        EditorUtility.DisplayProgressBar("Cargando idiomas", "Cargando...", 1.0f);
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// Coroutine which downloads the Google Sheet in CSV format and parses it on success
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

        if (!string.IsNullOrEmpty(serverCall.error))
        {
            Debug.LogError("Unable to fetch CSV data from Google");
        }
    }

    /// <summary>
    /// Parses the downloaded CSV formatted Google Sheet
    /// </summary>
    /// <param name="csvData">The Google Sheet in CSV format</param>
    private static void ParsheLocalizationData(string csvData)
    {
        if (string.IsNullOrEmpty(csvData))
        {
            return;
        }

        List<Dictionary<string, string>> gameParametersData = CsvReader.Read(csvData);

        LanguagePair[] item = new LanguagePair[(int)Localizer.eLanguage.COUNT];

        for (int i = 0; i < gameParametersData.Count; ++i)
        {
            for (int j = 0; j < gameParametersData[i].Count; ++j)
            {
                for (int z = 0; z < (int)Localizer.eLanguage.COUNT; ++z)
                {
                    if (gameParametersData[i].ElementAt(j).Key == "KEY")
                    {
                        item = new LanguagePair[(int)Localizer.eLanguage.COUNT];

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
                    /*else if (gameParametersData[i].ElementAt(j).Key == "FRE" && z == 2)
                    {
                        item[z].Value = gameParametersData[i].ElementAt(j).Value;

                        localizationMap[z].Add(item[z]);
                    }
                    else if (gameParametersData[i].ElementAt(j).Key == "GER" && z == 3)
                    {
                        item[z].Value = gameParametersData[i].ElementAt(j).Value;

                        localizationMap[z].Add(item[z]);
                    }*/
                }
            }
        }

        for (int i = 0; i < (int)Localizer.eLanguage.COUNT; ++i)
        {
            ScriptableLanguage asset = ScriptableObject.CreateInstance<ScriptableLanguage>();

            AssetDatabase.CreateAsset(asset, "Assets/Resources/ScriptableResources/Languages/" + ((Localizer.eLanguage)i).ToString() + ".asset");
            
            //asset.Language = localizationMap[i];

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

