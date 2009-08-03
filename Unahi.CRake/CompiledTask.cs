using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Unahi.CRake {
    public class CompiledTask {
        public string Key { get; set; }
        public CompilerResults Compiled { get; set; }
        public Assembly CompiledAssembly {
            get {
                return Compiled.CompiledAssembly;
            }
        }
    }
}
