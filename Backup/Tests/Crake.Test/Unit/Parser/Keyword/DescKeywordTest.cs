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
			Assert.AreEqual (new[] { "\"x \\\" y\";" }, actual);
		}

		[Test()]
		public void ShouldMatch2 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "desc 'x \" y';");
			Assert.AreEqual (new[] { "'x \" y';" }, actual);
		}

		[Test()]
		public void ShouldMatch3 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "desc ");
			Assert.AreEqual (new[] { "" }, actual);
		}

		[Test()]
		public void ShouldMatch4 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "desc 'xyz'; task");
			Assert.AreEqual (new[] { "'xyz'; task" }, actual);
		}

		[Test()]
		public void ShouldMatch5 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "desc eu sou a lenda;");
			Assert.AreEqual (new[] { "eu sou a lenda;" }, actual);
		}

		[Test()]
		public void ShouldMatch6 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "desc \"xyz\"; task");
			Assert.AreEqual (new[] { "\"xyz\"; task" }, actual);
		}

		[Test()]
		public void ShouldMatch7 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "desc \"xyz\" task");
			Assert.AreEqual (new[] { "\"xyz\" task" }, actual);
		}

		[Test()]
		public void ShouldMatch8 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "desc 'xyz' task");
			Assert.AreEqual (new[] { "'xyz' task" }, actual);
		}

		[Test()]
		public void ShouldMatch9 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "desc xyz; //errado");
			Assert.AreEqual (new[] { "xyz; //errado" }, actual);
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
			keyword.Parse(new PlainTask(null, null), ref file, "");
		}

		[Test()]
		public void ShouldValidateParent2 ()
		{
			keyword.Parse(parent, ref file, "'123'");
			Assert.IsTrue(true);
		}

		[Test()]
		public void ShouldValidateParent3 ()
		{
			keyword.Parse(new PlainNamespace(null, null), ref file, "'123'");
			Assert.IsTrue(true);
		}

		[Test()]
		public void ShouldValidateSyntax1 ()
		{
			parent.LastDesc = null;
			var actual = keyword.Parse(parent, ref file, "'123';");
			Assert.IsNotNull(parent.LastDesc);
			Assert.AreEqual("123", parent.LastDesc.Text);
			Assert.IsNullOrEmpty(actual);
		}

		[Test()]
		public void ShouldValidateSyntax2 ()
		{
			parent.LastDesc = null;
			var actual = keyword.Parse(parent, ref file, "\"123 456\";");
			Assert.IsNotNull(parent.LastDesc);
			Assert.AreEqual("123 456", parent.LastDesc.Text);
			Assert.IsNullOrEmpty(actual);
		}

		[Test()]
		public void ShouldValidateSyntax3 ()
		{
			parent.LastDesc = null;
			var actual = keyword.Parse(parent, ref file, "'12\"3 456';");
			Assert.IsNotNull(parent.LastDesc);
			Assert.AreEqual("12\"3 456", parent.LastDesc.Text);
			Assert.IsNullOrEmpty(actual);
		}

		[Test()]
		public void ShouldValidateSyntax4 ()
		{
			parent.LastDesc = null;
			var actual = keyword.Parse(parent, ref file, "\"12\\\"3 456\";");
			Assert.IsNotNull(parent.LastDesc);
			Assert.AreEqual("12\"3 456", parent.LastDesc.Text);
			Assert.IsNullOrEmpty(actual);
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax5 ()
		{
			keyword.Parse(parent, ref file, "'123 456\";");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax6 ()
		{
			keyword.Parse(parent, ref file, "\"123 456';");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax7 ()
		{
			keyword.Parse(parent, ref file, "\"123 456;");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax8 ()
		{
			keyword.Parse(parent, ref file, "'123 456;");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax9 ()
		{
			keyword.Parse(parent, ref file, "123 456;");
		}

		[Test()]
		public void ShouldValidateSyntax10 ()
		{
			var actual = keyword.Parse(parent, ref file, "'123 456'; algo mais");
			Assert.AreEqual(" algo mais", actual);
		}

		[Test()]
		public void ShouldValidateSyntax11 ()
		{
			parent.LastDesc = null;
			var actual = keyword.Parse(parent, ref file, "'12\\\"3 456';");
			Assert.IsNotNull(parent.LastDesc);
			Assert.AreEqual("12\\\"3 456", parent.LastDesc.Text);
			Assert.IsNotNull(actual);
		}

		[Test()]
		public void ShouldValidateSyntax12 ()
		{
			parent.LastDesc = null;
			var actual = keyword.Parse(parent, ref file, "'';");
			Assert.IsNotNull(parent.LastDesc);
			Assert.AreEqual("", parent.LastDesc.Text);
			Assert.IsNotNull(actual);
		}

		[Test()]
		public void ShouldValidateSyntax13 ()
		{
			var actual = keyword.Parse(parent, ref file, "'123 456' algo mais");
			Assert.AreEqual(" algo mais", actual);
		}
	}
}
