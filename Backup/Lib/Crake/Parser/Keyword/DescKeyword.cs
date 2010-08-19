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
				var match = Regex.Match(text, "^\\s*desc\\s+(.*)");
				if (match.Success) {
					return new string[] { match.Groups[1].Value };
				}
				/*
				var match = Regex.Match(text, "^\\s*desc\\s+(\"[^\\\\\"]*(:?\\\\.[^\\\\\"]*)*\")?;?$");
				if (match.Success) {
					return new string[] { match.Groups[1].Value };
				} else if ((match = Regex.Match(text, "^\\s*desc\\s+('[^']*')?;?$")).Success) {
					return new string[] { match.Groups[1].Value };
				}
				*/
			}
			return null;
		}
		
		
		public string Parse (IParsedObject parent, ref System.IO.StreamReader file, params object[] parts)
		{
			if (!(parent is TaskContainer)) {
				throw new SyntaxErrorException();
			}

			var line = (string)parts[0];
			var desc = string.Empty;
			var match = Regex.Match(line, "^(\"([^\\\\\"]*(:?\\\\.[^\\\\\"]*)*)\")(.*)");
			if (match.Success) {
				desc = Regex.Unescape(match.Groups[2].Value);
				line = match.Groups[4].Value;
			} else if ((match = Regex.Match(line, "^'([^']*)'(.*)")).Success) {
				desc = match.Groups[1].Value;
				line = match.Groups[2].Value;
			} else {
				throw new SyntaxErrorException();
			}
			if ((match = Regex.Match(line, "^\\s*;(.*)")).Success) {
				line = match.Groups[1].Value;
			}
			((TaskContainer)parent).LastDesc = new PlainDesc(desc, parent);

			return line;
		}
		
		#endregion
	}
}
