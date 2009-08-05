using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.CodeDom;

namespace Unahi.CRake.Code {
    public class Compiler : Base {
        //        private ICodeCompiler CSharpCompiler;
        //        private CompilerParameters parameters;
        //        string templateCode = @"using System;
        // 
        //namespace Unahi.CRake.RuntimeGenerated {{
        //    public class Namespace {{
        //        public void DynamicMethod(params object[] parameters) {{
        //           {0}
        //        }}
        //    }}
        //}}";

        public Compiler() {
            Require = new List<string>();
            Name = "root";
        }

        public List<string> Require { get; set; }

        CodeNamespace codeNamespace;
        Binary binary;

        internal Binary Compile() {
            //CSharpCompiler = new CSharpCodeProvider().CreateCompiler();
            //parameters = new CompilerParameters();
            //foreach (var require in Require) {
            //    parameters.ReferencedAssemblies.Add(string.Format("{0}.dll", require));
            //}
            //parameters.GenerateInMemory = true;

            var compiler = CodeDomProvider.CreateProvider("CSharp");
            var parameters = new CompilerParameters(Require.ToArray());
            var codeUnit = new CodeCompileUnit();

            codeNamespace = new CodeNamespace("Unahi.CRake.RuntimeGenerated");
            codeUnit.Namespaces.Add(codeNamespace);

            binary = new Binary();

            foreach (var item in Imports) {
                codeNamespace.Imports.Add(new CodeNamespaceImport(item));
            }
            CompileClass("", Tasks, Namespaces, null);
            //CompileTasks(binary, "", Tasks, this, codeNamespace, null);
            //CompileNamespaces(binary, "", Namespaces, codeNamespace, null);

            binary.CompiledResult = compiler.CompileAssemblyFromDom(parameters, codeUnit);

            return binary;
        }

        private CodeTypeDeclaration CompileClass(string parent, List<Task> tasks, List<Namespace> subClasses, CodeTypeDeclaration parentType) {
            var className = parent.Split(':').Last();
            if (className == string.Empty) className = "root";
            className = string.Format("Class_{0}", className);

            CodeTypeDeclaration type = GetTypeDeclaration(parentType, className);
            if (type == null) {
                type = new CodeTypeDeclaration();
                type.BaseTypes.Add(typeof(ClassBase));
                type.IsClass = true;
                type.Name = className;
                type.Attributes = MemberAttributes.Public;
                if (parentType == null) {
                    codeNamespace.Types.Add(type);
                } else {
                    parentType.Members.Add(type);
                }
            }

            foreach (var task in tasks) {
                var method = CompileMethod(task.Name, task.Body);
                type.Members.Add(method);
                var compiledTask = new CompiledTask() {
                    ClassName = GetFullClassName(parent),
                    Description = task.Description,
                    Key = string.Format("{0}{1}{2}", parent, string.IsNullOrEmpty(parent) ? "" : ":", task.Name),
                    MethodName = method.Name
                };
                binary.Tasks.Add(compiledTask);
            }

            foreach (var subClass in subClasses) {
                foreach (var item in subClass.Imports) {
                    codeNamespace.Imports.Add(new CodeNamespaceImport(item));
                }
                var child = CompileClass(string.Format("{0}{1}{2}", parent, string.IsNullOrEmpty(parent) ? "" : ":", subClass.Name), subClass.Tasks, subClass.Namespaces, type);
                foreach (var code in subClass.Codes) {
                    var member = new CodeSnippetTypeMember(code);
                    child.Members.Add(member);
                }
            }

            return type;
        }

        private string GetFullClassName(string parent) {
            if (string.IsNullOrEmpty(parent)) {
                parent = "root";
            } else {
                parent = "root:" + parent;
            }
            var list = new List<string>();
            foreach (var item in parent.Split(':')) {
                list.Add(string.Format("Class_{0}", item));
            }
            return string.Format("{0}.{1}", codeNamespace.Name, string.Join("+", list.ToArray()));
        }

