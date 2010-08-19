using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Crake.Test {
    [TestFixture]
    public class LinkerTest {
        private string basePath;
        [SetUp]
        public void Setup() {
            basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LinkerTest");
            Directory.CreateDirectory(basePath);
        }

        [Test]
        public void ShouldCreateTasks() {
            Task.Tasks.Clear();
            var workspace = new Workspace();
            workspace.BasePath = Path.Combine(basePath, "ShouldCreateTasks");
            Directory.CreateDirectory(workspace.TasksDir);
            var f = File.Create(Path.Combine(workspace.TasksDir, "task1.crake"));
            var buf = ASCIIEncoding.ASCII.GetBytes("namespace :name1 do\ntask :todo1 do\nint i;\nSay();\nSay();\nend\nend\n public void Say() {\nConsole.WriteLine(\"oi\");\n}");
            f.Write(buf, 0, buf.Length);
            f.Close();
            workspace.LoadFiles();
            workspace.Parse();
            workspace.Compile();

            var linker = new Linker(workspace);

            Assert.AreEqual(0, Task.Tasks.Count);
            linker.CreateTasks();
            Assert.AreEqual(1, Task.Tasks.Count);
            Task.Tasks["name1:todo1"].Run();
        }
    }
}
