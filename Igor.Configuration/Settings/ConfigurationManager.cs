using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Igor.Configuration {
	public class ConfigurationManager<T> where T : IConfiguration, new() {
		#region Singleton Instance

		public static ConfigurationManager<T> Instance { get; private set; }

		public int SettingsInitFailure { get; private set; } = 0;

		public static bool Initialize(string configurationFilePath = "Settings/.config", bool forceLoadDefaults = false, string exePath = ".") {
			if (exePath == ".") {
				_executablePath = AppDomain.CurrentDomain.BaseDirectory;
			}
			else {
				_executablePath = exePath;
			}

			Instance = new ConfigurationManager<T>(configurationFilePath);

			if (forceLoadDefaults) {
				bool loaded = Instance.Load();
				if (!loaded) {
					loaded = Instance.Load();
					return loaded;
				}
				return loaded;
			}
			return Instance.Load();
		}

		private ConfigurationManager(string configFilePath) {
			if (Path.IsPathRooted(configFilePath)) {
				configFile = configFilePath;
			}
			else {
				configFile = Path.Combine(_executablePath, configFilePath);
			}
			FileInfo file = new FileInfo(configFile);
			if (!file.Directory.Exists) {
				Directory.CreateDirectory(file.Directory.FullName);
			}
		}

		#endregion

		internal static string _executablePath;
		private readonly string configFile;

		public T CurrentSettings { get; private set; }

		private void GenerateSettings() {
			if (File.Exists(configFile)) {
				File.Delete(configFile);
			}

			File.WriteAllText(configFile, ToFileRep(new T()));
		}

		private T ParseSettings() {
			if (!File.Exists(configFile)) {
				throw new IOException("No settings exist!");
			}

			T ret = new T();

			try {
				using (StreamReader reader = new StreamReader(configFile)) {
					while (!reader.EndOfStream) {
						ConfigParser<T>.ParseLine(ret, reader.ReadLine(), reader);
					}
				}
			}
			catch (Exception e) {
				Debugger.Break();
				throw e;
				/*Handled in call above*/
			}
			return ret;
		}

		private bool Load() {
			if (!Directory.Exists(_executablePath)) {
				Directory.CreateDirectory(_executablePath);
			}

			try {
				CurrentSettings = ParseSettings();
				return true;
			}
			catch {
				SettingsInitFailure = 1;
				GenerateSettings();
				return false;
			}
		}

		public static int Save(T currentSettings, string path) {
			try {
				if (File.Exists(path)) {
					File.Delete(path);
				}
				File.WriteAllText(path, ToFileRep(currentSettings));
				return 0;
			}
			catch {
				return 3;
			}
		}

		public int Save() => Save(CurrentSettings, configFile);

		private static string ToFileRep(T currentSettings) {
			Type t = typeof(T);
			StringBuilder str = new StringBuilder();

			if (!string.IsNullOrWhiteSpace(currentSettings.ConfigurationHeader())) {
				string header = currentSettings.ConfigurationHeader();

				if (!header.StartsWith("#")) {
					header = "# " + header;
				}
				str.AppendLine(header);
			}

			foreach (PropertyInfo prop in t.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
				CommentAttribute comment = prop.GetCustomAttribute<CommentAttribute>();
				if (comment != null) {
					if (comment.NewLine) {
						str.AppendLine();
					}
					str.AppendLine($"# {comment.Comment}");
				}
				if (prop.PropertyType.IsArray) {
					throw new InvalidOperationException("Arrays are not supported, use a List instead");
				}
				if (prop.PropertyType.GetInterface(typeof(IEnumerable).Name) != null && prop.PropertyType != typeof(string)) {
					if (prop.PropertyType.IsGenericType) {
						Type[] generics = prop.PropertyType.GetGenericArguments();
						str.Append(prop.PropertyType.Name.Remove(prop.PropertyType.Name.IndexOf("`")));
						str.Append("<" + string.Join<string>(",", generics.Select(tt => tt.Name)) + ">");
						str.AppendLine($":{prop.Name}={{");
						str.AppendLine(FormatCollection((IEnumerable)prop.GetValue(currentSettings)));
						str.AppendLine("}");
					}
				}
				else {
					str.AppendLine($"{prop.PropertyType.Name}:{prop.Name}={prop.GetValue(currentSettings)}");
				}
			}
			return str.ToString();
		}

		private static string FormatCollection(IEnumerable col) {
			StringBuilder builder = new StringBuilder();
			if (col == null) {
				return "";
			}
			IEnumerator enumerator = col.GetEnumerator();
			if (!enumerator.MoveNext()) {
				return "";
			}
			if (enumerator.Current is string s && s.Split('\n').Length > 1) {
				builder.Append("\"" + s + "\"");
			}
			else {
				builder.Append(enumerator.Current.ToString());
			}
			string SEP = "," + Environment.NewLine;
			while (enumerator.MoveNext()) {
				builder.Append(SEP);
				if (enumerator.Current is string ss && ss.Split('\n').Length > 1) {
					builder.Append("\"" + ss + "\"");
				}
				else {
					builder.Append(enumerator.Current.ToString());
				}
			}
			return builder.ToString();
		}
	}
}
