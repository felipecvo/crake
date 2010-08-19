using System;
using System.Text.RegularExpressions;

namespace Crake.Parser
{
	public class MethodKeyword : IKeyword
	{
		enum Step {
			Return,
			Name,
			Params,
			Body,
			End
		};

		#region IKeyword implementation
		public string[] IsMatch (IParsedObject parent, ref System.IO.StreamReader file, string text)
		{
			var match = Regex.Match(text, "^\\s*(public|protected|private)\\s*(.*)");
			if (match.Success) {
				return new string[]{ match.Groups[2].Value };
			}
			return null;
		}

		public string Parse (IParsedObject parent, ref System.IO.StreamReader file, params object[] parts)
		{
			if (!(parent is CrakeFile)) throw new SyntaxErrorException();

			var method = new MethodHelper();
			var text = (string)parts[0];
			var step = Step.Return;
			do {
				foreach (var c in text.ToCharArray()) {
					step = ParseChar(method, step, c);
				}
			} while (null != (text = file.ReadLine()));
			return null;
		}

		#endregion

		private Step ParseChar(MethodHelper method, Step step, char c)
		{
			switch (step) {
			case Step.Return:
				return ParseReturn(method, c);
			case Step.Name:
				return ParseName(method, c);
			case Step.Params:
				return ParseParams(method, c);
			case Step.Body:
			default:
				return ParseBody(method, c);
			}
		}
		
		private Step ParseReturn(MethodHelper method, char c)
		{
			if (c == ' ' || c == '\t' || c == '\r' || c == '\n') {
				return Step.Name;
			} else {
				method.Return.Append(c);
				return Step.Return;
			}
		}
		
		private Step ParseName(MethodHelper method, char c)
		{
			if (c == '(') {
				return Step.Params;
			} else {
				method.Name.Append(c);
				return Step.Name;
			}
		}
		
		private Step ParseParams(MethodHelper method, char c)
		{
			if (c == ')') {
				brackets = 0;
				return Step.Body;
			} else {
				method.Params.Append(c);
				return Step.Params;
			}
		}

		private int brackets = 0;
		private Step ParseBody(MethodHelper method, char c)
		{
			if (c == '{') brackets++;
			if (c == '}') {
				brackets--;
				if (brackets == 0) return Step.End;
			}
			method.Body.Append(c);
			return Step.Body;
		}
	}
}
