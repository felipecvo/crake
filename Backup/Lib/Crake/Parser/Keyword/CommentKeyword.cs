using System;
using System.Text.RegularExpressions;

namespace Crake.Parser
{
	public class CommentKeyword : IKeyword
	{
		#region IKeyword implementation
		public string[] IsMatch (IParsedObject parent, ref System.IO.StreamReader file, string text)
		{
			var match = Regex.Match(text, "^\\s*//\\s*(.*)$");
			if (match.Success) {
				return new string[] { match.Groups[1].Value };
			}
			return null;
		}
		
		
		public string Parse (IParsedObject parent, ref System.IO.StreamReader file, params object[] parts)
		{
			return string.Empty;
		}
		
		#endregion
	}
}
