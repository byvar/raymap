using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class SuperObjectDynamic : ROMStruct {
		// Size: 12
		public Reference<Perso> perso;
		public ushort transformIndex;
		public ushort flags;
		public ushort unkByte; // read as byte?
		public uint unk;
		public ROMTransform transform;

		protected override void ReadInternal(Reader reader) {
			//Loader.print(Pointer.Current(reader));
			perso = new Reference<Perso>(reader);
			transformIndex = reader.ReadUInt16();
			flags = reader.ReadUInt16();
			unkByte = reader.ReadUInt16();
			unk = reader.ReadUInt32();


			transform = new ROMTransform(transformIndex);
			perso.Resolve(reader);
		}

		public GameObject GetGameObject() {
			if (perso.Value != null) {
				GameObject gao = new GameObject("SOD @ " + Offset);
				ROMPersoBehaviour rpb = perso.Value.GetGameObject(gao);
				return gao;
			}
			return null;
		}
	}
}
