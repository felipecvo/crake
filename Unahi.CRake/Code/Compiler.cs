using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace Unahi.CRake.Code {
    public class Compiler : Base {
        private ICodeCompiler CSharpCompiler;
        private CompilerParameters parameters;
        string templateCode = @"using System;
 
namespace Unahi.CRake.RuntimeGenerated {{
    public class Namespace {{
        public void DynamicMethod(params object[] parameters) {{
           {0}
        }}
    }}
}}";

        public Compiler() {
            Require = new List<string>();
        }

        public List<string> Require { get; set; }

        internal Binary Compile() {
            CSharpCompiler = new CSharpCodeProvider().CreateCompiler();
            parameters = new CompilerParameters();
            foreach (var require in Require) {
                parameters.ReferencedAssemblies.Add(string.Format("{0}.dll", require));
            }
            parameters.GenerateInMemory = true;

            var binary = new Binary();
            CompileTasks(binary, "", Tasks);
            CompileNamespaces(binary, "", Namespaces);
            return binary;
        }

        private void CompileNamespaces(Binary binary, string parent, List<Namespace> namespaces) {
            foreach (var namespc in namespaces) {
                var name = string.Format("{0}{1}{2}", parent, !string.IsNullOrEmpty(parent) ? ":" : "", namespc.Name);
                CompileTasks(binary, name, namespc.Tasks);
                CompileNamespaces(binary, name, namespc.Namespaces);
            }
        }

        private void CompileTasks(Binary binary, string parent, List<Task> tasks) {
            foreach (var task in tasks) {
                binary.Tasks.Add(new CompiledTask() {
                    Key = string.Format("{0}{1}{2}", parent, !string.IsNullOrEmpty(parent) ? ":" : "", task.Name),
                    Compiled = CSharpCompiler.CompileAssemblyFromSource(parameters, string.Format(templateCode, task.Body))
                });
            }
        }

    }
}
