using System;
using NUnit.Framework;
using Crake.Parser;
using System.IO;

namespace Crake.Test
{
	[TestFixture()]
	public class RequireKeywordTest
	{
		private RequireKeyword require = new RequireKeyword();

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax ()
		{
			var file = new StreamReader(new MemoryStream());
			var actual = require.IsMatch(new CrakeFile(), ref file, "require ;");
			Assert.AreEqual("", actual[0]);
			require.Parse(new CrakeFile(), ref file, actual);
		}

		[Test()]
		public void ShouldMatchRequire ()
		{
			var file = new StreamReader(new MemoryStream());
			var actual = require.IsMatch(new CrakeFile(), ref file, "require System;");
			Assert.AreEqual(new[]{"System"}, actual);
			actual = require.IsMatch(new CrakeFile(), ref file, " require System;");
			Assert.AreEqual(new[]{"System"}, actual);
			actual = require.IsMatch(new CrakeFile(), ref file, "\trequire System;");
			Assert.AreEqual(new[]{"System"}, actual);
		}

		[Test()]
		public void ShouldNotMatchRequire ()
		{
			var file = new StreamReader(new MemoryStream());
			var actual = require.IsMatch(new CrakeFile(), ref file, "rekuaire System;");
			Assert.IsNull(actual);
		}

		[Test()]
		public void ShouldNotMatchRequire2 ()
		{
			var file = new StreamReader(new MemoryStream());
			var actual = require.IsMatch(new CrakeFile(), ref file, "require System,Web;");
			Assert.IsNull(actual);
		}

		[Test()]
		public void ShouldReturnNullWhenDoNotMatch ()
		{
			var line = "line not match;";
			var file = new StreamReader(new MemoryStream());
			var actual = require.IsMatch(new CrakeFile(), ref file, line);
			Assert.AreEqual(null, actual);
		}

		[Test()]
		public void ShouldMatchRequireInQuotedString ()
		{
			var file = new StreamReader(new MemoryStream());
			var actual = require.IsMatch(new CrakeFile(), ref file, "require 'System';");
			Assert.AreEqual(new[]{"'System'"}, actual);
		}

		[Test()]
		public void ShouldMatchRequireInDoubleQuotedString ()
		{
			var file = new StreamReader(new MemoryStream());
			var actual = require.IsMatch(new CrakeFile(), ref file, "require \"System\";");
			Assert.AreEqual(new[]{"\"System\""}, actual);
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldAllowRequireOnlyInRootDeclaration ()
		{
			var file = new StreamReader(new MemoryStream());
			var parent = new PlainTask(null, null);
			var actual = require.IsMatch(parent, ref file, "require System;");
			Assert.IsNotNull(actual);
			require.Parse(parent, ref file, actual);
		}
		
		[Test()]
		public void ShouldParseAssemblyDependency()
		{
			var crakeFile = new CrakeFile();
			var file = new StreamReader(new MemoryStream());
			var actual = require.Parse(crakeFile, ref file, "System");
			Assert.AreEqual(null, actual);
			Assert.IsInstanceOf<AssemblyDependency>(crakeFile.Dependencies[0]);
			Assert.AreEqual("System", ((AssemblyDependency)crakeFile.Dependencies[0]).AssemblyName);
		}
		
		[Test()]
		public void ShouldParseCrakeFileDependency()
		{
			var crakeFile = new CrakeFile();
			var file = new StreamReader(new MemoryStream());
			var actual = require.Parse(crakeFile, ref file, "'System'");
			Assert.AreEqual(null, actual);
			Assert.IsInstanceOf<CrakeFileDependency>(crakeFile.Dependencies[0]);
			Assert.AreEqual("System", ((CrakeFileDependency)crakeFile.Dependencies[0]).FileName);
		}
		
		[Test()]
		public void ShouldParseCrakeFileDependency2()
		{
			var crakeFile = new CrakeFile();
			var file = new StreamReader(new MemoryStream());
			var actual = require.Parse(crakeFile, ref file, "\"System\"");
			Assert.AreEqual(null, actual);
			Assert.IsInstanceOf<CrakeFileDependency>(crakeFile.Dependencies[0]);
			Assert.AreEqual("System", ((CrakeFileDependency)crakeFile.Dependencies[0]).FileName);
		}
		
		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldThrowSyntaxError1()
		{
			var crakeFile = new CrakeFile();
			var file = new StreamReader(new MemoryStream());
			require.Parse(crakeFile, ref file, "\"System");
		}
		
		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldThrowSyntaxError2()
		{
			var crakeFile = new CrakeFile();
			var file = new StreamReader(new MemoryStream());
			require.Parse(crakeFile, ref file, "\"System'");
		}
		
		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldThrowSyntaxError3()
		{
			var crakeFile = new CrakeFile();
			var file = new StreamReader(new MemoryStream());
			require.Parse(crakeFile, ref file, "\"System");
		}
	}
}
