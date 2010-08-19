using System;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace Crake.Parser {
  public class AssemblyDependency : IDependency {
    internal static bool Lock;
    static AssemblyDependency() {
      Lock = false;
    }

    public AssemblyDependency(string assemblyName) {
      Name = assemblyName;
    }

    public string Name { get; set; }

    public string FullName { get; set; }

    public string Location { get; set; }

    #region IDependency implementation
    public void Resolve() {
      Assembly asm = null;

      var dirs = new string[] { 
                AppDomain.CurrentDomain.BaseDirectory,
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\Debug"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\Release"),
                Environment.CurrentDirectory,
                Path.Combine(Environment.CurrentDirectory, "bin"),
                Path.Combine(Environment.CurrentDirectory, "bin\\Debug"),
                Path.Combine(Environment.CurrentDirectory, "bin\\Release")
            };
      try {
        asm = Assembly.Load(Name);
      } catch { }

      try {
        if (!Name.EndsWith(".dll")) {
          var gacPath = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(typeof(System.Xml.XmlDocument).Assembly.Location).FullName).FullName).FullName, Name);
          if (Directory.Exists(gacPath)) {
            var versions = Directory.GetDirectories(gacPath);
            var file = Path.Combine(Path.Combine(gacPath, versions[versions.Length - 1]), string.Format("{0}.dll", Name));
            if (File.Exists(file)) {
              var items = Regex.Split(new DirectoryInfo(versions[versions.Length - 1]).Name, "__");
              var fullName = string.Format("{0}, Version={1}, Culture=neutral, PublicKeyToken={2}", Name, items[0], items[1]);
              asm = Assembly.Load(fullName);
            }
          }
        }
      } catch (Exception ex) {
        Console.WriteLine(ex.Message);
      }

      if (asm == null) {
        var name = Name.EndsWith(".dll") ? Name : string.Format("{0}.dll", Name);
        foreach (var dir in dirs) {
          if (File.Exists(Path.Combine(dir, name))) {
            asm = Assembly.LoadFile(Path.Combine(dir, name));
          }
        }
      }

      if (asm == null) {
        throw new Exception(string.Format("'{0}' not found", Name));
      } else {
        FullName = asm.FullName;
        Location = asm.Location;
      }
    }

    #endregion
  }
}
