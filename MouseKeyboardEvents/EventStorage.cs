using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;

namespace MouseKeyboardEvents
{
    /// <summary>
    /// The format to save and load the <see cref="MouseKeyEvent"/> from storage.
    /// </summary>
    [Flags]
    public enum StorageFormat
    {
        /// <summary>
        /// The storage format for the <see cref="MouseKeyEvent"/> is not specified.
        /// </summary>
        None,

        /// <summary>
        /// Save or load the <see cref="MouseKeyEvent"/> as human readable json strings taking up 400 bytes of storage per event. Use only for development and debugging purposes
        /// </summary>
        Json,

        /// <summary>
        /// Save or load the <see cref="MouseKeyEvent"/> in binary, stored as an 8 byte <see cref="ulong"/>, which is preferred over <see cref="Json"/> which uses up to 400 bytes, for high performance.
        /// </summary>
        Binary,

        /// <summary>
        /// Save or load the <see cref="MouseKeyEvent"/> in both <see cref="Json"/> and <see cref="Binary"/>.
        /// </summary>
        All
    }


    /// <summary>
    /// Stores and Loads <see cref="MouseKeyEvent"/> from storage.
    /// </summary>
    public class EventStorage
    {
        /// <summary>
        /// Saves the <paramref name="events"/> by calling <see cref="Save(IEnumerable{MouseKeyEvent}, string, StorageFormat)"/> using <see cref="StorageFormat.All"></see> 
        /// which saves the events as both <see cref="StorageFormat.Binary"/> and <see cref="StorageFormat.Json"/>.
        /// </summary>
        /// <param name="events">The <see cref="MouseKeyEvent"/> <see cref="IEnumerable{T}"/> to save. </param>
        /// <param name="fileName">The name of the file to use for storage. If not specifed the name will be generated
        /// using the format 'data-{ts}.json' where {ts} is the current date formatted as "yyyy-MM-dd_hh-mm-ss".</param>

        public static void Save(IEnumerable<MouseKeyEvent> events, string fileName = null)
        {
            if (fileName is null)
            {
                var ts = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
                fileName = $"data-{ts}";
            }
            Save(events, fileName, StorageFormat.All);

        }

        /// <summary>
        /// Saves the <paramref name="events"/> by calling  <see cref="SaveAsJson(IEnumerable{MouseKeyEvent}, string)"/> and/or 
        /// <see cref="SaveAsBinary(IEnumerable{MouseKeyEvent}, string)"/> depending the <see cref="StorageFormat"/> specifed in the <paramref name="format"/> parameter.
        /// </summary>
        /// <param name="events">The <see cref="MouseKeyEvent"/> <see cref="IEnumerable{T}"/> to save. </param>
        /// <param name="fileName">The name of the file to use for storage. If not specifed the name will be generated
        /// using the format 'data-{ts}.{ext}' where {ts} is the current date formatted as "yyyy-MM-dd_hh-mm-ss"
        /// and {ext} is either 'bin' or 'json' depending on whether <see cref="StorageFormat.Binary"/> or <see cref="StorageFormat.Json"/>, respectively, is specified </param>
        /// <param name="format">The <see cref="StorageFormat" /> to use for storage.</param>
        private static void Save(IEnumerable<MouseKeyEvent> events, string fileName, StorageFormat format)
        {
            if (((int)format & (int)StorageFormat.Json) == (int)StorageFormat.Json)
            {
                SaveAsJson(events, fileName);
            }
            if (((int)format & (int)StorageFormat.Binary) == (int)StorageFormat.Binary)
            {
                SaveAsBinary(events, fileName);
            }
        }

