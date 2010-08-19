using System;
using System.Collections.Generic;
using System.Text;
using Crake.Parser;
using System.Text.RegularExpressions;

namespace Crake.Parser {
    public class EmptyKeyword : IKeyword {
        #region IKeyword Members

        public string[] IsMatch(IParsedObject parent, ref System.IO.StreamReader file, string text) {
            var match = Regex.Match(text, "^\\s*$");
            if (match.Success) {
                return new string[] { string.Empty };
            }
            return null;
        }

        public string Parse(IParsedObject parent, ref System.IO.StreamReader file, params object[] parts) {
            return string.Empty;
        }

        #endregion
    }
}
