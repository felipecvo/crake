using System;

namespace Crake.Parser
{
	public class TaskKeyword : IKeyword
	{
		#region IKeyword implementation
		public string[] IsMatch (IParsedObject parent, ref System.IO.StreamReader file, string text)
		{
			throw new System.NotImplementedException();
		}
		
		
		public string Parse (IParsedObject parent, ref System.IO.StreamReader file, params object[] parts)
		{
			throw new System.NotImplementedException();
		}
		
		#endregion
		public TaskKeyword ()
		{
		}
	}
}