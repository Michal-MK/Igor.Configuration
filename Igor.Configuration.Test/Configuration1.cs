using System.IO;

namespace Igor.Configuration.Test {
	public class Configuration1 : IConfiguration {

		[Comment("Help to get")]
		public string Help { get; set; } = "Test";

		public string OtherInfo { get; set; } = "Some info about something";

		[Comment("Include this crap inside of whatever that it")]
		public bool IncludeMe { get; set; } = false;

		public string IncludePath { get; set; } = Directory.GetCurrentDirectory();

		[Comment("RUN FOREST")]
		public bool Run { get; set; } = true;

		public int TestInt { get; set; } = 5181235;

		[Comment("An Integer ith comment")]
		public int Value { get; set; } = 124;

		public string ConfigurationHeader() {
			return "";
		}
	}
}
