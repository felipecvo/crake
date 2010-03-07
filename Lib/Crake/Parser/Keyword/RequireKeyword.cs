using System;
using System.Text.RegularExpressions;

namespace Crake.Parser
{
	public class RequireKeyword : IKeyword
	{
		#region IKeyword implementation
		public string[] IsMatch (IParsedObject parent, ref System.IO.StreamReader file, string text)
		{
			if (!string.IsNullOrEmpty (text)) {
				var match = Regex.Match(text, "^\\s*require\\s+([\"'\\w.\\/]+)?;?$");
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

			var dependency = (string)parts[0];
			var match = Regex.Match(dependency, "^\"(.+?)\";?$|^'(.+?)';?$");
			if (match.Success) {
				var name = string.IsNullOrEmpty(match.Groups[1].Value) ? match.Groups[2].Value : match.Groups[1].Value;
				((CrakeFile)parent).Dependencies.Add(new CrakeFileDependency(name));
			} else if (Regex.IsMatch(dependency, "^[\\w\\.]+;?$")) {
				((CrakeFile)parent).Dependencies.Add(new AssemblyDependency(dependency));
			} else {
				throw new SyntaxErrorException();
			}
			return null;
		}
		
		#endregion
	}
}
