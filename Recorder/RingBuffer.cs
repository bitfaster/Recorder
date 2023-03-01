using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder
{
    public class RingBuffer<T>
    {
        private int currentIndex = -1;
        private readonly T[] data;
        private readonly int mask;

        public RingBuffer(int size)
        { 
            size = BitOps.CeilingPowerOfTwo(size);
            this.data = new T[size];
            this.mask = size - 1;
        }

        public bool HasData => currentIndex != -1;

        public void Add(T item) 
        { 
            var index = Interlocked.Increment(ref currentIndex) & this.mask;

            this.data[index] = item;
        }

        public IEnumerable<T> Get() 
        {
            if (currentIndex == -1)
            { 
                return Array.Empty<T>();
            }

            if (currentIndex < mask)
            {
                return data.Take(currentIndex + 1);
            }

            return data; 
        }

        public void Clear() 
        {
            currentIndex = -1;
        }
    }
}
