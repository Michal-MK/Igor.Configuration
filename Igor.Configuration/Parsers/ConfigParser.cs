using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Igor.Configuration {
	internal static class ConfigParser<T> {
		private const string BOOL = "Boolean";
		private const string STRING = "String";
		private const string INT = "Int32";
		private const string BYTE = "Byte";
		private const string ARRAY = ">";

		internal static void ParseLine(T ret, string line, StreamReader reader) {
			if (line.StartsWith("#") || string.IsNullOrEmpty(line)) {
				return;
			}

			string[] split = line.SplitFirstN(':', 1);
			if (split.Length != 2) {
				throw new InvalidOperationException(line);
			}

			Type settingsType = typeof(T);
			if (split[0].EndsWith(ARRAY)) {
				int index = split[0].IndexOf('<') + 1;
				string arrayType = split[0].Substring(index, Math.Abs(index - split[0].LastIndexOf('>')));
				switch (arrayType) {
					case STRING: {
						ICollection<string> value = ParseStringList(reader);
						settingsType.GetProperty(split[1].Split('=')[0]).SetValue(ret, value);
						return;
					}
					case INT: {
						ICollection<int> value = ParseList(reader, s => int.Parse(s.TrimEnd(',')));
						settingsType.GetProperty(split[1].Split('=')[0]).SetValue(ret, value);
						return;
					}
					case BOOL: {
						ICollection<bool> value = ParseList(reader, s => bool.Parse(s.TrimEnd(',')));
						settingsType.GetProperty(split[1].Split('=')[0]).SetValue(ret, value);
						return;
					}
					case BYTE: {
						ICollection<byte> value = ParseList(reader, s => byte.Parse(s.TrimEnd(',')));
						settingsType.GetProperty(split[1].Split('=')[0]).SetValue(ret, value);
						return;
					}
				}
			}
			else {
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
					case BYTE: {
						byte value = ParseByte(split[1], out string propertyName);
						settingsType.GetProperty(propertyName).SetValue(ret, value);
						return;
					}

				}
			}
		}

		private static ICollection<string> ParseStringList(StreamReader reader) {
			ICollection<string> ret = new List<string>();
			string currentLine = reader.ReadLine();
			while (currentLine != "}") {
				string value = ParseString("=" + currentLine, reader, out _);
				ret.Add(value);
				currentLine = reader.ReadLine();
			}
			return ret;
		}

		private static List<U> ParseList<U>(StreamReader reader, Func<string, U> converterFunc) {
			List<U> lines = new List<U>();
			string curr = reader.ReadLine();
			while (curr != "}") {
				lines.Add(converterFunc(curr));
				curr = reader.ReadLine();
			}
			return lines;
		}

		private static string ParseString(string line, StreamReader reader, out string propertyName) {
			string[] split = line.Split('=');
			propertyName = split[0];
			char terminator = string.IsNullOrEmpty(propertyName) ? ',' : '"';
			if (split[1].StartsWith("\"")) {
				StringBuilder builder = new StringBuilder(split[1].Remove(0, 1) + Environment.NewLine);
				do {
					string nextLine = reader.ReadLine();
					builder.AppendLine(nextLine);
				}
				while (!Terminated(builder, terminator));
				int newLineQuote = Environment.NewLine.Length + 1;

				string result = builder.ToString();
				result = result.Replace("\\\"", "\"");

				result = result.Remove(result.Length - newLineQuote);
				if (terminator == ',')
					result = result.Remove(result.Length - 1);
				return result;
			}
			split[1] = split[1].Trim();
			if (terminator == ',')
				split[1] = split[1].Remove(split[1].Length - 1);

			return split[1];
		}

		private static bool Terminated(StringBuilder builder, char terminator) {
			int nlLength = Environment.NewLine.Length;
			if (builder[builder.Length - nlLength - 1] == terminator) {
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

		private static byte ParseByte(string line, out string propertyName) {
			string[] split = line.Split('=');
			propertyName = split[0];
			return byte.Parse(split[1]);
		}
	}
}
