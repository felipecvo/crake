using System;
using System.Text.RegularExpressions;

namespace Crake.Parser
{
	public class EndKeyword : IKeyword
	{
		#region IKeyword implementation
		public string[] IsMatch (IParsedObject parent, ref System.IO.StreamReader file, string text)
		{
			if (!string.IsNullOrEmpty (text)) {
				if (Regex.IsMatch(text, "^\\s*end\\b")){
					return new string[] { text };
				}
			}
			return null;
		}
		
		
		public string Parse (IParsedObject parent, ref System.IO.StreamReader file, params object[] parts)
		{
			return parts[0] as string;
		}
		
		#endregion
	}
}
