using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WhateverDevs.Localization.Runtime
{
    /// <summary>
    ///     a Simple CSV reader based on this:
    ///     https://github.com/tikonen/blog/tree/master/csvreader
    ///     Altered it from reading a TextAsset into parsing a CSV string
    /// </summary>
    public class CsvReader
    {
        private const string LineSplitRe = @"\r\n|\n\r|\n|\r";
        private static readonly char[] TrimChars = {'\"'};

        public static List<Dictionary<string, string>> Read(string file, LocalizerConfigurationData configuration)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

            string[] lines = Regex.Split(file, LineSplitRe);

            if (lines.Length <= 1) return list;

            string[] header = Regex.Split(lines[0], configuration.Separator);

            for (int i = 1; i < lines.Length; ++i)
            {
                string[] values = Regex.Split(lines[i], configuration.Separator);
                if (values.Length == 0 || values[0] == "") continue;

                Dictionary<string, string> entry = new Dictionary<string, string>();

                for (int j = 0; j < header.Length && j < values.Length; ++j)
                {
                    string value = values[j];
                    value = value.TrimStart(TrimChars).TrimEnd(TrimChars).Replace("\\", "");
                    string finalValue = value;
                    entry[header[j]] = finalValue;
                }

                list.Add(entry);
            }

            return list;
        }
    }
}