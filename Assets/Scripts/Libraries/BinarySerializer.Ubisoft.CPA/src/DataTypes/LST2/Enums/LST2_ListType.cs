using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public enum LST2_ListType {
		Static, // = change type upon serialize
		Dynamic, // = change type upon serialize
		DoubleLinked,
		SingleLinked,
		Array,
		SemiOptimized, // Not fully implemented, a mixture between array & double linked?
		Optimized,  // Array
	}
}
