using System;

namespace Unahi.CRake {
	public class DescriptionAttribute : Attribute {
		public DescriptionAttribute(string text) {
			Text = text;
		}

		public string Text { get; private set; }
	}
}