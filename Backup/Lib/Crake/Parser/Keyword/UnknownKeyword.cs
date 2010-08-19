using System;

namespace Crake.Parser
{
	public class UnknownKeyword : IKeyword
	{
		#region IKeyword implementation
		public string[] IsMatch (IParsedObject parent, ref System.IO.StreamReader file, string line)
		{
			throw new UnknownKeywordException(line);
		}
		
		
		public string Parse (IParsedObject parent, ref System.IO.StreamReader file, params object[] line)
		{
			throw new UnknownKeywordException((string)line[0]);
		}
		
		#endregion
	}
	
	public class UnknownKeywordException : Exception
	{
		public UnknownKeywordException(string line) : base(string.Format("Unexpected code.\n'{0}'", line)) { }
	}
}
