using System;

namespace Crake.Parser
{
	public class PlainDesc : IParsedObject
	{
		#region IParsedObject implementation
		public IParsedObject Parent {
			get;
			private set;
		}
		
		#endregion

		public string Text { get; private set; }
		
		public PlainDesc (string text, IParsedObject parent)
		{
			Text = text;
			Parent = parent;
		}
	}
}
