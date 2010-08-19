using System;
using NUnit.Framework;
using Crake.Parser;
using System.IO;

namespace Crake.Test
{
	[TestFixture()]
	public class EndKeywordTest
	{
		private EndKeyword keyword = new EndKeyword ();
		private StreamReader file = new StreamReader (new MemoryStream ());
		private CrakeFile parent = new CrakeFile ();

		[Test()]
		public void ShouldMatch ()
		{
			var actual = keyword.IsMatch(parent, ref file, "end // ocmm");
			Assert.AreEqual("end // ocmm", actual[0]);
		}

		[Test()]
		public void ShouldNotMatch ()
		{
			var actual = keyword.IsMatch(parent, ref file, "endif");
			Assert.IsNull(actual);
		}

		[Test()]
		public void ShouldParse ()
		{
			var actual = keyword.Parse(parent, ref file, "end");
			Assert.AreEqual("end", actual);
		}
	}
}
