﻿using System;

namespace Igor.Configuration.Test {
	class Test {
		static void Main(string[] args) {
			ConfigurationManager<Configuration1>.Initialize(forceLoadDefaults: true);
			Configuration1 config = ConfigurationManager<Configuration1>.Instance.CurrentSettings;

			config.Help = "NOOOOOO";
			config.Run = false;
			config.OtherInfo = "None";
			for (int i = 0; i < config.Ints.Count; i++) {
				config.Ints[i] += 10;
			}
			for (int i = 0; i < config.Bytes.Count; i++) {
				config.Bytes[i] += 10;
			}


			ConfigurationManager<Configuration1>.Instance.Save();

			Console.WriteLine("SUCCESS");

			try {
				ConfigurationManager<Conf2>.Initialize(@"C:\config.cfg", forceLoadDefaults: true);
				Conf2 c2 = ConfigurationManager<Conf2>.Instance.CurrentSettings;
				Console.WriteLine("INVALID");
			}
			catch {
				Console.WriteLine("SUCCESS");
			}

			ConfigurationManager<Conf2>.Initialize(@"Test/conf.configuration", forceLoadDefaults: true);
			Conf2 c3 = ConfigurationManager<Conf2>.Instance.CurrentSettings;
		}
	}
}
