
using System;
using System.IO;
using System.Collections.Generic;

namespace Crake.Parser {


  public sealed class CrakeFileParser {
    private static List<IKeyword> Keywords = new List<IKeyword>();
    static CrakeFileParser() {
      Keywords.Add(new RequireKeyword());
      Keywords.Add(new ImportsKeyword());
      Keywords.Add(new NamespaceKeyword());
      Keywords.Add(new DescKeyword());
      Keywords.Add(new TaskKeyword());
      Keywords.Add(new EndKeyword());
      Keywords.Add(new CommentKeyword());
      Keywords.Add(new MethodKeyword());
      Keywords.Add(new EmptyKeyword());
      Keywords.Add(new UnknownKeyword());
    }

    private CrakeFileParser() {
    }

    public static CrakeFile Parse(string fileName) {
      if (!File.Exists(fileName)) {
        throw new FileNotFoundException("Crakefile '{0}' not found!", fileName);
      }

      var crakeFile = new CrakeFile();
      crakeFile.Name = Path.GetFileName(fileName);

      var file = new StreamReader(File.OpenRead(fileName));
      try {
        var line = file.ReadLine();
        while (line != null) {
          line = MatchKeyword(crakeFile, ref file, line);
          if (string.IsNullOrEmpty(line) || line == "\n") {
            line = file.ReadLine();
          }
        }
      } finally {
        file.Dispose();
      }

      foreach (var dependency in crakeFile.Dependencies) {
        dependency.Resolve();
      }

      return crakeFile;
    }

    static internal string MatchKeyword(IParsedObject parent, ref StreamReader file, string line) {
      foreach (IKeyword keyword in Keywords) {
        var match = keyword.IsMatch(parent, ref file, line);
        if (match != null) {
          return keyword.Parse(parent, ref file, match);
        }
      }
      return null;
    }
  }
}
