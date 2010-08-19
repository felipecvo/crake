using System;
using System.IO;
using System.Collections.Generic;

namespace Crake.Parser {
  public class CrakeFile : TaskContainer {
    public int identifier;

    public CrakeFile() {
      identifier = new Random((int)DateTime.Now.Ticks).Next(999);
      Dependencies = new List<IDependency>();
      Imports = new List<string>();
      HelperMethods = new List<MethodHelper>();
    }

    public string UniqueName {
      get {
        return string.Format("{0}_{1:000}", Path.GetFileNameWithoutExtension(Name), identifier);
      }
    }

    public string Name { get; set; }
    public List<IDependency> Dependencies { get; private set; }
    public List<string> Imports { get; set; }
    public List<MethodHelper> HelperMethods { get; set; }
  }
}
