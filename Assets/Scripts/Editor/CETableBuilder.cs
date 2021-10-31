using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenSpace;

// Util class to build cheatengine tables
namespace Assets.Scripts.Editor {
    public class CETableBuilder {

        public enum CEVarType
        {
            Binary,
            Byte,
            _2_Bytes,
            _4_Bytes,
            _8_Bytes,
            Float,
            Double,
            String,
            Array_of_byte,
        }

        private static string VarTypeToString(CEVarType type)
        {
            return type.ToString().Replace('_', ' ').Trim();
        }

        public struct TableEntry
        {
            public string Name;
            public Pointer Address;
            public CEVarType Type;
            public bool ShowAsHex;
            public List<TableEntry> Children;

            public TableEntry(string name, Pointer address, CEVarType type, bool showAsHex) : this()
            {
                Name = name;
                Address = address;
                Type = type;
                ShowAsHex = showAsHex;
            }

            public string Build(CETableBuilder builder)
            {
                string children = Children!=null ? string.Join(string.Empty,Children.Select(c => c.Build(builder))) : "";

                return $"<CheatEntry>" +
                       $"<ID>{builder.GetNextID()}</ID>" +
                       $"<Description>{Name}</Description>" +
                       $"<VariableType>{VarTypeToString(Type)}</VariableType>" +
                       $"<ShowAsHex>{(ShowAsHex?"1":"0")}</ShowAsHex>" +
                       $"<Address>{Address.AbsoluteOffset:X8}</Address>" +
                       $"<CheatEntries>{children}</CheatEntries>" +
                       $"</CheatEntry>";
            }
        }

        public List<TableEntry> Entries = new List<TableEntry>();
        private int _id;

        public int GetNextID()
        {
            return _id++;
        }

        public string Build()
        {
            _id = 0;
            string result = "";
            foreach (var e in Entries) {
                result += e.Build(this);
            }
            return $"<?xml version=\"1.0\" encoding=\"utf-8\"?><CheatTable><CheatEntries>{result}</CheatEntries></CheatTable>";
        }
    }
}
