using System;
using System.Collections.Generic;
using System.Text;

namespace Crake {
    public class CrakeParameters {
        static CrakeParameters() {
            PARAMS = new CrakeParameters();
        }

        public CrakeParameters() {
            list = new Dictionary<string, string>();
        }

        public static CrakeParameters PARAMS {
            get;
            private set;
        }

        private Dictionary<string, string> list;

        public void AddParameter(string key, string value) {
            list.Add(key, value);
        }

        public string this[string key] {
            get {
                if (list.ContainsKey(key)) {
                    return list[key];
                }
                return string.Empty;
            }
            internal set {
                if (list.ContainsKey(key)) {
                    list[key] = value;
                } else {
                    list.Add(key, value);
                }
            }
        }
    }
}
