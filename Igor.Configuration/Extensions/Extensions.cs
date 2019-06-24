using System.Collections.Generic;
using System.Linq;

namespace Igor.Configuration {
	internal static class Extensions {
		internal static string[] SplitFirstN(this string input, char separator, int occurences) {
			List<string> ret = new List<string>();

			while (input.Contains(separator)) {
				int index = input.IndexOf(separator);
				string sub = input.Substring(0, index);
				input = input.Substring(index);
				ret.Add(sub);
				if (ret.Count == occurences) {
					ret.Add(input.Substring(1));
					break;
				}
			}
			return ret.ToArray();
		}
	}
}
