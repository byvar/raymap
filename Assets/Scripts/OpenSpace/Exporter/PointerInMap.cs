using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenSpace.Exporter {

    [TypeConverter(typeof(PointerInMapConverter))]
    public struct PointerInMap : IEquatable<PointerInMap> {
        public string Map;
        public uint Offset;

        public PointerInMap(string map, uint offset)
        {
            Map = map;
            Offset = offset;
        }

        public PointerInMap(string str)
        {
            var parts = str.Split('@');
            Map = parts[1];
            Offset = uint.Parse(parts[0], System.Globalization.NumberStyles.HexNumber);
        }

        public bool Equals(PointerInMap other)
        {
            return Map == other.Map && Offset == other.Offset;
        }

        public override bool Equals(object obj)
        {
            return obj is PointerInMap other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked {
                return ((Map != null ? Map.GetHashCode() : 0) * 397) ^ (int) Offset;
            }
        }

        public static bool operator ==(PointerInMap left, PointerInMap right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PointerInMap left, PointerInMap right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{Offset:x}@{Map}";
        }
    }

    public class PointerInMapConverter : TypeConverter {
        // Overrides the CanConvertFrom method of TypeConverter.
        // The ITypeDescriptorContext interface provides the context for the
        // conversion. Typically, this interface is used at design time to 
        // provide information about the design-time container.
        public override bool CanConvertFrom(ITypeDescriptorContext context,
            Type sourceType)
        {

            if (sourceType == typeof(string)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value is string str) {
                return new PointerInMap(str);
            }
            return base.ConvertFrom(context, culture, value);
        }

    }
}
