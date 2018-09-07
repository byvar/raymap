using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSpace {
    class PointerException : Exception {

        string _excludeFromStackTrace;
        string message;

        public PointerException(string message, string excludeFromStackTrace) {
            this.message = message;
            _excludeFromStackTrace = excludeFromStackTrace;
        }

        public override string Message {
            get { return message; }
        }

        public override string StackTrace {
            get {
                List<string> stackTrace = new List<string>();
                stackTrace.AddRange(base.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
                stackTrace.RemoveAll(x => x.Contains(_excludeFromStackTrace));
                return string.Join(Environment.NewLine, stackTrace.ToArray());
            }
        }
    }
}