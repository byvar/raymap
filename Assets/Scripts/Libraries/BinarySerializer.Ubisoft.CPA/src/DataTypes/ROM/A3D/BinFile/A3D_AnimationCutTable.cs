using System;
using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA.ROM {
	public class A3D_AnimationCutTable : BinarySerializable {
		public A3D_AnimationCut[] Cuts { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			var anims = Context.GetStoredObject<A3D_AnimationsFile>(A3D_AnimationsFile.ContextKey);
			Cuts = s.SerializeObjectArray<A3D_AnimationCut>(Cuts, anims.AnimationsCount, name: nameof(Cuts));
		}

		public void ReadData(SerializerObject s, ushort index) {
			if (index != 0xFFFF && index < Cuts.Length) {
				var anims = Context.GetStoredObject<A3D_AnimationsFile>(A3D_AnimationsFile.ContextKey);
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