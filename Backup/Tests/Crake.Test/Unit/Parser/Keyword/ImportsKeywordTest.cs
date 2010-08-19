using System;
using NUnit.Framework;
using Crake.Parser;
using System.IO;

namespace Crake.Test
{
	[TestFixture()]
	public class ImportsKeywordTest
	{
		private ImportsKeyword imports = new ImportsKeyword();

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax ()
		{
			var file = new StreamReader(new MemoryStream());
			var actual = imports.IsMatch(new CrakeFile(), ref file, "imports ;");
			Assert.AreEqual("", actual[0]);
			imports.Parse(new CrakeFile(), ref file, actual);
		}

		[Test()]
		public void ShouldMatch ()
		{
			var file = new StreamReader(new MemoryStream());
			var actual = imports.IsMatch(new CrakeFile(), ref file, "imports System.Web;");
			Assert.AreEqual(new[]{"System.Web"}, actual);
			actual = imports.IsMatch(new CrakeFile(), ref file, " imports System.Web;");
			Assert.AreEqual(new[]{"System.Web"}, actual);
			actual = imports.IsMatch(new CrakeFile(), ref file, "\timports System.Web;");
			Assert.AreEqual(new[]{"System.Web"}, actual);
		}

		[Test()]
		public void ShouldNotMatch ()
		{
			var file = new StreamReader(new MemoryStream());
			var actual = imports.IsMatch(new CrakeFile(), ref file, "using System;");
			Assert.IsNull(actual);
		}

		[Test()]
		public void ShouldNotMatch2 ()
		{
			var file = new StreamReader(new MemoryStream());
			var actual = imports.IsMatch(new CrakeFile(), ref file, "imports System,sws;");
			Assert.IsNull(actual);
		}

		[Test()]
		public void ShouldReturnNullWhenDoNotMatch ()
		{
			var line = "line not match;";
			var file = new StreamReader(new MemoryStream());
			var actual = imports.IsMatch(new CrakeFile(), ref file, line);
			Assert.IsNull(actual);
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldAllowRequireOnlyInRootDeclaration ()
		{
			var file = new StreamReader(new MemoryStream());
			var parent = new PlainTask(null, null);
			var actual = imports.IsMatch(parent, ref file, "imports System;");
			Assert.IsNotNull(actual);
			imports.Parse(parent, ref file, actual);
		}

		[Test()]
		public void ShouldParseImports()
		{
			var crakeFile = new CrakeFile();
			var file = new StreamReader(new MemoryStream());
			var actual = imports.Parse(crakeFile, ref file, "System");
			Assert.AreEqual(null, actual);
			Assert.AreEqual("System", crakeFile.Imports[0]);
		}
		
		[Test()]
		public void ShouldParseImports2()
		{
			var crakeFile = new CrakeFile();
			var file = new StreamReader(new MemoryStream());
			var actual = imports.Parse(crakeFile, ref file, "System.Web");
			Assert.AreEqual(null, actual);
			Assert.AreEqual("System.Web", crakeFile.Imports[0]);
		}
		
		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldThrowSyntaxError()
		{
			var crakeFile = new CrakeFile();
			var file = new StreamReader(new MemoryStream());
			imports.Parse(crakeFile, ref file, "");
		}

	}
}
