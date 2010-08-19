using System;
using NUnit.Framework;
using Crake.Parser;
using System.IO;

namespace Crake.Test
{
	[TestFixture()]
	public class CommentKeywordTest
	{
		private CommentKeyword keyword = new CommentKeyword ();
		private StreamReader file = new StreamReader (new MemoryStream ());
		private CrakeFile parent = new CrakeFile ();
		
		[Test()]
		public void ShouldMatch ()
		{
			var actual = keyword.IsMatch(parent, ref file, "// comments");
			Assert.AreEqual("comments", actual[0]);
		}

		[Test()]
		public void ShouldNotMatch ()
		{
			var actual = keyword.IsMatch(parent, ref file, "# comments");
			Assert.IsNull(actual);
		}

		[Test()]
		public void ShouldParse ()
		{
			var actual = keyword.Parse(parent, ref file, "comments");
			Assert.IsEmpty(actual);
		}
	}
}
