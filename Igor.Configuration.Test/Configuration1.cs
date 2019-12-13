using System.Collections.Generic;
using System.IO;

namespace Igor.Configuration.Test {
	public class Configuration1 : IConfiguration {

		[Comment("Help to get")]
		public string Help { get; set; } = "Test";

		public string OtherInfo { get; set; } = "Some info about something=";

		[Comment("Include this crap inside of whatever that it")]
		public bool IncludeMe { get; set; } = false;

		public string IncludePath { get; set; } = Directory.GetCurrentDirectory();

		[Comment("RUN FOREST")]
		public bool Run { get; set; } = true;

		public int TestInt { get; set; } = 5181235;

		[Comment("An Integer ith comment")]
		public int Value { get; set; } = 124;

		[Comment("List of ints")]
		public List<int> Ints { get; set; } = new List<int> { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

		[Comment("List of bytes")]
		public List<byte> Bytes { get; set; } = new List<byte> { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };


		public List<string> Strings { get; set; } = new List<string> { "Teset", "Testings vwfew", "awvlrtpoqwlksdmvlav", "wevirtnpolsnzxc", "ribeprbaneklwvddd", "vwevmkknlffkm", "qwertyuiopasdfghjklzxcvbnm", "mveopasdvklerbnaduficmiosd", "1111111111111111" };
		public string ConfigurationHeader() {
			return "";
		}
	}
}
