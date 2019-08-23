using System;
using System.IO;
using System.Text;

namespace Igor.Configuration {
	internal static class SettingsParser<T> {
		private const string BOOL = "Boolean";
		private const string STRING = "String";
		private const string INT = "Int32";


		internal static void ParseLine(T ret, string line, StreamReader reader) {
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
					string value = ParseString(split[1], reader, out string propertyName);
					settingsType.GetProperty(propertyName).SetValue(ret, value);
					return;
				}
				case INT: {
					int value = ParseInt(split[1], out string propertyName);
					settingsType.GetProperty(propertyName).SetValue(ret, value);
					return;
				}
			}
		}

		private static string ParseString(string line, StreamReader reader, out string propertyName) {
			string[] split = line.Split('=');
			propertyName = split[0];
			if (split[1].StartsWith("\"")) {
				StringBuilder builder = new StringBuilder(split[1].Remove(0, 1) + Environment.NewLine);
				do {
					string nextLine = reader.ReadLine();
					builder.AppendLine(nextLine);
				}
				while (!Terminated(builder));
				int newLineQuote = Environment.NewLine.Length + 1;

				string result = builder.ToString();
				result = result.Replace("\\\"", "\"");

				result = result.Remove(result.Length - newLineQuote, newLineQuote);
				return result;
			}
			return split[1].Trim();
		}

		private static bool Terminated(StringBuilder builder) {
			int nlLength = Environment.NewLine.Length;
			if (builder[builder.Length - nlLength - 1] == '"') {
				return builder[builder.Length - nlLength - 2] != '\\';
			}
			return false;
		}

		private static bool ParseBool(string line, out string propertyName) {
			string[] split = line.Split('=');
			propertyName = split[0];
			return split[1] == "True";
		}

		private static int ParseInt(string line, out string propertyName) {
			string[] split = line.Split('=');
			propertyName = split[0];
			return int.Parse(split[1]);
		}
	}
}
