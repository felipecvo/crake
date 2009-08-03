using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unahi.CRake.Code;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace Unahi.CRake {
    class CRake : Base<CRake> {

        public override void Run() {
            if (Arguments.IsHelp) {
                Console.Write(Arguments.ToHelp("crake [-p | --port numport][-u] [-t | --tasks] namespace:task"));
                return;
            }

            var files = Configuration.Load();

            var compiler = new Compiler();
            foreach (var file in files) {
                FileParser.Parse(compiler, file);
            }

            var binary = compiler.Compile();

            if (ShowTasks) {
                foreach (var task in binary.Tasks) {
                    Console.WriteLine(task.Key);
                }
                return;
            }

            if (Arguments.Values.Count > 0) {
                CodeNamespace codeNamespace = new CodeNamespace("Unahi.CRake.RuntimeGenerated");
                CodeCompileUnit codeUnit = new CodeCompileUnit();
                codeUnit.Namespaces.Add(codeNamespace);
                CodeDomProvider compiler2 = CodeDomProvider.CreateProvider("CSharp");
                string[] references = new string[2] { "System.Web.dll", "System.dll" };
                CompilerParameters parameters = new CompilerParameters(references);

                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));

                CodeTypeDeclaration type = new CodeTypeDeclaration();
                type.IsClass = true;
                type.Name = "Namespace";
                type.Attributes = MemberAttributes.Public;
                codeNamespace.Types.Add(type);

                CodeMemberMethod testMethod = new CodeMemberMethod();
                testMethod.Name = "DynamicMethod";
                testMethod.Attributes = MemberAttributes.Public;
                testMethod.Statements.Add(new CodeSnippetStatement("Console.WriteLine(\"oi\");"));
                type.Members.Add(testMethod);

                CompilerResults results = compiler2.CompileAssemblyFromDom(parameters, codeUnit);

                var objectTask2 = results.CompiledAssembly.CreateInstance("Unahi.CRake.RuntimeGenerated.Namespace");
                objectTask2.GetType().InvokeMember("DynamicMethod", BindingFlags.InvokeMethod, null, objectTask2, null);

                var key = Arguments.Values[0];
                foreach (var task in binary.Tasks) {
                    if (task.Key == key) {
                        if (task.Compiled.Errors.HasErrors) {
                            foreach (CompilerError error in task.Compiled.Errors) {
                                Console.WriteLine(error.ErrorText);
                            }
                            return;
                        }
                        var objectTask = task.CompiledAssembly.CreateInstance("Unahi.CRake.RuntimeGenerated.Namespace");
                        objectTask.GetType().InvokeMember("DynamicMethod", BindingFlags.InvokeMethod, null, objectTask, null);
                    }
                }
            }
        }

        public bool ShowTasks {
            get {
                return (bool)Arguments["t"];
            }
        }

        public override void Init() {
            Arguments.AddArgument('t', "tasks", "List all tasks available to CRake", ArgumentType.Bit, false);
        }
    }
}