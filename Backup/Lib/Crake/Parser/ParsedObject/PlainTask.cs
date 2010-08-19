
using System;
using System.Collections.Generic;

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
		public string Body { get; set; }
		public string Description { get; set; }

		public List<string> Prerequisites { get; private set; }

		public PlainTask (string name, IParsedObject parent)
		{
			this.parent = parent;
			Name = name;
			Prerequisites = new List<string>();
		}
	}
}
