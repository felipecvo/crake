using System;
using NUnit.Framework;

namespace Crake.Test
{
	[TestFixture]
	[Crake.Description("test")]
	public class DescriptionAttributeTest
	{
		[Test]
		public void TestCase()
		{
			var target = new DescriptionAttributeTest();
			var attr = (DescriptionAttribute)target.GetType().GetCustomAttributes(typeof(Crake.DescriptionAttribute), false)[0];
			Assert.AreEqual("test", attr.Text);
		}
		
		[Test]
		public void ShouldNotAllowMultipleAttributes(){
			var attr = (AttributeUsageAttribute)typeof(DescriptionAttribute).GetCustomAttributes(typeof(AttributeUsageAttribute), false)[0];
			Assert.NotNull(attr);
			Assert.IsFalse(attr.AllowMultiple);
		}
		
		[Test]
		public void ShouldNotAllowInheritedAttributes(){
			var attr = (AttributeUsageAttribute)typeof(DescriptionAttribute).GetCustomAttributes(typeof(AttributeUsageAttribute), false)[0];
			Assert.NotNull(attr);
			Assert.IsFalse(attr.Inherited);
		}
	}
}
