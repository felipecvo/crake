using System;
using NUnit.Framework;
using Crake.Parser;
using System.IO;

namespace Crake.Test
{
	[TestFixture()]
	public class DescKeywordTest
	{
		private DescKeyword keyword = new DescKeyword ();
		private StreamReader file = new StreamReader (new MemoryStream ());
		private CrakeFile parent = new CrakeFile ();

		[Test()]
		public void ShouldMatch1 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "desc \"x \\\" y\";");
			Assert.AreEqual (new[] { "\"x \\\" y\"" }, actual);
		}

		[Test()]
		public void ShouldMatch2 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "desc 'x \" y';");
			Assert.AreEqual (new[] { "'x \" y'" }, actual);
		}

		[Test()]
		public void ShouldMatch3 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "desc ");
			Assert.AreEqual (new[] { "" }, actual);
		}

		[Test()]
		public void ShouldNotMatch ()
		{
			var actual = keyword.IsMatch(parent, ref file, "descs ");
			Assert.IsNull(actual);
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateParent1 ()
		{
			keyword.Parse(new TaskKeyword(), ref file, "");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateParent2 ()
		{
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateParent3 ()
		{
		}

		[Test()]
		public void ShouldValidateSyntax ()
		{
		}

		[Test()]
		public void ShouldParse ()
		{
		}
	}
}
