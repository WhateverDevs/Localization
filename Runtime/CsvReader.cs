using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
///     a Simple CSV reader based on this:
///     https://github.com/tikonen/blog/tree/master/csvreader
///     Altered it from reading a TextAsset into parsing a CSV string
/// </summary>
public class CsvReader
{
    private static readonly string SPLIT_RE = @";(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    private static readonly string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    private static readonly char[] TRIM_CHARS = {'\"'};

    public static List<Dictionary<string, string>> Read(string file)
    {
        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

        string[] lines = Regex.Split(file, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        string[] header = Regex.Split(lines[0], SPLIT_RE);

        for (int i = 1; i < lines.Length; ++i)
        {
            string[] values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            Dictionary<string, string> entry = new Dictionary<string, string>();

            for (int j = 0; j < header.Length && j < values.Length; ++j)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                string finalvalue = value;
                entry[header[j]] = finalvalue;
            }

            list.Add(entry);
        }

        return list;
    }
}