namespace IniFile
{

    public class IniFile
    {
        private readonly Dictionary<string, Dictionary<string, string>> _data
            = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        public IniFile(string path)
        {
            Load(path);
        }

        private void Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("INI file not found", path);

            string currentSection = "Default";
            _data[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var rawLine in File.ReadAllLines(path))
            {
                var line = rawLine.Trim();

                // 空行・コメント
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";"))
                    continue;

                // セクション
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line.Substring(1, line.Length - 2).Trim();
                    if (!_data.ContainsKey(currentSection))
                        _data[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    continue;
                }

                // key=value
                var parts = line.Split('=', 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                // 行末コメント除去
                int commentPos = value.IndexOfAny(new[] { ';', '#' });
                if (commentPos >= 0)
                    value = value.Substring(0, commentPos).Trim();

                _data[currentSection][key] = value;
            }
        }

        public string GetString(string section, string key, string defaultValue = "")
        {
            if (_data.TryGetValue(section, out var sec) &&
                sec.TryGetValue(key, out var value))
            {
                return value;
            }
            return defaultValue;
        }

        private string? GetRawString(string section, string key)
        {
            if (_data.TryGetValue(section, out var sec) &&
                sec.TryGetValue(key, out var value))
            {
                return value;
            }
            return null;
        }

        public int GetInt(string section, string key, int defaultValue = 0)
        {
            var value = GetRawString(section, key);
            return int.TryParse(value, out var result) ? result : defaultValue;
        }

        public bool GetBool(string section, string key, bool defaultValue = false)
        {
            var value = GetRawString(section, key);
            return bool.TryParse(value, out var result) ? result : defaultValue;
        }
    }
}
