using Newtonsoft.Json;
using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Object.Properties {
    public struct ObjectListEntry {
        public LegacyPointer off_scale;
        public LegacyPointer off_po;
        public uint thirdvalue;
        public ushort unk0;
        public ushort unk1;
        public uint lastvalue;

        public Vector3? scale;
        public PhysicalObject po;
    }
}
