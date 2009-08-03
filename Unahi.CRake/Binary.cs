using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unahi.CRake {
    public class Binary {
        public Binary() {
            Tasks = new List<CompiledTask>();
        }

        public List<CompiledTask> Tasks { get; set; }
    }
}