        /// <summary>
        /// Loads the <paramref name="events"/> stored as <see cref="StorageFormat.Json" />, which uses up to 400 bytes of storage per event. 
        /// This format is only recommended for development and debugging purposes.
        /// For high volume or performance critical operations use <see cref="SaveAsBinary(IEnumerable{MouseKeyEvent}, string)"/> instead.
        /// </summary>
        /// <param name="events">The <see cref="MouseKeyEvent"/> <see cref="IEnumerable{T}"/> to save. </param>
        /// <param name="fileName">The name of the file to use for storage. If not specifed the name will be generated
        /// using the format 'data-{ts}.json' where {ts} is the current date formatted as "yyyy-MM-dd_hh-mm-ss".</param>
        public static void SaveAsJson(IEnumerable<MouseKeyEvent> events, string fileName = null)
        {
            if (fileName is null)
            {
                var ts = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
                fileName = $"data-{ts}";
            }
            var json = JsonConvert.SerializeObject(events, Formatting.Indented);

            var name = Path.GetFileNameWithoutExtension(fileName);
            var nameWithExtension = $"{name}.json";
            var fi = new FileInfo(fileName);
            var dir = Directory.CreateDirectory(fi.Directory.FullName);
            fi = new FileInfo(Path.Combine(dir.FullName, nameWithExtension));
            var path = fi.FullName;
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Saves the <paramref name="events"/> stored using the 8 byte <see cref="ulong"/> per event <see cref="StorageFormat.Binary" /> format. 
        /// This is the preferred <see cref="StorageFormat" /> for high volume or performance critical storage operations.
        /// </summary>
        /// <param name="events">The <see cref="MouseKeyEvent"/> <see cref="IEnumerable{T}"/> to save. </param>
        /// <param name="fileName">The name of the file to use for storage. If not specifed the name will be generated 
        /// using the format 'data-{ts}.bin' where {ts} is the current date formatted as "yyyy-MM-dd_hh-mm-ss".</param>
        public static void SaveAsBinary(IEnumerable<MouseKeyEvent> events, string fileName = null)
        {
            if (fileName is null)
            {
                var ts = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
                fileName = $"data-{ts}";
            }
            
            var name = Path.GetFileNameWithoutExtension(fileName);
            var nameWithExtension = $"{name}.bin";
            var fi = new FileInfo(fileName);
            var dir = Directory.CreateDirectory(fi.Directory.FullName);
            fi = new FileInfo(Path.Combine(dir.FullName, nameWithExtension));
            var path = fi.FullName;
            /* legacy
            using (var fs = new FileStream(path, FileMode.Create))
            {
                using (var br = new BinaryWriter(fs))
                {
                    events.ForEach(evt => br.Write(evt.ToUlong()));
                }
            }
             */
            //TODO: Save from async buffered paged list of byte
            var bytes = events.SelectMany(evt => BitConverter.GetBytes((ulong)evt)).ToArray();
            File.WriteAllBytes(path, bytes);

        }

        /// <summary>
        /// Loads the <paramref name="events"/> stored as <see cref="StorageFormat.Json" />, which uses up to 400 bytes of storage per event. 
        /// This format is only recommended for development and debugging purposes.
        /// For high volume or performance critical operations use <see cref="SaveAsBinary(IEnumerable{MouseKeyEvent}, string)"/> instead.
        /// </summary>
        /// <param name="fileName">The name of the file to load from storage.</param>
        public static IEnumerable<MouseKeyEvent> Load(string fileName)
        {
            var file = new FileInfo(fileName);
            if (file.Extension.EndsWith("bin", StringComparison.OrdinalIgnoreCase))
                return LoadFromBinary(fileName);
            else if (file.Extension.EndsWith("json", StringComparison.OrdinalIgnoreCase))
                return LoadFromJson(fileName);
            else
                throw new Exception($"Invalid file type: '{file.Extension}'.");

        }

        /// <summary>
        /// Loads the <paramref name="events"/> stored as <see cref="StorageFormat.Json" />, which uses up to 400 bytes of storage per event. 
        /// This format is only recommended for development and debugging purposes.
        /// For high volume or performance critical operations use <see cref="SaveAsBinary(IEnumerable{MouseKeyEvent}, string)"/> instead.
        /// </summary>
        /// <param name="fileName">The name of the file to load from storage.</param>
        public static IEnumerable<MouseKeyEvent> LoadFromJson(string fileName)
        {
            //TODO: Impement async pages using internal yeild return.
            var json = File.ReadAllText(fileName);
            var JsonEvents = JsonConvert.DeserializeObject<List<MouseKeyEvent>>(json);
            return JsonEvents;
        }

        /// <summary>
        /// Loads the <paramref name="events"/> stored using the 8 byte <see cref="ulong"/> per event <see cref="StorageFormat.Binary" /> format. 
        /// This is the preferred <see cref="StorageFormat" /> for high volume or performance critical storage operations.
        /// </summary>
        /// <param name="fileName">The name of the file to load from storage.</param>
        public static IEnumerable<MouseKeyEvent> LoadFromBinary(string filePath)
        {
            /*
            legacy
            var result = new List<MacroEvent>();
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                using (var br = new BinaryReader(fs))
                {
                    var endOfStream = fs.Length;
                    while (fs.Position < endOfStream)
                    {
                        var raw = br.ReadUInt64();
                        var macroEvent = new MacroEvent(raw);
                        result.Add(macroEvent);
                    }
                }
            }
            return result;
             */
            var bytes = File.ReadAllBytes(filePath);
            int eventCount = bytes.Length / 8;

            /*double cast
            var macros = Enumerable
                .Range(0, events)
                .Select(i => BitConverter.ToUInt64(bytes, i))
                .Select(u => new MacroEvent(u))
                .ToList();
            return macros;
            */

            //TODO: Async Yield return  using internal paged list of events,;
            return Enumerable.Range(0, eventCount).Select(i => (MouseKeyEvent)BitConverter.ToUInt64(bytes, i)).ToList();




        }

    }
}

