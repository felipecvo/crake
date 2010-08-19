
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Crake.Parser;
using System.Reflection;

namespace Crake {


  public class Workspace {

    public Workspace() {
      BasePath = Environment.CurrentDirectory;
    }

    public string BasePath { get; set; }
    public string GlobalTasksDir {
      get {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tasks");
      }
    }

    private string tasksDir;
    public string TasksDir {
      get { return tasksDir ?? (tasksDir = Path.Combine(BasePath, "Tasks")); }
      set { tasksDir = value; }
    }

    public List<string> Files { get; private set; }
    public List<CrakeFile> ParsedFiles { get; private set; }

    public void LoadFiles() {
      Files = new List<string>();

      var rootFiles = Directory.GetFiles(BasePath);
      foreach (var file in rootFiles) {
        if (Regex.IsMatch(Path.GetFileName(file), "^crakefile$", RegexOptions.IgnoreCase)) {
          if (Files.Count > 0) {
            throw new Exception("Duplicate crakefile. You have more than 1 crakefile in root directory!");
          }
          Files.Add(file);
        }
      }

      if (Directory.Exists(GlobalTasksDir)) {
        var taskFiles = Directory.GetFiles(GlobalTasksDir, "*.crake", SearchOption.TopDirectoryOnly);
        foreach (var file in taskFiles) {
          if (!Files.Contains(file))
            Files.Add(file);
        }
      }

      if (Directory.Exists(TasksDir)) {
        var taskFiles = Directory.GetFiles(TasksDir, "*.crake", SearchOption.AllDirectories);
        foreach (var file in taskFiles) {
          if (!Files.Contains(file))
            Files.Add(file);
        }
      }
    }

    public void Parse() {
      ParsedFiles = new List<CrakeFile>();
      Queue<string> filesToParse = new Queue<string>(Files);
      var file = string.Empty;
      while (filesToParse.Count > 0 && (file = filesToParse.Dequeue()) != null) {
        var crakeFile = CrakeFileParser.Parse(file);
        foreach (var item in crakeFile.Dependencies) {
          if (item is CrakeFileDependency && !Files.Contains(((CrakeFileDependency)item).FileName)) {
            filesToParse.Enqueue(((CrakeFileDependency)item).FileName);
          }
        }
        ParsedFiles.Add(crakeFile);
      }
    }

    public void Compile() {
      var compiler = new Compiler();
      foreach (var crakeFile in ParsedFiles) {
        compiler.CompileParsedCrakeFile(crakeFile);
      }
      Assembly.Load(compiler.GenerateAssembly().GetName());
    }

    public void Prepare() {
      LoadFiles();
      Parse();
      Compile();
      Link();
    }

    private void Run() {
    }

    private void Link() {
      var linker = new Linker(this);
      linker.CreateTasks();
    }
  }
}
