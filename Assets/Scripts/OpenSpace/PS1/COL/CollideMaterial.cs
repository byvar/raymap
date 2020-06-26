using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Colors = OpenSpace.Collide.CollideMaterial.Colors;

namespace OpenSpace.PS1 {
	public class CollideMaterial : IEquatable<CollideMaterial> {
		public byte flag0;
        public byte identifier;

        public override bool Equals(System.Object obj) {
            return obj is CollideMaterial && this == (CollideMaterial)obj;
        }
        public override int GetHashCode() {
            return flag0.GetHashCode() ^ identifier.GetHashCode();
        }

        public bool Equals(CollideMaterial other) {
            return this == (CollideMaterial)other;
        }

        public static bool operator ==(CollideMaterial x, CollideMaterial y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.flag0 == y.flag0 && x.identifier == y.identifier;
        }
        public static bool operator !=(CollideMaterial x, CollideMaterial y) {
            return !(x == y);
        }

        public Material CreateMaterial() {
            Material mat = new Material(MapLoader.Loader.collideMaterial); ;
            if (NoCollision) {
                mat = MapLoader.Loader.collideTransparentMaterial;
                //mat.SetTexture("_MainTex", Util.CreateDummyCheckerTexture());
                mat.color = Colors.NoCollision;
            }
            if (Slide) mat.color = Colors.Slide;
            if (Water) {
                mat = MapLoader.Loader.collideTransparentMaterial;
                //mat.SetTexture("_MainTex", Util.CreateDummyCheckerTexture());
                mat.color = Colors.Water;
            }
            if (ClimbableWall || HangableCeiling) {
                mat.color = Colors.Climbable;
            }
            if (LavaDeathWarp || DeathWarp) {
                mat.color = Colors.DeathWarp;
            }
            if (HurtTrigger) mat.color = Colors.HurtTrigger;
            if (FallTrigger) mat.color = Colors.FallTrigger;
            if (Trampoline) mat.color = Colors.Trampoline;
            if (Electric) mat.color = Colors.Electric;
            if (Wall) mat.color = Colors.Wall;
            if (GrabbableLedge) mat.color = Colors.GrabbableLedge;
            if (FlagUnknown || FlagUnk2 || FlagUnk3) mat.color = Colors.FlagUnknown;
            return mat;
        }

        public bool Slide { get { return GetFlag(00); } set { SetFlag(00, value); } }
        public bool Trampoline { get { return GetFlag(01); } set { SetFlag(01, value); } }
        public bool GrabbableLedge { get { return GetFlag(02); } set { SetFlag(02, value); } }
        public bool Wall { get { return GetFlag(03); } set { SetFlag(03, value); } }
        public bool FlagUnknown { get { return GetFlag(04); } set { SetFlag(04, value); } }
        public bool HangableCeiling { get { return GetFlag(05); } set { SetFlag(05, value); } }
        public bool ClimbableWall { get { return GetFlag(06); } set { SetFlag(06, value); } }
        public bool Electric { get { return GetFlag(07); } set { SetFlag(07, value); } }
        public bool LavaDeathWarp { get { return GetFlag(08); } set { SetFlag(08, value); } }
        public bool FallTrigger { get { return GetFlag(09); } set { SetFlag(09, value); } }
        public bool HurtTrigger { get { return GetFlag(10); } set { SetFlag(10, value); } }
        public bool DeathWarp { get { return GetFlag(11); } set { SetFlag(11, value); } }
        public bool FlagUnk2 { get { return GetFlag(12); } set { SetFlag(12, value); } }
        public bool FlagUnk3 { get { return GetFlag(13); } set { SetFlag(13, value); } }
        public bool Water { get { return GetFlag(14); } set { SetFlag(14, value); } }
        public bool NoCollision { get { return GetFlag(15); } set { SetFlag(15, value); } }

        public void SetFlag(int index, bool value) {
            BitArray bitArray = new BitArray(BitConverter.GetBytes(identifier));
            bitArray.Set(index, value);
            byte[] array = new byte[1];
            bitArray.CopyTo(array, 0);
            this.identifier = array[0];
        }

        public bool GetFlag(int index) {
            BitArray bitArray = new BitArray(BitConverter.GetBytes(identifier));
            return bitArray.Get(index);
        }
    }
}
