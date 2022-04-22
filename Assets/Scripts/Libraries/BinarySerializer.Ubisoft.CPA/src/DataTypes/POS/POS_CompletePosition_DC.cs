using System;

namespace BinarySerializer.Ubisoft.CPA {
	// Transform matrix for R2 DC engine versions
	public class POS_CompletePosition_DC : BinarySerializable, MAT_ITransform {
		public POS_TransformationType Type { get; set; }
		public MTH3D_Matrix RotationMatrix { get; set; }
		public MTH4D_Matrix TransformMatrix { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.Serialize<POS_TransformationType>(Type, name: nameof(Type));
			RotationMatrix = s.SerializeObject<MTH3D_Matrix>(RotationMatrix, name: nameof(RotationMatrix));
			TransformMatrix = s.SerializeObject<MTH4D_Matrix>(TransformMatrix, name: nameof(TransformMatrix));
		}

		#region MAT_ITransform implementation
		/// <summary>
		/// Position vector
		/// </summary>
		public MTH3D_Vector Position {
			get {
				if (Type == POS_TransformationType.Identity)
					return MTH3D_Vector.Zero;
				else
					return TransformMatrix.GetPosition();
			}
			set {
				TransformMatrix.SetPosition(value);
				if (value.IsUniform && value.X == 0f) {
					if (Type == POS_TransformationType.Translation)
						Type = POS_TransformationType.Identity;
				} else {
					if (Type == POS_TransformationType.Identity)
						Type = POS_TransformationType.Translation;
				}
			}
		}
		/// <summary>
		/// Rotation quaternion
		/// </summary>
		public MTH4D_Vector Rotation {
			get {
				switch (Type) {
					case POS_TransformationType.Rotation:
					case POS_TransformationType.Complete:
					case POS_TransformationType.Uninitialized:
						return RotationMatrix.GetRotation();
					default:
						return MTH4D_Vector.IdentityQuaternion;
				}
			}
			set {
				switch (Type) {
					case POS_TransformationType.Translation:
					case POS_TransformationType.Identity:
						Type = POS_TransformationType.Rotation;
						break;
				}
				throw new NotImplementedException();
			}
		}
		/// <summary>
		/// Scale vector
		/// </summary>
		public MTH3D_Vector Scale {
			get {
				switch (Type) {
					case POS_TransformationType.Complete:
						return TransformMatrix.GetScale();
					default:
						return MTH3D_Vector.One;
				}
			}
			set {
				if (value.IsUniform && value.X == 1f) {
					// remove scale type if it's a (1,1,1) scale
					if (Type == POS_TransformationType.Complete) Type = POS_TransformationType.Rotation;
				} else {
					Type = POS_TransformationType.Complete;
					throw new NotImplementedException();
				}
			}
		}
		#endregion
	}
}
