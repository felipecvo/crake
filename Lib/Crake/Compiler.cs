using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.CodeDom;
using Crake.Parser;
using System.Reflection;
using System.IO;

namespace Crake {
  public class Compiler {
    private CodeDomProvider codeProvider;
    private CodeCompileUnit codeUnit;
    private CodeNamespace codeNamespace;
    private List<string> require;

    public Compiler() {
      codeProvider = CodeDomProvider.CreateProvider("CSharp");
      codeUnit = new CodeCompileUnit();
      codeNamespace = new CodeNamespace("Crake.Runtime");
      codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
      codeNamespace.Imports.Add(new CodeNamespaceImport("Crake"));
      require = new List<string>();
      require.Add("System.dll");
      require.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Crake.Core.dll"));
    }

    public void CompileParsedCrakeFile(CrakeFile crakeFile) {
      var type = new CodeTypeDeclaration(crakeFile.UniqueName);
      type.IsClass = true;
      type.Attributes = MemberAttributes.Public | MemberAttributes.Static;

      foreach (var item in crakeFile.Imports) {
        codeNamespace.Imports.Add(new CodeNamespaceImport(item));
      }

      foreach (var plainTask in crakeFile.Tasks) {
        var task = new CodeMemberMethod();
        task.Attributes = MemberAttributes.Public | MemberAttributes.Static;
        task.Name = plainTask.Name.Replace(":", "_");
        task.Attributes = MemberAttributes.Public | MemberAttributes.Static;
        task.Statements.Add(new CodeSnippetStatement(plainTask.Body));
        type.Members.Add(task);
      }

      if (crakeFile.HelperMethods.Count > 0) {
        var helperMethods = new CodeTypeDeclaration("HelperMethods");
        helperMethods.IsClass = true;
        helperMethods.Attributes = MemberAttributes.Public | MemberAttributes.Static;

        foreach (var helperMethod in crakeFile.HelperMethods) {
          var method = new CodeMemberMethod();
          method.ReturnType = new CodeTypeReference(helperMethod.Return.ToString());
          method.Name = helperMethod.Name.ToString();
          method.Attributes = MemberAttributes.Public | MemberAttributes.Static;
          method.Statements.Add(new CodeSnippetStatement(helperMethod.Body.ToString()));
          if (helperMethod.Params.ToString().Trim() != string.Empty) {
            foreach (var param in helperMethod.Params.ToString().Split(',')) {
              var p = param.Split(' ');
              method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(p[0]), p[1]));
            }
          }
          helperMethods.Members.Add(method);
        }

        type.Members.Add(helperMethods);
      }

      foreach (var r in crakeFile.Dependencies) {
        if (r is AssemblyDependency) {
          var name = ((AssemblyDependency)r).Location;
          if (!require.Contains(name)) require.Add(name);
        }
      }

      codeNamespace.Types.Add(type);
      if (!codeUnit.Namespaces.Contains(codeNamespace))
        codeUnit.Namespaces.Add(codeNamespace);
    }

    public Assembly GenerateAssembly() {
      var parameters = new CompilerParameters(require.ToArray());
      var compiled = codeProvider.CompileAssemblyFromDom(parameters, codeUnit);
      if (compiled.Errors.Count > 0) {
        var builder = new StringBuilder();
        var writer = new StringWriter(builder);
        codeProvider.GenerateCodeFromCompileUnit(codeUnit, writer, new CodeGeneratorOptions());
        var sb = new StringBuilder();
        foreach (CompilerError error in compiled.Errors) {
          sb.AppendFormat("{1}:{2} => {0}\n", error.ErrorText, error.FileName, error.Line);
        }
        sb.AppendLine();
        sb.AppendLine(builder.ToString());
        throw new Exception(sb.ToString());
      } else {
        return compiled.CompiledAssembly;
      }
    }
  }
}
