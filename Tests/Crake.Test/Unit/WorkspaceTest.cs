
using System;
using NUnit.Framework;
using System.IO;
using System.Text;
using System.Reflection;

namespace Crake.Test
{


	[TestFixture()]
	public class WorkspaceTest
	{
		private string basePath;

		[SetUp]
		public void Setup()
		{
			basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WorkspaceTest");
			Directory.CreateDirectory(basePath);
		}
		
		[TearDown]
		public void Teardown()
		{
			Directory.Delete(basePath, true);
		}

		[Test()]
		[ExpectedException()]
		public void ShouldNotAllowMultipleCrakefiles ()
		{
			var workspace = new Workspace();
			workspace.BasePath = Path.Combine(basePath, "ShouldNotAllowMultipleCrakefiles");
			Directory.CreateDirectory(workspace.BasePath);
			File.Create(Path.Combine(workspace.BasePath, "crakefile")).Close();
			if (File.Exists(Path.Combine(workspace.BasePath, "Crakefile"))){
				Assert.Ignore("File System is not case sensitive.");
				return;
			}
			
			File.Create(Path.Combine(workspace.BasePath, "Crakefile"));
			workspace.LoadFiles();
		}
		
		[Test()]
		public void ShouldMatchExactlyFileName ()
		{
			var workspace = new Workspace();
			workspace.BasePath = Path.Combine(basePath, "ShouldNotAllowMultipleCrakefiles");
			Directory.CreateDirectory(workspace.BasePath);
            File.Create(Path.Combine(workspace.BasePath, "crakefile")).Close();
            File.Create(Path.Combine(workspace.BasePath, "cCrakefile")).Close();
			workspace.LoadFiles();
			Assert.AreEqual(1, workspace.Files.Count);
		}
		
		[Test()]
		public void ShouldLoadDefaultCrakefile()
		{
			var workspace = new Workspace();
			workspace.BasePath = Path.Combine(basePath, "ShouldLoadDefaultCrakefile");
			Directory.CreateDirectory(workspace.BasePath);
			File.Create(Path.Combine(workspace.BasePath, "crakefile")).Close();
			workspace.LoadFiles();
			Assert.AreEqual(1, workspace.Files.Count);
			Assert.AreEqual(Path.Combine(workspace.BasePath, "crakefile"), workspace.Files[0]);
		}
		
		[Test()]
		public void ShouldNotFailWhenDefaultCrakefileNotExists()
		{
			var workspace = new Workspace();
			workspace.BasePath = Path.Combine(basePath, "ShouldNotFailWhenDefaultCrakefileNotExists");
			Directory.CreateDirectory(workspace.BasePath);
			workspace.LoadFiles();
			Assert.AreEqual(0, workspace.Files.Count);
		}
		
		[Test()]
		public void ShouldLoadFilesFromTasksDir()
		{
			var workspace = new Workspace();
			workspace.BasePath = Path.Combine(basePath, "ShouldLoadFilesFromTasksDir");
			Directory.CreateDirectory(workspace.BasePath);
			Directory.CreateDirectory(workspace.TasksDir);
			File.Create(Path.Combine(workspace.TasksDir, "task1.crake")).Close();
            File.Create(Path.Combine(workspace.TasksDir, "task2.crake")).Close();
            File.Create(Path.Combine(workspace.TasksDir, "task3.cs")).Close();
			workspace.LoadFiles();
			Assert.AreEqual(2, workspace.Files.Count);
			Assert.AreEqual(Path.Combine(workspace.TasksDir,"task1.crake"), workspace.Files[0]);
			Assert.AreEqual(Path.Combine(workspace.TasksDir,"task2.crake"), workspace.Files[1]);
		}
		
