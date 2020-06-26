using OpenSpace.PS1.GLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public interface IPS1PolygonCollide {
		CollideMaterial Material { get; }
		Pointer Offset { get; }
	}
}
