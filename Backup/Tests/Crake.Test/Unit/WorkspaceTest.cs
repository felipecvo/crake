
using System;
using NUnit.Framework;
using System.IO;

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
			File.Create(Path.Combine(workspace.BasePath, "crakefile"));
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
			File.Create(Path.Combine(workspace.BasePath, "crakefile"));
			File.Create(Path.Combine(workspace.BasePath, "cCrakefile"));
			workspace.LoadFiles();
			Assert.AreEqual(1, workspace.Files.Count);
		}
		
		[Test()]
		public void ShouldLoadDefaultCrakefile()
		{
			var workspace = new Workspace();
			workspace.BasePath = Path.Combine(basePath, "ShouldLoadDefaultCrakefile");
			Directory.CreateDirectory(workspace.BasePath);
			File.Create(Path.Combine(workspace.BasePath, "crakefile"));
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
			File.Create(Path.Combine(workspace.TasksDir, "task1.crake"));
			File.Create(Path.Combine(workspace.TasksDir, "task2.crake"));
			File.Create(Path.Combine(workspace.TasksDir, "task3.cs"));
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
			File.Create(Path.Combine(workspace.TasksDir, "task1.crake"));
			File.Create(Path.Combine(Path.Combine(workspace.TasksDir, "subdir"), "task2.crake"));
			workspace.LoadFiles();
			Assert.AreEqual(2, workspace.Files.Count);
			Assert.AreEqual(Path.Combine(workspace.TasksDir, "task1.crake"), workspace.Files[0]);
			Assert.AreEqual(Path.Combine(Path.Combine(workspace.TasksDir, "subdir"),"task2.crake"), workspace.Files[1]);
		}
	}
}
