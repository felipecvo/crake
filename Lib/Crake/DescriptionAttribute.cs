using System;

namespace Crake
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class DescriptionAttribute : Attribute
	{
		public DescriptionAttribute (string text)
		{
			Text = text;
		}

		public string Text { get; private set; }
	}
}
