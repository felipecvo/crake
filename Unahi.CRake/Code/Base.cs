using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unahi.CRake.Code {
    public class Base {
        public Base() {
            Namespaces = new List<Namespace>();
            Tasks = new List<Task>();
            Imports = new List<string>();
            Codes = new List<string>();
        }

        public List<Namespace> Namespaces { get; set; }
        public List<Task> Tasks { get; set; }
        public List<string> Imports { get; set; }
        public List<string> Codes { get; set; }
        public string Name { get; set; }
    }
}
