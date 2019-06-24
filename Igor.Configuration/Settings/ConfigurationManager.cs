using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Igor.Configuration {
	public class ConfigurationManager<T> where T : IConfiguration, new() {
		#region Singleton Instance

		public static ConfigurationManager<T> Instance { get; private set; }

		public int SettingsInitFailure { get; private set; } = 0;

		public static bool Initialize(string configurationFilePath = "Settings/.config", bool forceLoadDefaults = false) {
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
				configFile = executablePath + configFilePath;
			}
			FileInfo file = new FileInfo(configFile);
			if (!file.Directory.Exists) {
				Directory.CreateDirectory(file.Directory.FullName);
			}
		}

		#endregion

		private static string executablePath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar;
		private string configFile { get; set; }

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
						SettingsParser<T>.ParseLine(ret, reader.ReadLine());
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
			if (!Directory.Exists(executablePath)) {
				Directory.CreateDirectory(executablePath);
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

		public int Save() {
			try {
				if (File.Exists(configFile)) {
					File.Delete(configFile);
				}
				File.WriteAllText(configFile, ToFileRep(CurrentSettings));
				return 0;
			}
			catch {
				return 3;
			}
		}

		private string ToFileRep(T currentSettings) {
			Type t = typeof(T);
			StringBuilder str = new StringBuilder();

			if (!string.IsNullOrWhiteSpace(currentSettings.ConfigurationHeader())) {
				str.Append("# ");
				str.Append(currentSettings.ConfigurationHeader());
				str.Append(Environment.NewLine);
			}

			foreach (PropertyInfo prop in t.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
				CommentAttribute comment = prop.GetCustomAttribute<CommentAttribute>();
				if (comment != null) {
					str.Append("# ");
					str.Append(comment.Comment);
					str.Append(Environment.NewLine);
				}
				str.Append($"{prop.PropertyType.Name}:{prop.Name}={prop.GetValue(currentSettings)}");
				str.Append(Environment.NewLine);
			}
			return str.ToString();
		}
	}
}
