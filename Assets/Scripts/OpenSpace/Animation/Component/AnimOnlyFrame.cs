using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimOnlyFrame : OpenSpaceStruct {
        public ushort quaternion;
        public ushort vector;
        public ushort num_hierarchies_for_frame;
        public ushort start_hierarchies_for_frame;
        public ushort unk8;
        public ushort deformation;
        public ushort numOfNTTO;

		protected override void ReadInternal(Reader reader) {
			quaternion = reader.ReadUInt16();
			vector = reader.ReadUInt16();
			num_hierarchies_for_frame = reader.ReadUInt16();
			start_hierarchies_for_frame = reader.ReadUInt16();
			if (CPA_Settings.s.hasDeformations) {
				if (CPA_Settings.s.game != CPA_Settings.Game.LargoWinch) {
					unk8 = reader.ReadUInt16();
				}
				deformation = reader.ReadUInt16();
			}
			numOfNTTO = reader.ReadUInt16();
			if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.TT) {
				unk8 = reader.ReadUInt16();
			}
		}

		/*public static int Size {
            get {
                switch (Settings.s.engineVersion) {
                    case Settings.EngineVersion.TT: return 12;
                    default:
						if (Settings.s.hasDeformations) {
							if (Settings.s.game == Settings.Game.LargoWinch) {
								return 12;
							} else {
								return 14;
							}
						} else {
							return 10;
						}
                }
            }
        }*/

		public static bool Aligned {
            get { return false; }
        }
    }
}
