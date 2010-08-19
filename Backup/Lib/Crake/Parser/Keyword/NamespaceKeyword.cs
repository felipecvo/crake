using System;
using System.Text.RegularExpressions;

namespace Crake.Parser
{
	public class NamespaceKeyword : IKeyword
	{
		#region IKeyword implementation
		public string[] IsMatch (IParsedObject parent, ref System.IO.StreamReader file, string text)
		{
			if (!string.IsNullOrEmpty (text)) {
				var match = Regex.Match (text, "^\\s*namespace\\s+([^\\s]*)(\\s+(do)\\b)?(.*)");
				if (match.Success) {
					var name = match.Groups[1].Value == "do" ? string.Empty : match.Groups[1].Value;
					var _do = match.Groups[1].Value == "do" ? "do" : match.Groups[3].Value;
					return new string[] { name, _do, match.Groups[4].Value };
				}
			}
			return null;
		}


		public string Parse (IParsedObject parent, ref System.IO.StreamReader file, params object[] parts)
		{
			if (!(parent is TaskContainer)) {
				throw new SyntaxErrorException ();
			}
			var name = (string)parts[0];
			var _do = (string)parts[1];
			
			var match = Regex.Match (name, ":(\\w+)");
			if (!match.Success)
				throw new SyntaxErrorException ();
			name = match.Groups[1].Value;
			
			string line = null;
			if (_do != "do") {
				while (!string.IsNullOrEmpty (line = file.ReadLine ())) {
					match = Regex.Match (line, "^\\s*do\\b(.*)");
					if (!match.Success) {
						if (EmptyLine (line)) {
							continue;
						} else {
							throw new SyntaxErrorException ();
						}
					} else {
						_do = "do";
						line = match.Groups[1].Value;
						break;
					}
				}
				if (_do != "do")
					throw new SyntaxErrorException ();
			} else {
				line = (string)parts[2];
			}
			
			var _namespace = new PlainNamespace (name, parent);
			var end = false;
			do {
				if (string.IsNullOrEmpty(line)) continue;
				var child = CrakeFileParser.MatchKeyword (_namespace, ref file, line);
				match = Regex.Match (child, "^\\s*end\\b(.*)");
				if (match.Success) {
					line = match.Groups[1].Value;
					end = true;
					break;
				}
			} while (!string.IsNullOrEmpty (line = file.ReadLine ()));
			
			if (!end)
				throw new SyntaxErrorException ();

			if (_namespace.LastDesc != null) throw new SyntaxErrorException("desc for no task");

			foreach (var task in _namespace.Tasks) {
				task.Name = string.Format ("{0}:{1}", _namespace.Name, task.Name);
				((TaskContainer)parent).Tasks.Add (task);
			}
			
			return line;
		}

		#endregion

		private bool EmptyLine (string line)
		{
			return Regex.IsMatch (line, "^\\s*//|^\\s*$");
		}
	}
}