        private CodeMemberMethod CompileMethod(string key, string code) {
            var method = new CodeMemberMethod();
            method.Name = string.Format("Call_{0}", key.Replace(":", "_"));
            method.Attributes = MemberAttributes.Public;
            method.Statements.Add(new CodeSnippetStatement(code));
            return method;
        }

        private CodeTypeDeclarationCollection CompileNamespaces(Binary binary, string parent, List<Namespace> namespaces, CodeNamespace codeNamespace, CodeTypeDeclaration parentType) {
            foreach (var namespc in namespaces) {
                var name = string.Format("{0}{1}{2}", parent, !string.IsNullOrEmpty(parent) ? ":" : "", namespc.Name);
                var type = CompileTasks(binary, name, namespc.Tasks, namespc, codeNamespace, parentType);
                codeNamespace.Types.Add(type);
                var childrenTypes = CompileNamespaces(binary, name, namespc.Namespaces, codeNamespace, type);
                foreach (CodeTypeDeclaration childType in childrenTypes) {
                    type.Members.Add(childType);
                }
            }
            return codeNamespace.Types;
        }

        private CodeTypeDeclaration CompileTasks(Binary binary, string parent, List<Task> tasks, Base namespc, CodeNamespace codeNamespace, CodeTypeDeclaration parentType) {
            var className = string.Format("Class_{0}", namespc.Name);
            CodeTypeDeclaration type = GetTypeDeclaration(parentType, className);
            if (type == null) {
                type = new CodeTypeDeclaration();
                type.IsClass = true;
                type.Name = string.Format("Class_{0}", namespc.Name);
                type.Attributes = MemberAttributes.Public;
            }

            foreach (var task in tasks) {
                var key = string.Format("{0}{1}{2}", parent, !string.IsNullOrEmpty(parent) ? ":" : "", task.Name);
                binary.Tasks.Add(new CompiledTask() {
                    Key = key,
                    MethodName = string.Format("Call_{0}", key.Replace(":", "_")),
                    ClassName = type.Name
                    //Compiled = CompileTask(namespc, task.Body)
                });

                var method = new CodeMemberMethod();
                method.Name = string.Format("Call_{0}", key.Replace(":", "_"));
                method.Attributes = MemberAttributes.Public;
                method.Statements.Add(new CodeSnippetStatement(task.Body));
                type.Members.Add(method);
            }

            return type;
        }

        private CodeTypeDeclaration GetTypeDeclaration(CodeTypeDeclaration parentType, string className) {
            foreach (CodeTypeDeclaration type in codeNamespace.Types) {
                if (type.Name == className) {
                    return type;
                }
            }
            if (parentType != null) {
                foreach (CodeTypeMember member in parentType.Members) {
                    if (member is CodeTypeDeclaration && member.Name == className) {
                        return (CodeTypeDeclaration)member;
                    }
                }
            }
            return null;
        }

        private CompilerResults CompileTask(Base namespc, string code) {
            var codeNamespace = new CodeNamespace("Unahi.CRake.RuntimeGenerated");
            var codeUnit = new CodeCompileUnit();
            codeUnit.Namespaces.Add(codeNamespace);

            var compiler = CodeDomProvider.CreateProvider("CSharp");
            var parameters = new CompilerParameters(Require.ToArray());

            foreach (var item in namespc.Imports) {
                codeNamespace.Imports.Add(new CodeNamespaceImport(item));
            }

            var type = new CodeTypeDeclaration();
            type.IsClass = true;
            type.Name = "Namespace";
            type.Attributes = MemberAttributes.Public;
            codeNamespace.Types.Add(type);

            var testMethod = new CodeMemberMethod();
            testMethod.Name = "DynamicMethod";
            testMethod.Attributes = MemberAttributes.Public;
            testMethod.Statements.Add(new CodeSnippetStatement(code));
            type.Members.Add(testMethod);

            return compiler.CompileAssemblyFromDom(parameters, codeUnit);
        }

    }
}
