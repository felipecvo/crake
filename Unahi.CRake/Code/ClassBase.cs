using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unahi.CRake.Code {
    public class ClassBase {
        public string this[string key] {
            get {
                return CRake.Instance.Arguments[key] as string;
            }
        }
    }
}
