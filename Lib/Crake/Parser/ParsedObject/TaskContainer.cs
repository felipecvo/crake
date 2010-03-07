using System;
using System.Collections.Generic;

namespace Crake.Parser
{
	public class TaskContainer : IParsedObject
	{
		public TaskContainer ()
		{
			Tasks = new List<PlainTask>();
		}
		
		public List<PlainTask> Tasks { get; private set; }
		
		public PlainDesc LastDesc { get; set; }

		#region IParsedObject implementation
		public IParsedObject Parent {
			get;
			set;
		}
		
		#endregion
	}
}
