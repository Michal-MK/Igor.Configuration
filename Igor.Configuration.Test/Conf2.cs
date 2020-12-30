using System;

namespace Igor.Configuration.Test {
	internal class Conf2 : IConfiguration {
		public string ConfigurationHeader() {
			return "#Empty configuration" + Environment.NewLine + " #Containing a new line" + Environment.NewLine + "...and another one";
		}

		public bool Set { get; set; }

		public bool Go { get; set; } = true;

		public bool Halt { get; set; } = false;

		public string Init { get; set; }

		public string Help { get; set; } = "";

		public string Other { get; set; } = "A";

		private string NotInc { get; set; } = "HELLO";
	}
}