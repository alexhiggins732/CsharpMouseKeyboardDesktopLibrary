using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseKeyboardEvents
{
    public class BrowserEventFactory
    {
        static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public static MouseKeyEvent Parse(string json)
        {
            var result = JsonConvert.DeserializeObject<CompactBrowseEvent>(json, settings);
            var @event = (MouseKeyEvent)result.Data;
            return @event;
        }
    }
    public class CompactBrowseEvent
    {
        public int StorageType { get; set; }
        public uint[] Payload { get; set; }
        public ulong Data => Payload[0] | (((ulong)Payload[1]) << 32);
    }

    public class BrowserMouseEvent : CompactBrowseEvent
    {

    }
    public class BrowserKeyEvent : CompactBrowseEvent
    {

    }
    public enum BrowseEventType
    {
        //mousemove,
        //mousedown,
        //mouseup,
        //wheel,
        //keydown,
        //keyup
    }

}
