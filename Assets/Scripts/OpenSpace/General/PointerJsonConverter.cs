using Newtonsoft.Json;
using OpenSpace.FileFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenSpace {
    public class PointerJsonConverter : JsonConverter {
        const string PointerPattern = @"^(?<file>.*)|0x(?<offset>[a-fA-F0-9]{8})(\[0x(?<offsetInFile>[a-fA-F0-9]{8})\])?$";
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(Pointer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (value == null) {
                writer.WriteNull();
            } else {
                writer.WriteValue(value.ToString());
            }
        }

        public override bool CanRead {
            get { return true; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            string str = reader.ReadAsString();
            if (str != null) {
                Match match = Regex.Match(str, PointerPattern);
                if (match.Success) {
                    string offset = match.Groups["offset"].Value;
                    string file = match.Groups["file"].Value;
                    FileWithPointers[] files_array = MapLoader.Loader.files_array;
                    if (uint.TryParse(offset, System.Globalization.NumberStyles.HexNumber, default, out uint result)) {
                        for (int i = 0; i < files_array.Length; i++) {
                            if (files_array[i] != null && files_array[i].name == file) {
                                return new Pointer(result, files_array[i]);
                            }
                        }
                    }
                }
            }
            return existingValue;
        }
    }
}
