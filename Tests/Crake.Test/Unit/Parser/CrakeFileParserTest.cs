
using System;
using NUnit.Framework;
using System.IO;
using Crake.Parser;

namespace Crake.Test
{


	[TestFixture()]
	public class CrakeFileParserTest
	{

		[Test()]
		[ExpectedException(typeof(FileNotFoundException))]
		public void ShouldThrowAnExceptionWhenFileNotExists ()
		{
			var file = "x";
			CrakeFileParser.Parse(file);
		}
	}
}
