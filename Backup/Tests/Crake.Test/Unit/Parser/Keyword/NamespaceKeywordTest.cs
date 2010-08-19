using System;
using NUnit.Framework;
using Crake.Parser;
using System.IO;
using System.Text;

namespace Crake.Test
{
	[TestFixture()]
	public class NamespaceKeywordTest
	{
		private NamespaceKeyword keyword = new NamespaceKeyword ();
		private StreamReader file = new StreamReader (new MemoryStream ());
		private CrakeFile parent = new CrakeFile ();

		[Test()]
		public void ShouldMatch1 ()
		{
			var actual = keyword.IsMatch (parent, ref file, "namespace :name1 do");
			Assert.AreEqual (new[] { ":name1", "do", "" }, actual);
		}

		[Test()]
		public void ShouldMatch2 ()
		{
			var actual = keyword.IsMatch (parent, ref file, "  namespace :name_1 do");
			Assert.AreEqual (new[] { ":name_1", "do", "" }, actual);
		}

		[Test()]
		public void ShouldMatch3 ()
		{
			var actual = keyword.IsMatch (parent, ref file, "\tnamespace :name_space1   do");
			Assert.AreEqual (new[] { ":name_space1", "do", "" }, actual);
		}

		[Test()]
		public void ShouldMatch4 ()
		{
			var actual = keyword.IsMatch (parent, ref file, "\tnamespace   do");
			Assert.AreEqual (new[] { "", "do", "" }, actual);
		}

		[Test()]
		public void ShouldMatch5 ()
		{
			var actual = keyword.IsMatch (parent, ref file, "\tnamespace   ");
			Assert.AreEqual (new[] { "", "", "" }, actual);
		}

		[Test()]
		public void ShouldMatch6 ()
		{
			var actual = keyword.IsMatch (parent, ref file, "\tnamespace   :do");
			Assert.AreEqual (new[] { ":do", "", "" }, actual);
		}

		[Test()]
		public void ShouldMatch7 ()
		{
			var actual = keyword.IsMatch (parent, ref file, "namespace name1 do");
			Assert.AreEqual (new[] { "name1", "do", "" }, actual);
		}

		[Test()]
		public void ShouldMatch8 ()
		{
			var actual = keyword.IsMatch (parent, ref file, "namespace :\"name1\" do");
			Assert.AreEqual (new[] { ":\"name1\"", "do", "" }, actual);
		}

		[Test()]
		public void ShouldMatch9 ()
		{
			var actual = keyword.IsMatch (parent, ref file, "namespace :'name1' do");
			Assert.AreEqual (new[] { ":'name1'", "do", "" }, actual);
		}

		[Test()]
		public void ShouldMatch10 ()
		{
			var actual = keyword.IsMatch (parent, ref file, "namespace \"name1\" do");
			Assert.AreEqual (new[] { "\"name1\"", "do", "" }, actual);
		}

		[Test()]
		public void ShouldMatch11 ()
		{
			var actual = keyword.IsMatch (parent, ref file, "namespace 'name1' do");
			Assert.AreEqual (new[] { "'name1'", "do", "" }, actual);
		}

