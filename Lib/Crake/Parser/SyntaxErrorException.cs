using System;

namespace Crake.Parser
{
	public class SyntaxErrorException : Exception
	{
		public SyntaxErrorException ()
		{
		}

		public SyntaxErrorException (string message) : base(message)
		{
		}
}
}
