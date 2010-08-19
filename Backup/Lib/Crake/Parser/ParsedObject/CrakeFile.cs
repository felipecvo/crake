using System;
using System.IO;
using System.Collections.Generic;

namespace Crake.Parser
{
	public class CrakeFile : TaskContainer
	{

		public CrakeFile ()
		{
			Dependencies = new List<IDependency>();
			Imports = new List<string>();
		}
		
		public List<IDependency> Dependencies { get; private set; }
		public List<string> Imports { get; set; }
		//public List<HelperMethod> HelperMethods { get; set; }
	}
}
