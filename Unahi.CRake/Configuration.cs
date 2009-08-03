using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Unahi.CRake {
    public class Configuration {

        internal static List<StreamReader> Load() {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var files = new List<StreamReader>();

            var path = Path.Combine(basePath, "crakefile");
            if (File.Exists(path)) {
                files.Add(File.OpenText(path));
            }

            var tasksDir = Path.Combine(basePath, "tasks");
            if (Directory.Exists(tasksDir)) {
                foreach (var file in Directory.GetFiles(tasksDir, "*.crake")) {
                    files.Add(File.OpenText(file));
                }
            }

            return files;
        }
    }
}
