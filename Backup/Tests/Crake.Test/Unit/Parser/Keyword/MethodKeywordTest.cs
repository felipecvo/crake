using System;
using NUnit.Framework;
using Crake.Parser;
using System.IO;
using System.Text;

namespace Crake.Test
{
	[TestFixture()]
	public class MethodKeywordTest
	{
		private MethodKeyword keyword = new MethodKeyword ();
		private StreamReader file = new StreamReader (new MemoryStream ());
		private CrakeFile parent = new CrakeFile ();

		[Test()]
		public void ShouldMatch1 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "  public void x");
			Assert.AreEqual(new string[]{ "void x" }, actual);
		}

		[Test()]
		public void ShouldMatch2 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "  private void x");
			Assert.AreEqual(new string[]{ "void x" }, actual);
		}

		[Test()]
		public void ShouldMatch3 ()
		{
			var actual = keyword.IsMatch(parent, ref file, "  protected void x");
			Assert.AreEqual(new string[]{ "void x" }, actual);
		}

		[Test()]
		public void ShouldNotMatch ()
		{
			var actual = keyword.IsMatch(parent, ref file, "  internal void x");
			Assert.IsNull(actual);
		}
		
		[Test()]
		[ExpectedException]
		public void ShouldValidateParent1 ()
		{
			keyword.Parse(new PlainNamespace(null, null), ref file, "  internal void x");
		}
		
		[Test()]
		public void ShouldValidateParent2 ()
		{
			keyword.Parse(parent, ref file, "  internal void x");
			Assert.IsTrue(true);
		}
		
		[Test]
		public void ShouldParse()
		{
			var sb = new StringBuilder();
			sb.AppendLine("public int MyHelperMethod(string text, int size) {");
			sb.AppendLine("\ttext = text.ToLowerCase();");
			sb.AppendLine("if (text.Length > size) {");
			sb.AppendLine("\t\treturn text.Substring(0, size);");
			sb.AppendLine("\t} else {");
			sb.AppendLine("\t\treturn text;");
			sb.AppendLine("\t}");
			sb.AppendLine("}");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			var match = keyword.IsMatch(parent, ref file2, sb.ToString());
			var actual = keyword.Parse(parent, ref file2, match);
			Assert.IsNull(actual);
			Assert.AreE
			Assert.AreEqual("int", )
		}
	}
}
