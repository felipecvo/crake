
using System;
using System.Text.RegularExpressions;

namespace Crake.Parser
{


	public class ImportsKeyword : IKeyword
	{
		#region IKeyword implementation
		public string[] IsMatch (IParsedObject parent, ref System.IO.StreamReader file, string text)
		{
			if (!string.IsNullOrEmpty (text)) {
				var match = Regex.Match(text, "^\\s*imports\\s+([\\w\\.]+)?;?$");
				if (match.Success)
				{
					return new string[] { match.Groups[1].Value };
				}
			}
			return null;
		}
		
		public string Parse (IParsedObject parent, ref System.IO.StreamReader file, params object[] parts)
		{
			if (!(parent is CrakeFile))
			{
				throw new SyntaxErrorException();
			}

			if (string.IsNullOrEmpty((string)parts[0]))
			{
				throw new SyntaxErrorException();
			}
			
			((CrakeFile)parent).Imports.Add((string)parts[0]);
			return null;
		}
		
		#endregion
	}
}
