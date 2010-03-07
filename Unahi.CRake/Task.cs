using System;
using System.Collections.Generic;

namespace Unahi.CRake {

	public class Task {
		
		public Task() {
		}
		
		public string Name { get; set; }
		public List<Task> Prerequisites { get; set; }
	}
}
