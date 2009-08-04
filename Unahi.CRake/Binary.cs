using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Unahi.CRake {
    public class Binary {
        public Binary() {
            Tasks = new List<CompiledTask>();
            instances = new Dictionary<string, object>();
        }

        private Dictionary<string, object> instances;
        public List<CompiledTask> Tasks { get; set; }
        public CompilerResults CompiledResult { get; set; }
        public Assembly CompiledAssembly {
            get {
                return CompiledResult.CompiledAssembly;
            }
        }

        internal void InvokeMember(string key) {
            foreach (var task in Tasks) {
                if (task.Key == key) {
                    var objectTask = GetInstance(task.ClassName);
                    objectTask.GetType().InvokeMember(task.MethodName, BindingFlags.InvokeMethod, null, objectTask, null);
                    return;
                }
            }
            throw new InvalidOperationException(string.Format("Unknow task '{0}'", key));
        }

        private object GetInstance(string className) {
            if (!instances.ContainsKey(className)) {
                var objectTask = CompiledAssembly.CreateInstance(className);
                instances.Add(className, objectTask);
            }
            return instances[className];
        }
    }

    public static class StringArrayExtender {
        public static string Last(this string[] array) {
            return array[array.LastIndex()];
        }

        public static int LastIndex(this string[] array) {
            if (array.Length == 0) return 0;
            return array.Length - 1;
        }
    }
}
