using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Type = OpenSpace.Object.SuperObject.Type;

namespace OpenSpace.PS1 {
	public class AlwaysItem : OpenSpaceStruct {
		public Pointer off_superObject;

		public uint uint_04;
		//public Pointer off_04;

		public uint uint_08;

		public uint uint_0C;
		//public Pointer off_0C;

		// Parsed
		public SuperObject superObject;

		protected override void ReadInternal(Reader reader) {
			off_superObject = Pointer.Read(reader);
			uint_04 = reader.ReadUInt32();
			uint_08 = reader.ReadUInt32();
			uint_0C = reader.ReadUInt32();

			superObject = Load.FromOffsetOrRead<SuperObject>(reader, off_superObject);
		}

		public GameObject GetGameObject() {
			GameObject gao = superObject.GetGameObject();
			/*if (gao != null) {
				gao.name = 
			}*/
			return gao;
		}
	}
}
