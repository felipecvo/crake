using System;
using NUnit.Framework;
using System.IO;
using Crake.Parser;
using System.Text;
using System.Text.RegularExpressions;

namespace Crake.Test
{
	[TestFixture()]
	public class CrakeFileParserTest
	{
        private string path;

        [SetUp]
        public void Setup() {
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CrakeFileParserTest");
            Directory.CreateDirectory(path);
        }

        [Test()]
		[ExpectedException(typeof(FileNotFoundException))]
		public void ShouldThrowAnExceptionWhenFileNotExists ()
		{
			var file = "x";
			CrakeFileParser.Parse(file);
		}

        [Test()]
        public void ShouldParseAndReturnCrakeFile() {
            var file = Path.Combine(path, "Crakefile");
            var f = File.Create(file);
            var buf = ASCIIEncoding.ASCII.GetBytes("namespace :name1 do\ntask :todo1 do\nint i = 1;\nend\nend\n public void Say() {\nConsole.WriteLine(\"oi\");\n}");
            f.Write(buf, 0, buf.Length);
            f.Close();
            var actual = CrakeFileParser.Parse(file);
            Assert.IsNotNull(actual);
            Assert.That(Regex.IsMatch(actual.UniqueName, "Crakefile_\\d+"));
        }

        [Test()]
        public void ShouldParseTaskAndReturnCrakeFile() {
            var file = Path.Combine(path, "Task1.crake");
            var f = File.Create(file);
            var buf = ASCIIEncoding.ASCII.GetBytes("namespace :name1 do\ntask :todo1 do\nint i = 1;\nend\nend\n public void Say() {\nConsole.WriteLine(\"oi\");\n}");
            f.Write(buf, 0, buf.Length);
            f.Close();
            var actual = CrakeFileParser.Parse(file);
            Assert.IsNotNull(actual);
            Assert.That(Regex.IsMatch(actual.UniqueName, "Task1_\\d+"));
        }

        [Test()]
        public void ShouldResolveHelperMethodReference() {
            var file = Path.Combine(path, "Task1.crake");
            var f = File.Create(file);
            var buf = ASCIIEncoding.ASCII.GetBytes(@"namespace :name1 do
task :todo1 do
Say();
end
end
public void Say() {
Console.WriteLine(1);
}");
            f.Write(buf, 0, buf.Length);
            f.Close();
            var actual = CrakeFileParser.Parse(file);
            Assert.IsNotNull(actual);
            Assert.AreEqual("HelperMethods.Say();" + Environment.NewLine, actual.Tasks[0].Body);
        }

        [Test()]
        public void ShouldResolveMultilineHelperMethodReference() {
            var file = Path.Combine(path, "Task1.crake");
            var f = File.Create(file);
            var buf = ASCIIEncoding.ASCII.GetBytes(@"namespace :name1 do
task :todo1 do
Say();
Say();
Say(1, 2, 3, 'A');
end
end
public void Say() {
Console.WriteLine(1);
}");
            f.Write(buf, 0, buf.Length);
            f.Close();
            var actual = CrakeFileParser.Parse(file);
            Assert.IsNotNull(actual);
            Assert.AreEqual(@"HelperMethods.Say();
HelperMethods.Say();
HelperMethods.Say(1, 2, 3, 'A');
", actual.Tasks[0].Body);
        }
    }
}
