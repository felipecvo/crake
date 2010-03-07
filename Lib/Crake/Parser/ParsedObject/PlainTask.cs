
using System;

namespace Crake.Parser
{


	public class PlainTask : IParsedObject
	{
		#region IParsedObject implementation
		private IParsedObject parent;
		public IParsedObject Parent {
			get {
				return parent;
			}
		}
		
		#endregion

		public string Name { get; set; }

		public PlainTask (string name, IParsedObject parent)
		{
			this.parent = parent;
			Name = name;
		}
	}
}
