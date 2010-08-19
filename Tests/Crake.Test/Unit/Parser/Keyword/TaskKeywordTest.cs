using System;
using NUnit.Framework;
using System.IO;
using Crake.Parser;
using System.Text;

namespace Crake.Test
{
	[TestFixture()]
	public class TaskKeywordTest
	{
		private TaskKeyword keyword = new TaskKeyword ();
		private StreamReader file = new StreamReader (new MemoryStream ());
		private CrakeFile parent = new CrakeFile ();

		[Test()]
		public void ShouldMatch ()
		{
			var actual = keyword.IsMatch(parent, ref file, "task :name do");
			Assert.AreEqual(new string[]{ ":name do" }, actual);
		}

		[Test()]
		public void ShouldNotMatch ()
		{
			var actual = keyword.IsMatch(parent, ref file, "tasc :name do");
			Assert.IsNull(actual);
		}
		
		[Test]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldParseWrongTaskName()
		{
			keyword.Parse(parent, ref file, "name do");
		}

		[Test]
		[ExpectedException(typeof(SyntaxErrorException))]
		public void ShouldValidateParent()
		{
			keyword.Parse(new PlainTask(null, null), ref file, ":name do");
		}

		[Test]
		public void ShouldParse1()
		{
			var sb = new StringBuilder();
			sb.AppendLine("linha1");
			sb.AppendLine("end");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			parent.Tasks.Clear();
			var actual = keyword.Parse(parent, ref file2, ":name do");
			Assert.AreEqual(1, parent.Tasks.Count);
			var task = parent.Tasks[0];
			Assert.AreEqual("name", task.Name);
			Assert.AreEqual(0, task.Prerequisites.Count);
			Assert.AreEqual("linha1" + Environment.NewLine, task.Body);
			Assert.IsNullOrEmpty(actual);
		}

		[Test]
		public void ShouldParse2()
		{
			var sb = new StringBuilder();
			sb.AppendLine("linha1");
			sb.AppendLine("end");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			parent.Tasks.Clear();
			var actual = keyword.Parse(parent, ref file2, ":name => :name2 do");
			Assert.AreEqual(1, parent.Tasks.Count);
			var task = parent.Tasks[0];
			Assert.AreEqual("name", task.Name);
			Assert.AreEqual(1, task.Prerequisites.Count);
			Assert.AreEqual("name2", task.Prerequisites[0]);
            Assert.AreEqual("linha1" + Environment.NewLine, task.Body);
			Assert.IsNullOrEmpty(actual);
		}

		[Test]
		public void ShouldParse3()
		{
			var sb = new StringBuilder();
			sb.AppendLine("linha1");
			sb.AppendLine("end");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			parent.Tasks.Clear();
			var actual = keyword.Parse(parent, ref file2, ":name => \"task:name2\" do");
			Assert.AreEqual(1, parent.Tasks.Count);
			var task = parent.Tasks[0];
			Assert.AreEqual("name", task.Name);
			Assert.AreEqual(1, task.Prerequisites.Count);
			Assert.AreEqual("task:name2", task.Prerequisites[0]);
            Assert.AreEqual("linha1" + Environment.NewLine, task.Body);
			Assert.IsNullOrEmpty(actual);
		}

		[Test]
		public void ShouldParse4()
		{
			var sb = new StringBuilder();
			sb.AppendLine("linha1");
			sb.AppendLine("end");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			parent.Tasks.Clear();
			var actual = keyword.Parse(parent, ref file2, ":name => [:name1] do");
			Assert.AreEqual(1, parent.Tasks.Count);
			var task = parent.Tasks[0];
			Assert.AreEqual("name", task.Name);
			Assert.AreEqual(1, task.Prerequisites.Count);
			Assert.AreEqual("name1", task.Prerequisites[0]);
            Assert.AreEqual("linha1" + Environment.NewLine, task.Body);
			Assert.IsNullOrEmpty(actual);
		}

		[Test]
		public void ShouldParse5()
		{
			var sb = new StringBuilder();
			sb.AppendLine("linha1");
			sb.AppendLine("end");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			parent.Tasks.Clear();
			var actual = keyword.Parse(parent, ref file2, ":name => [:name1,:name2] do");
			Assert.AreEqual(1, parent.Tasks.Count);
			var task = parent.Tasks[0];
			Assert.AreEqual("name", task.Name);
			Assert.AreEqual(2, task.Prerequisites.Count);
			Assert.AreEqual("name1", task.Prerequisites[0]);
			Assert.AreEqual("name2", task.Prerequisites[1]);
            Assert.AreEqual("linha1" + Environment.NewLine, task.Body);
			Assert.IsNullOrEmpty(actual);
		}

		[Test]
		public void ShouldParse6()
		{
			var sb = new StringBuilder();
			sb.AppendLine("linha1");
			sb.AppendLine("end");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			parent.Tasks.Clear();
			var actual = keyword.Parse(parent, ref file2, ":name => [\"task:name1\",\"task:name2\"] do");
			Assert.AreEqual(1, parent.Tasks.Count);
			var task = parent.Tasks[0];
			Assert.AreEqual("name", task.Name);
			Assert.AreEqual(2, task.Prerequisites.Count);
			Assert.AreEqual("task:name1", task.Prerequisites[0]);
			Assert.AreEqual("task:name2", task.Prerequisites[1]);
            Assert.AreEqual("linha1" + Environment.NewLine, task.Body);
			Assert.IsNullOrEmpty(actual);
		}

		[Test]
		public void ShouldParse7()
		{
			var sb = new StringBuilder();
			sb.AppendLine("linha1");
			sb.AppendLine("end");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			parent.Tasks.Clear();
			var actual = keyword.Parse(parent, ref file2, ":name => [\"task:name1\",:name2] do");
			Assert.AreEqual(1, parent.Tasks.Count);
			var task = parent.Tasks[0];
			Assert.AreEqual("name", task.Name);
			Assert.AreEqual(2, task.Prerequisites.Count);
			Assert.AreEqual("task:name1", task.Prerequisites[0]);
			Assert.AreEqual("name2", task.Prerequisites[1]);
            Assert.AreEqual("linha1" + Environment.NewLine, task.Body);
			Assert.IsNullOrEmpty(actual);
		}

		[Test]
		public void ShouldParse8()
		{
			var sb = new StringBuilder();
			sb.AppendLine("linha1");
			sb.AppendLine("end");
			var ms = new MemoryStream();
			var bt = UTF8Encoding.UTF8.GetBytes(sb.ToString());
			ms.Write(bt, 0, bt.Length);
			ms.Seek(0, SeekOrigin.Begin);
			var file2 = new StreamReader(ms);
			parent.Tasks.Clear();
			parent.LastDesc = new PlainDesc("minha desc", parent);
			var actual = keyword.Parse(parent, ref file2, ":name => [\"task:name1\",:name2] do");
			Assert.AreEqual(1, parent.Tasks.Count);
			var task = parent.Tasks[0];
			Assert.AreEqual("name", task.Name);
			Assert.AreEqual(2, task.Prerequisites.Count);
			Assert.AreEqual("task:name1", task.Prerequisites[0]);
			Assert.AreEqual("name2", task.Prerequisites[1]);
            Assert.AreEqual("linha1" + Environment.NewLine, task.Body);
			Assert.AreEqual("minha desc", task.Description);
			Assert.IsNull(parent.LastDesc);
			Assert.IsNullOrEmpty(actual);
		}
	}
}