using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Unahi.CRake {
    class Program {
        static void Main(string[] args) {
            CRake.Run(args);
        }
    }

    #region Framework

    public abstract class Base<T> where T: class, new() {
        public Base() {
            Arguments = new Arguments();
        }

        internal Arguments Arguments { get; set; }
        public abstract void Init();
        public abstract void Run();

        public static void Run(string[] args) {
            if (Instance != null) {
                throw new InvalidOperationException("Already running...");
            }
            var console = new T() as Base<T>;
            Instance = console as T;
            console.Init();
            console.Arguments.Parse(args);
            console.Run();
            Instance = null;
        }

        public static T Instance { get; private set; }
    }

    public enum ArgumentType {
        Bit,
        String
    }

    public class Argument {
        public char? Abbreviation { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public string Description { get; set; }
        public ArgumentType Type { get; set; }
    }

    public class Arguments {
        public Arguments() {
            // Always support help
            AddArgument('h', "help", "Display help message.", ArgumentType.Bit, false);
        }

        private List<Argument> list = new List<Argument>();
        private List<string> valueList = new List<string>();
        private Dictionary<string,string> keyValueList = new Dictionary<string,string>();

        public void AddArgument(char? abbr, string name, string description, ArgumentType type, object defaultValue) {
            list.Add(new Argument() {
                Abbreviation = abbr,
                Description = description,
                Name = name,
                Type = type,
                Value = defaultValue
            });
        }

        public object this[string argument]{
            get {
                var arg = GetArgument(argument);
                if (arg != null) {
                    return arg.Value;
                }

                if (keyValueList.Keys.Contains(argument)) {
                    return keyValueList[argument];
                }
                return null;
            }
        }

        public List<string> Values {
            get { return valueList; }
        }

        public bool IsHelp {
            get {
                return (bool)this["h"];
            }
        }

        public string ToHelp(string example) {
            var sorted = new SortedList<string, Argument>();
            foreach (var item in list) {
                string key;
                key = item.Abbreviation.HasValue && item.Abbreviation != ' ' ? "-" + item.Abbreviation : string.Empty;
                if (key.Length > 0 && !string.IsNullOrEmpty(item.Name)) key += ", ";
                if (!string.IsNullOrEmpty(item.Name)) key += "--" + item.Name;
                sorted.Add(key, item);
            }
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(example)) {
                builder.Append("Example: ");
                builder.Append(example);
                builder.AppendLine();
                builder.AppendLine();
            }
            builder.AppendLine("Options:");
            builder.AppendLine();
            foreach (var item in sorted) {
                var line = string.Format("\t{0}\t{1}", item.Key, item.Value.Description);
                builder.AppendLine(line);
            }
            return builder.ToString(); ;
        }

        internal void Parse(string[] args) {
            string arguments = string.Join(" ", args);
            for (int i = 0; i < args.Length; i++) {
                var arg = args[i];
                if (arg.StartsWith("-")) {
                    var cleanArg = arg.Remove(0, arg.StartsWith("--") ? 2 : 1);
                    var argument = GetArgument(cleanArg);
                    if (argument != null) {
                        object value;
                        if (argument.Type == ArgumentType.String) {
                            value = args[++i];
                        } else {
                            value = true;
                        }
                        argument.Value = value;
                        continue;
                    }
                    throw new ArgumentException(string.Format("Unknow argument '{0}'", arg));
                }
                var match = Regex.Match(arg, "^(.+?)=['\"]?(.+?)['\"]?$", RegexOptions.IgnoreCase);
                if (match.Success) {
                    keyValueList.Add(match.Groups[1].Value, match.Groups[2].Value);
                } else {
                    valueList.Add(arg);
                }
            }
        }

        private Argument GetArgument(string key) {
            foreach (var arg in list) {
                if ((arg.Abbreviation.HasValue && arg.Abbreviation.ToString() == key) || arg.Name == key) {
                    return arg;
                }
            }
            return null;
        }
    }

    #endregion
}
