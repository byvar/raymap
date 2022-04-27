using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Nintendo.DS {
    public enum DS3D_PrimitiveType : byte {
		Triangles      = 0, // 3*N vertices per N triangles
		Quads          = 1, // 4*N vertices per N quads
		TriangleStrips = 2, // 3+(N-1) vertices per N triangles. Nv = N-2t
		QuadStrips     = 3, // 4+(N-1)*2 vertices per N quads.   Nv = N/2-1t
	}
}
