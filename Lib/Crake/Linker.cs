using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Crake.Parser;

namespace Crake {
  public class Linker {
    private Workspace workspace;
    public Linker(Workspace workspace) {
      this.workspace = workspace;
    }

    public void CreateTasks() {
      foreach (var crakeFile in workspace.ParsedFiles) {
        foreach (var task in crakeFile.Tasks) {
          var method = GetMethod(crakeFile.UniqueName, task.Name);
          var t = new Task(task.Name, task.Description, method);
        }
      }
      AddPrerequisites();
      AddDependencies();
    }

    private void AddDependencies() {
      foreach (var crakeFile in workspace.ParsedFiles) {
        foreach (var dep in crakeFile.Dependencies) {
          var asmDep = dep as AssemblyDependency;
          if (asmDep != null && !Task.Dependencies.ContainsKey(asmDep.Name)) {
            Task.Dependencies.Add(asmDep.FullName, asmDep);
          }
        }
      }
    }

    private void AddPrerequisites() {
      foreach (var crakeFile in workspace.ParsedFiles) {
        foreach (var task in crakeFile.Tasks) {
          foreach (var prerequisite in task.Prerequisites) {
            Task.Tasks[task.Name].Prerequisites.Add(Task.Tasks[prerequisite]);
          }
        }
      }
    }

    private TaskRunner GetMethod(string className, string methodName) {
      var loaded = AppDomain.CurrentDomain.GetAssemblies();
      Type type = null;
      foreach (var asm in loaded) {
        type = asm.GetType(string.Format("Crake.Runtime.{0}", className));
        if (type != null) break;
      }
      var method = type.GetMethod(methodName.Replace(":", "_"), BindingFlags.Public | BindingFlags.Static);

      return (TaskRunner)Delegate.CreateDelegate(typeof(TaskRunner), method);
    }
  }
}
