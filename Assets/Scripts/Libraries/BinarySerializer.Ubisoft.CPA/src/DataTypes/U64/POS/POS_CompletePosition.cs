using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class POS_CompletePosition : U64_Struct, MAT_ITransform {
		public U64_Index<U64_TripledIndex> Index { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Index = s.SerializeObject<U64_Index<U64_TripledIndex>>(Index, name: nameof(Index)); // TODO: Set action
		}

		#region Properties
		public MTH3D_Matrix RotationMatrix {
			get {
				var val = Index.Value;
				if(val == null) return null;
				U64_Index<MTH3D_Matrix> mat = new U64_Index<MTH3D_Matrix>(Context, val.Index0); // TODO: set action
				return mat.Value;
			}
			set => throw new NotImplementedException();
		}
		public MTH3D_Matrix ScaleMatrix {
			get {
				var val = Index.Value;
				if (val == null) return null;
				U64_Index<MTH3D_Matrix> mat = new U64_Index<MTH3D_Matrix>(Context, val.Index1); // TODO: set action
				return mat.Value;
			}
			set => throw new NotImplementedException();
		}
		public MTH3D_Vector TranslationVector {
			get {
				var val = Index.Value;
				if (val == null) return null;
				U64_Index<MTH3D_Vector> mat = new U64_Index<MTH3D_Vector>(Context, val.Index2); // TODO: set action
				return mat.Value;
			}
			set => throw new NotImplementedException();
		}
		#endregion

		#region MAT_ITransform implementation
		public CPA.MTH3D_Vector Position {
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public MTH4D_Vector Rotation {
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public CPA.MTH3D_Vector Scale {
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		#endregion
	}
}
