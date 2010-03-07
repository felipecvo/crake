using System;
using System.Collections.Generic;
using System.Threading;

namespace Crake
{
	public delegate void TaskRunner();
	
	public class Task
	{
		private static Dictionary<string, Task> tasks;
		public static Dictionary<string, Task> Tasks
		{
			get
			{
				return tasks ?? (tasks = new Dictionary<string, Task>());
			}
		}

		private TaskRunner runnerMethod;
		
		public Task (string name) : this(name, null) { }

		public Task (string name, TaskRunner method) {
			Name = name;
			runnerMethod = method;
			if (Tasks.ContainsKey(Name)) throw new DuplicateCrakeTaskException(Name);
			Tasks.Add(name, this);
		}

		public string Name { get; private set; }

		private List<Task> prerequisites;
		public List<Task> Prerequisites
		{
			get
			{
				return prerequisites ?? (prerequisites = new List<Task>());
			}
		}
		
		public void Run()
		{
			foreach(var task in Prerequisites){
				task.Run();
			}
			if (runnerMethod != null) runnerMethod.DynamicInvoke();
			RunInternal();
		}

		protected virtual void RunInternal() { }

		public static void Run<T>() where T:new()
		{
			var task = new T() as Task;
			foreach(var prerequisite in task.Prerequisites)
				prerequisite.Run();
			task.Run();
		}
	}
}