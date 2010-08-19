using System;

namespace Crake.Parser
{


	public class CrakeFileDependency : IDependency
	{
		#region IDependency implementation
		public void Resolve ()
		{
			throw new System.NotImplementedException();
		}
		
		#endregion

		public string FileName { get; set; }

		public CrakeFileDependency (string fileName)
		{
			FileName = fileName;
		}
	}
}
