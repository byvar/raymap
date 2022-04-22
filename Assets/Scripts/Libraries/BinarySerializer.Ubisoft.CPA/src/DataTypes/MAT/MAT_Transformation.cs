using BinarySerializer.Ubisoft.CPA.PS1;
using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class MAT_Transformation : BinarySerializable, MAT_ITransform {
		public MAT_Transformation_R3 Transform_R3 { get; set; }
		public MAT_Transformation_R2PS2 Transform_R2PS2 { get; set; }
		public POS_CompletePosition_DC Transform_R2DC { get; set; }
		public POS_CompletePosition Transform_R2 { get; set; }
		public POS_CompletePosition_PS1 Transform_PS1 { get; set; }

		public MAT_ITransform SelectedTransform {
			get {
				if (Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					return Transform_R3;
				} else if (Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)) {
					return Transform_R2PS2;
				} else if (Context.GetCPASettings().Platform == Platform.DC) {
					return Transform_R2DC;
				} else if(Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_PS1)) {
					return Transform_PS1;
				} else {
					return Transform_R2;
				}
			}
		}

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				Transform_R3 = s.SerializeObject<MAT_Transformation_R3>(Transform_R3, name: nameof(Transform_R3));
			} else if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)) {
				Transform_R2PS2 = s.SerializeObject<MAT_Transformation_R2PS2>(Transform_R2PS2, name: nameof(Transform_R2PS2));
			} else if (s.GetCPASettings().Platform == Platform.DC) {
				Transform_R2DC = s.SerializeObject<POS_CompletePosition_DC>(Transform_R2DC, name: nameof(Transform_R2DC));
			} else if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_PS1)) {
				Transform_PS1 = s.SerializeObject<POS_CompletePosition_PS1>(Transform_PS1, name: nameof(Transform_PS1));
			} else {
				Transform_R2 = s.SerializeObject<POS_CompletePosition>(Transform_R2, name: nameof(Transform_R2));
			}
		}

		#region MAT_ITransform implementation
		public MTH3D_Vector Position { get => SelectedTransform.Position; set => SelectedTransform.Position = value; }
		public MTH4D_Vector Rotation { get => SelectedTransform.Rotation; set => SelectedTransform.Rotation = value; }
		public MTH3D_Vector Scale { get => SelectedTransform.Scale; set => SelectedTransform.Scale = value; }
		#endregion
	}
}
