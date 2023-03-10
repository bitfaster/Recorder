
using System.Text.Json;

namespace Recorder
{
    public class SpeedscopeWriter : IDisposable
    {
        private readonly Dictionary<string, int> frameMap;
        private readonly Utf8JsonWriter jsonWriter;

        public SpeedscopeWriter(Stream stream)
        {
            this.jsonWriter = new Utf8JsonWriter(stream);
            this.frameMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public void WritePreAmble()
        {
            jsonWriter.WriteStartObject();
            WritePreamble(jsonWriter);

            jsonWriter.WritePropertyName("profiles");
            jsonWriter.WriteStartArray();
        }

        public void WriteEvent(StackFrame stack)
        {
            BuildFrameMap(stack);

            var c = new Clock(stack.Start);
            jsonWriter.WriteStartObject();

            jsonWriter.WritePropertyName("events");
            jsonWriter.WriteStartArray();
            WriteEventTree(jsonWriter, stack, c);
            jsonWriter.WriteEndArray();

            WriteEventPreamble(jsonWriter, stack, c);
            jsonWriter.WriteEndObject();
        }

        public void Flush()
        {
            jsonWriter.WriteEndArray();

            WriteFrames(jsonWriter);

            jsonWriter.WriteEndObject();
        }

        private void BuildFrameMap(StackFrame root)
        {
            if (!this.frameMap.ContainsKey(root.Name))
            {
                this.frameMap.Add(root.Name, frameMap.Count);
            }

            foreach (var c in root.Children)
            {
                this.BuildFrameMap(c);
            }
        }

        //"version":"0.0.1",
        //"$schema":"https://www.speedscope.app/file-format-schema.json",
        private static void WritePreamble(Utf8JsonWriter json)
        {
            json.WritePropertyName("version");
            json.WriteStringValue("0.0.1");
            json.WritePropertyName("$schema");
            json.WriteStringValue("https://www.speedscope.app/file-format-schema.json");
        }

        //"shared":
        //{"frames":
        //	[{"name":"w3wp.exe"},{"name":"a"},{"name":"b"},{"name":"c"}]
        //},
        private void WriteFrames(Utf8JsonWriter json)
        {
            json.WritePropertyName("shared");
            json.WriteStartObject();

            json.WritePropertyName("frames");
            json.WriteStartArray();

            foreach (var n in frameMap.Keys)
            {
                json.WriteStartObject();
                json.WritePropertyName("name");
                json.WriteStringValue(n);
                json.WriteEndObject();
            }

            json.WriteEndArray();
            json.WriteEndObject();
        }

        //"profiles":[
        //{

        //"type":"evented",
        //"name":"foo",
        //"unit":"milliseconds",
        //"startValue":0,
        //"endValue":0.0,
        private void WriteEventPreamble(Utf8JsonWriter json, StackFrame root, Clock c)
        {
            json.WritePropertyName("type");
            json.WriteStringValue("evented");

            json.WritePropertyName("name");
            json.WriteStringValue(root.Name);

            json.WritePropertyName("unit");
            json.WriteStringValue(Clock.Unit);

            json.WritePropertyName("startValue");
            json.WriteNumberValue(0);

            json.WritePropertyName("endValue");
            json.WriteNumberValue(c.Time(root.End));
        }

        //"events":[
        //	{"type":"O","frame":0,"at":0.0},
        //	{"type":"C","frame":0,"at":0.0}
        //	]
        private void WriteEventTree(Utf8JsonWriter json, StackFrame frame, Clock clock)
        {
            int frameNo = frameMap[frame.Name];

            json.WriteStartObject();
            json.WritePropertyName("type");
            json.WriteStringValue("O");
            json.WritePropertyName("frame");
            json.WriteNumberValue(frameNo);
            json.WritePropertyName("at");
            json.WriteNumberValue(clock.Time(frame.Start));
            json.WriteEndObject();

            foreach (var child in frame.Children)
            {
                WriteEventTree(json, child, clock);
            }

            json.WriteStartObject();
            json.WritePropertyName("type");
            json.WriteStringValue("C");
            json.WritePropertyName("frame");
            json.WriteNumberValue(frameNo);
            json.WritePropertyName("at");
            json.WriteNumberValue(clock.Time(frame.End));
            json.WriteEndObject();
        }
        //	}
        //	]

        public void Dispose()
        {
            this.jsonWriter.Dispose();
        }
    }
}
