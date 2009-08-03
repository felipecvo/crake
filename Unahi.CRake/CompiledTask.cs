using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Unahi.CRake {
    public class CompiledTask {
        public string Key { get; set; }
        public string Description { get; set; }
        public string MethodName { get; set; }
        public string ClassName { get; set; }
    }
}
