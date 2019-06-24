using System;

namespace Igor.Configuration {
	public static class SettingsParser<T> {
		private const string BOOL = "Boolean";
		private const string STRING = "String";


		public static void ParseLine(T ret, string line) {
			if (line.StartsWith("#") || string.IsNullOrEmpty(line)) {
				return;
			}

			string[] split = line.SplitFirstN(':', 1);
			if (split.Length != 2) {
				throw new InvalidOperationException(line);
			}

			Type settingsType = typeof(T);

			switch (split[0]) {
				case BOOL: {
					bool value = ParseBool(split[1], out string propertyName);
					settingsType.GetProperty(propertyName).SetValue(ret, value);
					return;
				}
				case STRING: {
					string value = ParseString(split[1], out string propertyName);
					settingsType.GetProperty(propertyName).SetValue(ret, value);
					return;
				}
			}
		}

		private static string ParseString(string line, out string propertyName) {
			string[] split = line.Split('=');
			propertyName = split[0];
			return split[1].Trim();
		}

		private static bool ParseBool(string line, out string propertyName) {
			string[] split = line.Split('=');
			propertyName = split[0];
			return split[1] == "True";
		}
	}
}
