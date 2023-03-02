using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder
{
    public class BlackBox
    {
        private static readonly RingBuffer<StackFrame> history = new(16);

        private readonly StackFrame root;
        private readonly Stack<StackFrame> stack;

        public static IEnumerable<StackFrame> History 
        { 
            get 
            {
                // return in order by start time
                var copy = history.Get().ToList();
                copy.Sort(new StackFrameStartComparer());
                return copy;
            } 
        }

        public static bool HasHistory => history.HasData;

        public static void ClearHistory()
        {
            history.Clear();
        }

        public BlackBox()
        {
            this.root = new StackFrame() { Name = "root", Start = Stopwatch.GetTimestamp() };
            stack = new Stack<StackFrame>();
            stack.Push(root);

            history.Add(root);
        }

        public string Name { get { return this.root.Name; } set { this.root.Name = value; } }

        public IDisposable Capture(string name)
        { 
            var s = new StackFrame() { Name = name, Start = Stopwatch.GetTimestamp() };
            stack.Peek().Children.Add(s);

            stack.Push(s);
            return new Ender(stack);
        }

        public void End()
        {
            this.root.End = Stopwatch.GetTimestamp();
        }

        private struct Ender : IDisposable
        {
            public Ender(Stack<StackFrame> stack)
            {
                this.Stack = stack;
            }

            public Stack<StackFrame> Stack { get; set; }

            public void Dispose()
            {
                var s = Stack.Pop();
                s.End = Stopwatch.GetTimestamp();
            }
        }
    }
}
