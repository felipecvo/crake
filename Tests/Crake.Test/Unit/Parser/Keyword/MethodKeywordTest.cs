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
		public void ShouldParse1()
		{
            parent.HelperMethods.Clear();
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
			var match = keyword.IsMatch(parent, ref file2, file2.ReadLine());
			var actual = keyword.Parse(parent, ref file2, match);
			Assert.IsNull(actual);
            Assert.AreEqual(1, parent.HelperMethods.Count);
            Assert.AreEqual("int", parent.HelperMethods[0].Return.ToString());
            Assert.AreEqual("MyHelperMethod", parent.HelperMethods[0].Name.ToString());
            Assert.AreEqual("string text, int size", parent.HelperMethods[0].Params.ToString());
            var sb1 = " \ttext = text.ToLowerCase();if (text.Length > size) {\t\treturn text.Substring(0, size);\t} else {\t\treturn text;\t}";
            Assert.AreEqual(sb1, parent.HelperMethods[0].Body.ToString());
		}

        [Test]
        public void ShouldParse2() {
            parent.HelperMethods.Clear();
            var sb = new StringBuilder();
            sb.AppendLine("public int MyHelperMethod(string text, int size) {");
            sb.AppendLine("\ttext = text.ToLowerCase();");
            sb.AppendLine("if (text.Length > size) {");
            sb.AppendLine("\t\treturn text.Substring(0, size);");
            sb.AppendLine("\t} else {");
            sb.AppendLine("\t\treturn text;");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            sb.AppendLine("public bool DoNothing() {");
            sb.AppendLine("if (false) {");
            sb.AppendLine("return true;");
            sb.AppendLine("}");
            sb.AppendLine("}");
            var ms = new MemoryStream();
            var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
            ms.Write(bt, 0, bt.Length);
            ms.Seek(0, SeekOrigin.Begin);
            var file2 = new StreamReader(ms);
            var match = keyword.IsMatch(parent, ref file2, file2.ReadLine());
            var actual = keyword.Parse(parent, ref file2, match);
            match = keyword.IsMatch(parent, ref file2, actual);
            actual = keyword.Parse(parent, ref file2, match);
            Assert.IsNull(actual);
            Assert.AreEqual(2, parent.HelperMethods.Count);
            Assert.AreEqual("int", parent.HelperMethods[0].Return.ToString());
            Assert.AreEqual("MyHelperMethod", parent.HelperMethods[0].Name.ToString());
            Assert.AreEqual("string text, int size", parent.HelperMethods[0].Params.ToString());
            var sb1 = " \ttext = text.ToLowerCase();if (text.Length > size) {\t\treturn text.Substring(0, size);\t} else {\t\treturn text;\t}";
            Assert.AreEqual(sb1, parent.HelperMethods[0].Body.ToString());
            Assert.AreEqual("bool", parent.HelperMethods[1].Return.ToString());
            Assert.AreEqual("DoNothing", parent.HelperMethods[1].Name.ToString());
            Assert.AreEqual("", parent.HelperMethods[1].Params.ToString());
            sb1 = " if (false) {return true;}";
            Assert.AreEqual(sb1, parent.HelperMethods[1].Body.ToString());
        }
	}
}
