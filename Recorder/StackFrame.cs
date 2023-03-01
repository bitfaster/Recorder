namespace Recorder
{
    public class StackFrame
    {
        public string Name { get; set; } = string.Empty;

        public long Start { get; set; }

        public long End { get; set; }

        public List<StackFrame> Children { get; } = new List<StackFrame>();
    }
}