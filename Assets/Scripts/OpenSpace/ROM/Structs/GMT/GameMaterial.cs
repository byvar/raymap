using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class GameMaterial : ROMStruct {
		public Reference<VisualMaterial> visualMaterial;
		public Reference<MechanicsMaterial> mechanicsMaterial;
		public Reference<CollideMaterial> collideMaterial;
		public ushort soundMaterial;

		protected override void ReadInternal(Reader reader) {
			visualMaterial = new Reference<VisualMaterial>(reader, true);
			mechanicsMaterial = new Reference<MechanicsMaterial>(reader, true);
			collideMaterial = new Reference<CollideMaterial>(reader, true);
			soundMaterial = reader.ReadUInt16();
		}
	}
}
