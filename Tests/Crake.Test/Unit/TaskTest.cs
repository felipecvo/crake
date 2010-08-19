
using System;
using NUnit.Framework;

namespace Crake.Test
{


	[TestFixture()]
	public class TaskTest
	{

		[Test()]
		public void ShouldRunDelegateMethod ()
		{
			var runned = false;
			var task = new Task("name", null, delegate() {
				runned = true;
			});
			task.Run();
			Assert.IsTrue(runned);
		}

		[Test()]
		public void ShouldRunPrerequisites ()
		{
			bool runned = false, pre_runned = false;

			var prerequisite = new Task("prerequisite", null, delegate() {
				pre_runned = true;
			});

			var task = new Task("pre:name", null, delegate() {
				runned = true;
			});
			
			task.Prerequisites.Add(prerequisite);
			task.Run();

			Assert.IsTrue(pre_runned);
			Assert.IsTrue(runned);
		}
		
		[Test]
		public void ShouldAddTaskToListofAvaiableTasks()
		{
			var task = Task.Tasks.ContainsKey("task_onlist");
			Assert.IsFalse(task);
			new Task("task_onlist");
			Assert.IsNotNull(Task.Tasks["task_onlist"]);
		}
		
		[Test]
		[ExpectedException(typeof(DuplicateCrakeTaskException))]
		public void ShouldThrowExceptionWhenDefineTwoTaslsWithSameName()
		{
			new Task("task_duplicated");
			new Task("task_duplicated");
		}
	}
}
