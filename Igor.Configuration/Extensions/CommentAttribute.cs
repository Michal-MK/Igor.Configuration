using System;

namespace Igor.Configuration {
	public class CommentAttribute : Attribute {

		internal string Comment { get; set; }

		public CommentAttribute(string comment = "") {
			Comment = comment;
		}
	}
}
