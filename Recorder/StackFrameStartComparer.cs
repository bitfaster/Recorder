using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Recorder
{
    public class StackFrameStartComparer : IComparer<StackFrame>
    {
        public int Compare(StackFrame? x, StackFrame? y)
        {
            if (x?.Start == y?.Start)
                return 0;
            if ((x?.Start < y?.Start))
                return -1;

            return 1;
        }
    }
}
