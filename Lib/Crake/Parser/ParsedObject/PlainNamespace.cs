using System;

namespace Crake.Parser
{
	public class PlainNamespace : TaskContainer
	{
		public PlainNamespace (string name, IParsedObject parent)
		{
			this.Parent = parent;
			Name = name;
		}
		
		public string Name { get; private set; }

		#region IParsedObject implementation
		
		public void AddTask (PlainTask task)
		{
			throw new System.NotImplementedException();
		}
		
		#endregion
	}
}
