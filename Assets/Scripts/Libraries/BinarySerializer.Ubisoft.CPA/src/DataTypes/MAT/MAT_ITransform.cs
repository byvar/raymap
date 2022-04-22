using System;

namespace BinarySerializer.Ubisoft.CPA {
	// Interface for all MAT_Transform versions
	public interface MAT_ITransform {
		/// <summary>
		/// Position vector
		/// </summary>
		public MTH3D_Vector Position { get; set; }

		/// <summary>
		/// Rotation quaternion
		/// </summary>
		public MTH4D_Vector Rotation { get; set; }

		/// <summary>
		/// Scale vector
		/// </summary>
		public MTH3D_Vector Scale { get; set; }
	}
}
