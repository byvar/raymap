using System;

namespace BinarySerializer.Ubisoft.CPA {
	// Transform matrix for R2 PS2 engine versions
	public class MAT_Transformation_R2PS2 : BinarySerializable, MAT_ITransform {
		public MTH4D_Matrix TransformMatrix { get; set; }
		public MTH4D_Vector ScaleVector { get; set; }
		public MAT_TransformationType Type { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			TransformMatrix = s.SerializeObject<MTH4D_Matrix>(TransformMatrix, name: nameof(TransformMatrix));
			ScaleVector = s.SerializeObject<MTH4D_Vector>(ScaleVector, name: nameof(ScaleVector));
			Type = s.Serialize<MAT_TransformationType>(Type, name: nameof(Type));
			throw new NotImplementedException();

		}

		#region MAT_ITransform implementation
		/// <summary>
		/// Position vector
		/// </summary>
		public MTH3D_Vector Position {
			get {
				if (Type == MAT_TransformationType.Identity) 
					return MTH3D_Vector.Zero;
				else
					return TransformMatrix.GetPosition();
			}
			set {
				TransformMatrix.SetPosition(value);
				if (value.IsUniform && value.X == 0f) {
					if (Type == MAT_TransformationType.Translation)
						Type = MAT_TransformationType.Identity;
				} else {
					if (Type == MAT_TransformationType.Identity)
						Type = MAT_TransformationType.Translation;
				}
			}
		}

		/// <summary>
		/// Rotation quaternion
		/// </summary>
		public MTH4D_Vector Rotation {
			get {
				switch (Type) {
					case MAT_TransformationType.Rotation:
						return TransformMatrix.GetRotation();
					case MAT_TransformationType.RotationZoom:
					case MAT_TransformationType.Uninitialized:
					case MAT_TransformationType.RotationScale:
					case MAT_TransformationType.ComplexRotationScale:
						return TransformMatrix.GetRotation(Scale);
					default:
						return MTH4D_Vector.IdentityQuaternion;
				}
			}
			set {
				switch (Type) {
					case MAT_TransformationType.Scale:
						Type = MAT_TransformationType.RotationScale;
						break;
					case MAT_TransformationType.Zoom:
						Type = MAT_TransformationType.RotationZoom;
						break;
					case MAT_TransformationType.Translation:
					case MAT_TransformationType.Identity:
						Type = MAT_TransformationType.Rotation;
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
					case MAT_TransformationType.Zoom:
					case MAT_TransformationType.RotationZoom:
						var zoom = Zoom.Value;
						return new MTH3D_Vector(zoom, zoom, zoom);
					case MAT_TransformationType.Scale:
					case MAT_TransformationType.RotationScale:
					case MAT_TransformationType.ComplexRotationScale:
						return new MTH3D_Vector(ScaleVector.X, ScaleVector.Y, ScaleVector.Z);
					case MAT_TransformationType.Uninitialized:
					default:
						return MTH3D_Vector.One;
				}
			}
			set {
				if (value.IsUniform) {
					// Set zoom value - will remove scale type if it's a (1,1,1) scale
					Zoom = value.X;
				} else {
					if (Type == MAT_TransformationType.Identity)
						Type = MAT_TransformationType.Scale;
					throw new NotImplementedException();
				}
			}
		}

		/// <summary>
		/// Zoom value for uniform scales
		/// </summary>
		public float? Zoom {
			get {
				switch (Type) {
					case MAT_TransformationType.Zoom:
					case MAT_TransformationType.RotationZoom:
						return ScaleVector.X;
					default:
						return null;
				}
			}
			set {
				if (value.HasValue && value != 1f) {
					switch (Type) {
						case MAT_TransformationType.Rotation:
						case MAT_TransformationType.RotationZoom:
						case MAT_TransformationType.RotationScale:
						case MAT_TransformationType.ComplexRotationScale:
							Type = MAT_TransformationType.RotationZoom;
							break;
						case MAT_TransformationType.Uninitialized:
							break;
						default:
							Type = MAT_TransformationType.Zoom;
							break;
					}
					ScaleVector.X = value.Value;
				} else {
					switch (Type) {
						case MAT_TransformationType.Zoom:
						case MAT_TransformationType.Scale:
							Type = MAT_TransformationType.Translation;
							break;
						case MAT_TransformationType.RotationZoom:
						case MAT_TransformationType.RotationScale:
						case MAT_TransformationType.ComplexRotationScale:
							Type = MAT_TransformationType.Rotation;
							break;
					}
				}
			}
		}
		#endregion
	}
}
