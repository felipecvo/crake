
using System;
using System.Text;

namespace Crake.Parser
{
	public class MethodHelper
	{
		public MethodHelper()
		{
			Return = new StringBuilder();
			Name = new StringBuilder();
			Params = new StringBuilder();
			Body = new StringBuilder();
		}

		public StringBuilder Return { get; private set; }
		public StringBuilder Name { get; private set; }
		public StringBuilder Params { get; private set; }
		public StringBuilder Body { get; private set; }
	}
}
