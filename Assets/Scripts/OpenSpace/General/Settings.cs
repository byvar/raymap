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
        public bool hasExtraInputData = false;
        public bool hasMemorySupport = false;
        public Dictionary<string, uint> memoryAddresses = null;

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
            hasDeformations = true,
            hasExtraInputData = true
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
            hasMemorySupport = true,
            memoryAddresses = new Dictionary<string, uint> {
                { "actualWorld", 0x005013C8 },
                { "dynamicWorld", 0x00500FD0 },
                { "inactiveDynamicWorld", 0x00500FC4 },
                { "fatherSector", 0x00500FC0 },
                { "always", 0x004A6B18 },
                { "anim_stacks", 0x004A6B38 },
                { "anim_framesKF", 0x00500274 },
                { "anim_a3d", 0x00500278 },
                { "anim_channels", 0x0050027C },
                { "anim_frames", 0x00500280 },
                { "anim_hierarchies", 0x00500284 },
                { "anim_morphData", 0x00500288 },
                { "anim_keyframes", 0x0050028C },
                { "anim_onlyFrames", 0x00500290 },
                { "anim_vectors", 0x00500294 },
                { "anim_events", 0x00500298 },
                { "anim_NTTO", 0x0050029C },
                { "anim_quaternions", 0x005002A0 },
                { "engineStructure", 0x00500380 },
                { "families", 0x00500560 },
                { "objectTypes", 0x005013E0 }
            }
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
            linkedListType = LinkedListType.Double,
            hasExtraInputData = true
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