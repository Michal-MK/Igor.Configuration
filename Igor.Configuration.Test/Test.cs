using System;
using System.IO;

namespace Igor.Configuration.Test {
	class Test {
		static void Main(string[] args) {
			ConfigurationManager<Configuration1>.Initialize(forceLoadDefaults: true);
			Configuration1 config = ConfigurationManager<Configuration1>.Instance.CurrentSettings;

			config.Help = "NOOOOOO";
			config.Run = false;
			config.OtherInfo = "None";

			ConfigurationManager<Configuration1>.Instance.Save();

			Console.WriteLine("SUCCESS");

			try {
				ConfigurationManager<Conf2>.Initialize(@"C:\config.cfg", true);
				Conf2 c2 = ConfigurationManager<Conf2>.Instance.CurrentSettings;
				Console.WriteLine("INVALID");
			}
			catch {
				Console.WriteLine("SUCCESS");
			}

			ConfigurationManager<Conf2>.Initialize(@"Test/conf.configuration", true);
			Conf2 c3 = ConfigurationManager<Conf2>.Instance.CurrentSettings;

		}
	}
}
