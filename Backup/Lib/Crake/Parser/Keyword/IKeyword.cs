using System;
using System.IO;

namespace Crake.Parser
{
	public interface IKeyword
	{
		string[] IsMatch(IParsedObject parent, ref StreamReader file, string text);
		string Parse(IParsedObject parent, ref StreamReader file, params object[] parts);
	}
}
