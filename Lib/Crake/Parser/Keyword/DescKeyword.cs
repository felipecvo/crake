using System;
using System.Text.RegularExpressions;

namespace Crake.Parser
{
	public class DescKeyword : IKeyword
	{
		#region IKeyword implementation
		public string[] IsMatch (IParsedObject parent, ref System.IO.StreamReader file, string text)
		{
			if (!string.IsNullOrEmpty(text)) {
				var match = Regex.Match(text, "^\\s*desc\\s+(\"[^\\\\\"]*(:?\\\\.[^\\\\\"]*)*\")?;?$");
				if (match.Success) {
					return new string[] { match.Groups[1].Value };
				} else if ((match = Regex.Match(text, "^\\s*desc\\s+('[^']*')?;?$")).Success) {
					return new string[] { match.Groups[1].Value };
				}
			}
			return null;
		}
		
		
		public string Parse (IParsedObject parent, ref System.IO.StreamReader file, params object[] parts)
		{
			throw new System.NotImplementedException();
		}
		
		#endregion
	}
}