		[Test()]
		public void ShouldLoadFilesFromTasksSubDir()
		{
			var workspace = new Workspace();
			workspace.BasePath = Path.Combine(basePath, "ShouldLoadFilesFromTasksDir");
			Directory.CreateDirectory(workspace.BasePath);
			Directory.CreateDirectory(workspace.TasksDir);
			Directory.CreateDirectory(Path.Combine(workspace.TasksDir, "subdir"));
            File.Create(Path.Combine(workspace.TasksDir, "task1.crake")).Close();
            File.Create(Path.Combine(Path.Combine(workspace.TasksDir, "subdir"), "task2.crake")).Close();
			workspace.LoadFiles();
			Assert.AreEqual(2, workspace.Files.Count);
			Assert.AreEqual(Path.Combine(workspace.TasksDir, "task1.crake"), workspace.Files[0]);
			Assert.AreEqual(Path.Combine(Path.Combine(workspace.TasksDir, "subdir"),"task2.crake"), workspace.Files[1]);
		}

        [Test()]
        public void ShouldParse() {
            var workspace = new Workspace();
            workspace.BasePath = Path.Combine(basePath, "Parse");
            Directory.CreateDirectory(workspace.TasksDir);
            var f = File.Create(Path.Combine(workspace.TasksDir, "parse.crake"));
            var buf = ASCIIEncoding.ASCII.GetBytes("namespace :name1 do\ntask :todo1 do\nSay();\nend\nend\n public void Say() {\nConsole.WriteLine(\"oi\");\n}");
            f.Write(buf, 0, buf.Length);
            f.Close();
            workspace.LoadFiles();
            workspace.Parse();
            Assert.AreEqual(1, workspace.ParsedFiles.Count);
            var actual = workspace.ParsedFiles[0];
            Assert.AreEqual(1, actual.Tasks.Count);
            Assert.AreEqual("name1:todo1", actual.Tasks[0].Name);
            Assert.AreEqual(1, actual.HelperMethods.Count);
            Assert.AreEqual("Say", actual.HelperMethods[0].Name.ToString());
        }

        [Test()]
        public void ShouldCompile() {
            var workspace = new Workspace();
            workspace.BasePath = Path.Combine(basePath, "Compile");
            Directory.CreateDirectory(workspace.TasksDir);
            var f = File.Create(Path.Combine(workspace.TasksDir, "task1.crake"));
            var buf = ASCIIEncoding.ASCII.GetBytes("namespace :name1 do\ntask :todo1 do\nSay();\nend\nend\n public void Say() {\nConsole.WriteLine(\"oi\");\n}");
            f.Write(buf, 0, buf.Length);
            f.Close();
            workspace.LoadFiles();
            workspace.Parse();
            workspace.Compile();

            var typeName = string.Format("Crake.Runtime.{0}", workspace.ParsedFiles[0].UniqueName);
            Type type = null;

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in loadedAssemblies) {
                type = asm.GetType(typeName);
                if (type != null) break;
            }

            Assert.IsNotNull(Assembly.GetAssembly(type));
        }

        [Test()]
        public void ShouldCompileNLines() {
            var workspace = new Workspace();
            workspace.BasePath = Path.Combine(basePath, "Compile");
            Directory.CreateDirectory(workspace.TasksDir);
            var f = File.Create(Path.Combine(workspace.TasksDir, "task1.crake"));
            var buf = ASCIIEncoding.ASCII.GetBytes("namespace :name1 do\ntask :todo1 do\nint i;\nSay();\nSay();\nend\nend\n public void Say() {\nConsole.WriteLine(\"oi\");\n}");
            f.Write(buf, 0, buf.Length);
            f.Close();
            workspace.LoadFiles();
            workspace.Parse();
            workspace.Compile();

            var typeName = string.Format("Crake.Runtime.{0}", workspace.ParsedFiles[0].UniqueName);
            Type type = null;

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in loadedAssemblies) {
                type = asm.GetType(typeName);
                if (type != null) break;
            }

            Assert.IsNotNull(Assembly.GetAssembly(type));
        }
    }
}
