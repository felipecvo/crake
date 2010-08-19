
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Crake
{


	public class Workspace
	{

		public Workspace ()
		{
		}

		public string BasePath { get; set; }
		
		private string tasksDir;
		public string TasksDir {
			get { return tasksDir ?? (tasksDir = Path.Combine(BasePath, "Tasks")); }
			set { tasksDir = value; }
		}

		public List<string> Files { get; private set; }

		public void LoadFiles()
		{
			Files = new List<string>();

			var rootFiles = Directory.GetFiles(BasePath);
			foreach (var file in rootFiles)
			{
				if (Regex.IsMatch(Path.GetFileName(file), "^crakefile$", RegexOptions.IgnoreCase))
				{
					if (Files.Count > 0)
					{
						throw new Exception("Duplicate crakefile. You have more than 1 crakefile in root directory!");
					}
					Files.Add(file);
				}
			}

			if (Directory.Exists(TasksDir))
			{
				var taskFiles = Directory.GetFiles(TasksDir, "*.crake", SearchOption.AllDirectories);
				foreach (var file in taskFiles)
				{
					Files.Add(file);
				}
			}
		}
	}
}
