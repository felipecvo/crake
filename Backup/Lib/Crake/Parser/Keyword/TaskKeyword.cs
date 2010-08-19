using System;
using System.Text.RegularExpressions;
using System.Text;

namespace Crake.Parser
{
	public class TaskKeyword : IKeyword
	{
		#region IKeyword implementation
		public string[] IsMatch (IParsedObject parent, ref System.IO.StreamReader file, string text)
		{
			if (!string.IsNullOrEmpty (text)) {
				var match = Regex.Match (text, "^\\s*task\\s+(.*)");
				if (match.Success) {
					return new string[] { match.Groups[1].Value };
				}
			}
			return null;
		}

		public string Parse (IParsedObject parent, ref System.IO.StreamReader file, params object[] parts)
		{
			if (!(parent is TaskContainer))
				throw new SyntaxErrorException ();
			
			var line = (string)parts[0];
			var task = new PlainTask (null, parent);
			var match = Regex.Match (line, "^\\s*:(\\w+)\\s+(.*)");
			if (match.Success) {
				task.Name = match.Groups[1].Value;
				line = match.Groups[2].Value;
			} else {
				throw new SyntaxErrorException ();
			}
			
			if ((match = Regex.Match (line, "^=>\\s+(.*)")).Success) {
				line = match.Groups[1].Value;
				if ((match = Regex.Match (line, "^\\[([^\\[]+)\\]\\s+(.*)")).Success) {
					line = match.Groups[2].Value;
					var items = match.Groups[1].Value.Split(',');
					foreach (var item in items) {
						Console.WriteLine(item);
						if ((match = Regex.Match(item, "^\\s*:(\\w+)\\b")).Success) {
							task.Prerequisites.Add(match.Groups[1].Value);
						} else if ((match = Regex.Match(item, "^\\s*\"([\\w:]+)\"\\s*$")).Success) {
							task.Prerequisites.Add(match.Groups[1].Value);
						} else {
							throw new SyntaxErrorException("prerequisite syntax error");
						}
					}
				} else {
					if ((match = Regex.Match(line, "^\\s*:(\\w+)\\s+(.*)")).Success) {
						task.Prerequisites.Add(match.Groups[1].Value);
						line = match.Groups[2].Value;
					} else if ((match = Regex.Match(line, "^\\s*\"([\\w:]+)\"\\s+(.*)")).Success) {
						task.Prerequisites.Add(match.Groups[1].Value);
						line = match.Groups[2].Value;
					} else {
						throw new SyntaxErrorException("prerequisite syntax error");
					}
				}
			}

			var _do = false;
			do {
				if (!(match = Regex.Match (line, "^\\s*do\\s*(.*)")).Success) {
					if (Regex.IsMatch (line, "^\\s*//|^\\s*$")) {
						continue;
					} else {
						throw new SyntaxErrorException ("command not expected. expected do\n" + line);
					}
				} else {
					_do = true;
					line = match.Groups[1].Value;
					break;
				}
			} while (!string.IsNullOrEmpty (line = file.ReadLine ()));
			if (!_do) throw new SyntaxErrorException ("do not found");
			
			var end = false;
			var body = new StringBuilder();
			do {
				if (string.IsNullOrEmpty(line)) continue;
				if ((match = Regex.Match (line, "^\\s*end\\b(.*)")).Success) {
					line = match.Groups[1].Value;
					end = true;
					break;
				} else {
					body.AppendLine(line);
				}
			} while (!string.IsNullOrEmpty (line = file.ReadLine ()));
			if (!end) throw new SyntaxErrorException("end not found");

			task.Body = body.ToString();
			
			if (((TaskContainer)parent).LastDesc != null) {
				task.Description = ((TaskContainer)parent).LastDesc.Text;
				((TaskContainer)parent).LastDesc = null;
			}

			((TaskContainer)parent).Tasks.Add (task);
			
			return line;
		}
		
		#endregion
	}
}