		[Test()]
		public void ShouldNotMatch ()
		{
			var file = new StreamReader (new MemoryStream ());
			var parent = new CrakeFile ();
			var actual = keyword.IsMatch (parent, ref file, "namespaces :name1 do");
			Assert.IsNull (actual);
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateParent ()
		{
			var file = new StreamReader (new MemoryStream ());
			var parent = new PlainTask (null, null);
			keyword.Parse (parent, ref file, ":name1", "do");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax1 ()
		{
			var file = new StreamReader (new MemoryStream ());
			var parent = new CrakeFile ();
			keyword.Parse (parent, ref file, "", "do");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax2 ()
		{
			var file = new StreamReader (new MemoryStream ());
			var parent = new CrakeFile ();
			keyword.Parse (parent, ref file, "", "");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax3 ()
		{
			var file = new StreamReader (new MemoryStream ());
			var parent = new CrakeFile ();
			keyword.Parse (parent, ref file, ":do", "");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax4 ()
		{
			var file = new StreamReader (new MemoryStream ());
			var parent = new CrakeFile ();
			keyword.Parse (parent, ref file, "name1", "do");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax5 ()
		{
			var file = new StreamReader (new MemoryStream ());
			var parent = new CrakeFile ();
			keyword.Parse (parent, ref file, ":\"name1\"", "do");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax6 ()
		{
			var file = new StreamReader (new MemoryStream ());
			var parent = new CrakeFile ();
			keyword.Parse (parent, ref file, ":'name1'", "do");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax7 ()
		{
			var file = new StreamReader (new MemoryStream ());
			var parent = new CrakeFile ();
			keyword.Parse (parent, ref file, "\"name1\"", "do");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax8 ()
		{
			var file = new StreamReader (new MemoryStream ());
			var parent = new CrakeFile ();
			keyword.Parse (parent, ref file, "'name1'", "do");
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateSyntax9 ()
		{
			var file = new StreamReader (new MemoryStream ());
			var parent = new CrakeFile ();
			keyword.Parse (parent, ref file, ":name1", "do", "");
		}

		[Test()]
		public void ShouldValidateSyntax10 ()
		{
			var file = new StreamReader (new MemoryStream ());
			var parent = new CrakeFile ();
			var actual = keyword.Parse (parent, ref file, ":name1", "do", "end");
			Assert.IsNullOrEmpty(actual);
		}

		[Test()]
		public void ShouldParse ()
		{
			var sb = new StringBuilder();
			sb.AppendLine("namespace :completa do");
			sb.AppendLine("\tend");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			var match = keyword.IsMatch(parent, ref file2, file2.ReadLine());
			var actual = keyword.Parse(parent, ref file2, match);
			Assert.IsNullOrEmpty(actual);
		}

		[Test()]
		public void ShouldParseMultiline ()
		{
			var sb = new StringBuilder();
			sb.AppendLine("namespace :completa");
			sb.AppendLine("do");
			sb.AppendLine("\tend");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			var match = keyword.IsMatch(parent, ref file2, file2.ReadLine());
			var actual = keyword.Parse(parent, ref file2, match);
			Assert.IsNullOrEmpty(actual);
		}

		[Test()]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldParseDescAlone ()
		{
			var sb = new StringBuilder();
			sb.AppendLine("namespace :completa");
			sb.AppendLine("do");
			sb.AppendLine("desc 'x'");
			sb.AppendLine("\tend");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			var match = keyword.IsMatch(parent, ref file2, file2.ReadLine());
			keyword.Parse(parent, ref file2, match);
		}

		[Test()]
		public void ShouldParseTaskName ()
		{
			var sb = new StringBuilder();
			sb.AppendLine("namespace :completa");
			sb.AppendLine("do");
			sb.AppendLine("\ttask :name do");
			sb.AppendLine("\tend");
			sb.AppendLine("end");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			parent.Tasks.Clear();
			var match = keyword.IsMatch(parent, ref file2, file2.ReadLine());
			keyword.Parse(parent, ref file2, match);
			Assert.AreEqual("completa:name", parent.Tasks[0].Name);
		}

		[Test()]
		public void ShouldParseTaskNameInSubnamespace ()
		{
			var sb = new StringBuilder();
			sb.AppendLine("namespace :completa");
			sb.AppendLine("do");
			sb.AppendLine("namespace :sub do");
			sb.AppendLine("\ttask :name do");
			sb.AppendLine("\tend");
			sb.AppendLine("\tend");
			sb.AppendLine("end");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			parent.Tasks.Clear();
			var match = keyword.IsMatch(parent, ref file2, file2.ReadLine());
			keyword.Parse(parent, ref file2, match);
			Assert.AreEqual("completa:sub:name", parent.Tasks[0].Name);
		}
	}
}
