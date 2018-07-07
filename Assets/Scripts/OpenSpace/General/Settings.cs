using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSpace {
    public class Settings {
        public enum EngineMode { R2, R3 };
        public enum Platform { PC, iOS, GC };
        public enum Endian { Little, Big };
        public enum LinkedListType { Single, Double };
        
        public EngineMode engineMode;
        public Platform platform;
        public Endian endian;
        public LinkedListType linkedListType;
        public bool hasNames = false;
        public bool hasDeformations = false;
        public int numEntryActions = 0;
        public bool isR2Demo = false;
        public bool isDonald = false;

        public bool IsLittleEndian {
            get { return endian == Endian.Little; }
        }


        public static Settings s = null;
        public static Settings R3PC = new Settings() {
            engineMode = EngineMode.R3,
            platform = Platform.PC,
            endian = Endian.Little,
            linkedListType = LinkedListType.Double,
            hasDeformations = true
        };

        public static Settings R3GC = new Settings() {
            engineMode = EngineMode.R3,
            platform = Platform.GC,
            endian = Endian.Big,
            linkedListType = LinkedListType.Double,
            hasNames = true,
            hasDeformations = true
        };

        public static Settings RAPC = new Settings() {
            engineMode = EngineMode.R3,
            platform = Platform.PC,
            endian = Endian.Little,
            linkedListType = LinkedListType.Double,
            hasDeformations = true
        };

        public static Settings RAGC = new Settings() {
            engineMode = EngineMode.R3,
            platform = Platform.GC,
            endian = Endian.Big,
            linkedListType = LinkedListType.Single,
            hasDeformations = true
        };

        public static Settings R2PC = new Settings() {
            engineMode = EngineMode.R2,
            platform = Platform.PC,
            endian = Endian.Little,
            numEntryActions = 43,
            linkedListType = LinkedListType.Double,
        };

        public static Settings R2PCDemo1 = new Settings() {
            engineMode = EngineMode.R2,
            platform = Platform.PC,
            endian = Endian.Little,
            linkedListType = LinkedListType.Double,
            numEntryActions = 1,
            isR2Demo = true
        };

        public static Settings R2PCDemo2 = new Settings() {
            engineMode = EngineMode.R2,
            platform = Platform.PC,
            endian = Endian.Little,
            numEntryActions = 7,
            linkedListType = LinkedListType.Double,
            isR2Demo = true
        };

        public static Settings R2IOS = new Settings() {
            engineMode = EngineMode.R2,
            platform = Platform.iOS,
            endian = Endian.Little,
            numEntryActions = 43,
            linkedListType = LinkedListType.Double
        };

        public static Settings DDPC = new Settings() {
            engineMode = EngineMode.R2,
            platform = Platform.PC,
            endian = Endian.Little,
            numEntryActions = 44,
            linkedListType = LinkedListType.Double,
            isDonald = true
        };
    }
}