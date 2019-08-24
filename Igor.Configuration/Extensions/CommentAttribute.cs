using System;

namespace Igor.Configuration {
	public class CommentAttribute : Attribute {

		internal string Comment { get; set; }
		internal bool NewLine { get; set; }

		public CommentAttribute(string comment = "", bool offsetWithNewLine = true) {
			Comment = string.IsNullOrWhiteSpace(comment) ? "" : comment;
			NewLine = offsetWithNewLine;
		}
	}
}
