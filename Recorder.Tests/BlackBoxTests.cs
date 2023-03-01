using System.Diagnostics;

namespace Recorder.Tests
{
    public class BlackBoxTests
    {
        [Fact]
        public void Test1()
        {
            var b = new BlackBox();
            b.Name = "foo";

            using var x = b.Capture("x");
            using (var y = b.Capture("y"))
            { }
                
            using var z = b.Capture("z");

            b.End();
        }

        [Fact]
        public void BufferTest()
        {
            var buffer = new RingBuffer<StackFrame>(4);

            buffer.Add(new StackFrame { Name = "foo" });
            buffer.Add(new StackFrame { Name = "foo" });
            //buffer.Add(new StackFrame { Name = "foo" });
            //buffer.Add(new StackFrame { Name = "foo" });
            //buffer.Add(new StackFrame { Name = "foo" });

            var data = buffer.Get();
        }
    }
}