using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unahi.CRake {
    public class CRakeParameters {
        static CRakeParameters() {
            PARAMS = new CRakeParameters();
        }

        public CRakeParameters() {
            list = new Dictionary<string, string>();
        }

        public static CRakeParameters PARAMS {
            get;
            private set;
        }

        private Dictionary<string, string> list;

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
