using System;
using NUnit.Framework;
using Crake.Parser;
using System.IO;

namespace Crake.Test
{
	[TestFixture()]
	public class UnknownKeywordTest
	{
		private UnknownKeyword keyword = new UnknownKeyword ();
		private StreamReader file = new StreamReader (new MemoryStream ());
		private CrakeFile parent = new CrakeFile ();
		
		[Test()]
		[ExpectedException(typeof(UnknownKeywordException))]
		public void IsMatchTestCase ()
		{
			keyword.IsMatch(parent, ref file, "nothing");
		}
		
		[Test()]
		[ExpectedException(typeof(UnknownKeywordException))]
		public void ParseTestCase ()
		{
			keyword.Parse(parent, ref file, "nothing");
		}
	}
}
