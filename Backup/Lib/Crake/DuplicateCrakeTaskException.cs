using System;

namespace Crake
{
	public class DuplicateCrakeTaskException : Exception
	{
		private string taskName;

		public DuplicateCrakeTaskException (string taskName)
		{
			this.taskName = taskName;
		}
		
		public override string Message {
			get {
				return string.Format("The task '{0}' was duplicated.", taskName);
			}
		}

	}
}
