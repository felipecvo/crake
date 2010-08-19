using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using Crake.Parser;

namespace Crake {
  public delegate void TaskRunner();

  public class Task {
    private static Dictionary<string, Task> tasks;
    public static Dictionary<string, Task> Tasks {
      get {
        return tasks ?? (tasks = new Dictionary<string, Task>());
      }
    }

    private static Dictionary<string, AssemblyDependency> dependencies;
    public static Dictionary<string, AssemblyDependency> Dependencies {
      get {
        return dependencies ?? (dependencies = new Dictionary<string, AssemblyDependency>());
      }
    }

    private TaskRunner runnerMethod;

    public Task(string name) : this(name, null, null) { }

    public Task(string name, string description) : this(name, description, null) { }

    public Task(string name, string description, TaskRunner method) {
      Name = name;
      Description = description;
      runnerMethod = method;
      if (Tasks.ContainsKey(Name)) throw new DuplicateCrakeTaskException(Name);
      Tasks.Add(name, this);
    }

    public string Name { get; private set; }
    public string Description { get; private set; }

    private List<Task> prerequisites;
    public List<Task> Prerequisites {
      get {
        return prerequisites ?? (prerequisites = new List<Task>());
      }
    }

    public void Run() {
      foreach (var task in Prerequisites) {
        task.Run();
      }
      if (runnerMethod != null) {
        try {
          AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
          runnerMethod();
        } catch (TargetInvocationException ex) {
          Console.WriteLine(ex.InnerException.Message);
          Console.WriteLine(ex.InnerException.StackTrace);
        }
      }
      RunInternal();
    }

    Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
      if (Task.Dependencies.ContainsKey(args.Name)) {
        return Assembly.LoadFile(Task.Dependencies[args.Name].Location);
      }
      if (!AssemblyDependency.Lock) {
        AssemblyDependency.Lock = true;
        var asmName = new AssemblyName(args.Name);
        var dep = new AssemblyDependency(asmName.Name);
        try {
          dep.Resolve();
        } catch {
        }
        AssemblyDependency.Lock = false;
        if (!string.IsNullOrEmpty(dep.Location)) {
          return Assembly.LoadFile(dep.Location);
        }
      }
      return null;
    }

    protected virtual void RunInternal() { }

    public static void Run<T>() where T : new() {
      var task = new T() as Task;
      foreach (var prerequisite in task.Prerequisites)
        prerequisite.Run();
      task.Run();
    }
  }
}