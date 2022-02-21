using System;
using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class A3D_AnimationCutTable : BinarySerializable {
		public A3D_AnimationCut[] Cuts { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			var anims = s.GetLoader().AnimationsFile;
			Cuts = s.SerializeObjectArray<A3D_AnimationCut>(Cuts, anims.AnimationsCount, name: nameof(Cuts));
		}

		public void ReadData(SerializerObject s, ushort index) {
			if (index != 0xFFFF && index < Cuts.Length) {
				var anims = s.GetLoader().AnimationsFile;
				ushort currentIndex = index;
				while (currentIndex != 0xFFFF) {
					anims.LoadAnimation(s, currentIndex);
					currentIndex = Cuts[currentIndex].NextAnimation;
				}
			}
		}

		public A3D_AnimationCut[] GetAnimationChain(ushort index) {
			if (index != 0xFFFF && index < Cuts.Length) {
				var cuts = new List<A3D_AnimationCut>();
				ushort currentIndex = index;
				while (currentIndex != 0xFFFF) {
					cuts.Add(Cuts[currentIndex]);
					currentIndex = Cuts[currentIndex].NextAnimation;
				}
				return cuts.ToArray();
			}
			return null;
		}
	}
}