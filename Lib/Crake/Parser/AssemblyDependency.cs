using System;

namespace Crake.Parser
{
	public class AssemblyDependency : IDependency
	{
		public AssemblyDependency(string assemblyName)
		{
			AssemblyName = assemblyName;
		}

		public string AssemblyName { get; set; }
		
		#region IDependency implementation
		public void Resolve ()
		{
			throw new System.NotImplementedException();
		}
		
		#endregion
	}
}
