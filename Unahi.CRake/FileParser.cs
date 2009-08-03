﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Unahi.CRake.Code;

namespace Unahi.CRake {
    public class FileParser {
        const string MethodPattern = "^(require|namespace|desc|task|end)\\s+(.+)";
        Compiler compiler = new Compiler();
        string tempDescription = string.Empty;

        public static void Parse(Compiler compiler, StreamReader reader) {
            var content = reader.ReadToEnd();
            reader.Dispose();

            content = content.Replace("\r", "").Replace("\n", " ");
            content = Regex.Replace(Regex.Replace(content, "\\s+", " "), "$\\s*", "");

            var fileParser = new FileParser(compiler);
            while (!string.IsNullOrEmpty(content)){
                var parser = Regex.Split(content, MethodPattern);
                content = fileParser.ProcessMethod(fileParser.compiler, parser[1], parser[2]);
            }
            var x = fileParser.compiler;
        }

        public FileParser(Compiler compiler) {
            this.compiler = compiler;
        }

        private string ProcessMethod(Code.Base parent, string method, string body) {
            switch (method) {
                case "require":
                    return ProcessRequire(body);
                case "namespace":
                    return ProcessNamespace(parent, body);
                case "desc":
                    return ProcessDescription(parent, body);
                case "task":
                    return ProcessTask(parent, body);
            }
            return null;
        }

        private string ProcessTask(Base parent, string body) {
            var split = Regex.Split(body, ":(\\w+)\\s+do\\s+(.+?)\\s+end\\s*(.*)");
            var task = new Task() {
                Body = split[2],
                Name = split[1],
                Description = tempDescription
            };
            parent.Tasks.Add(task);
            return split[3];
        }

        private string ProcessDescription(Code.Base parent, string body) {
            var desc = Regex.Split(body, "^['\"](.+?)['\"]\\s+(.*)$");
            tempDescription = desc[1];
            return desc[2];
        }

        private string ProcessNamespace(Code.Base parent, string body) {
            var tokens = Regex.Split(body, ":(\\w+)\\s+do\\s+(.+)$");
            var namespc = new Namespace(tokens[1]);
            parent.Namespaces.Add(namespc);
            return ProcessBlock(namespc, tokens[2]);
        }

        private string ProcessBlock(Code.Base parent, string body) {
            var split = Regex.Split(body, MethodPattern);
            var method = split[1];
            body = split[2];
            while (method != "end") {
                body = ProcessMethod(parent, method, body);
                split = Regex.Split(body, MethodPattern);
                method = split[1];
                body = split[2];
            }
            return body;
        }

        private string ProcessRequire(string body) {
            var require = Regex.Split(body, "^['\"](.+?)['\"]\\s+(.*)$");
            compiler.Require.Add(require[1]);
            return require[2];
        }
    }
}
