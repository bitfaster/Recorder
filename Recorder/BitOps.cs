using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Recorder
{
    public class BitOps
    {
        public static int CeilingPowerOfTwo(int x)
        {
            return (int)CeilingPowerOfTwo((uint)x);
        }

        public static uint CeilingPowerOfTwo(uint x)
        {
#if NETSTANDARD2_0
            // https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
            --x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
#else
            return 1u << -BitOperations.LeadingZeroCount(x - 1);
#endif
        }
    }
}
