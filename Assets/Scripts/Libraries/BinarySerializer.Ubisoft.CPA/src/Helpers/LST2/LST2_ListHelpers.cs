using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
    public class LST2_ListHelpers {
		// Corrects pointers in a LST2_List broken due to no/incorrect Fix->Level pointers in <CPA3 games
		public static void Validate(SerializerObject s, Pointer lastElement, Pointer father, LST2_ListType type,
			uint nextOffset = 0, uint prevOffset = 4, uint fatherOffset = 8, Pointer next = null) {
			if (type != LST2_ListType.DoubleLinked) return;
			
			Pointer currentElement = lastElement;
			Pointer nextElement = next;
			while (currentElement != null) {
				s.DoAt(currentElement, () => {
					currentElement.File.AddOverridePointer(currentElement.AbsoluteOffset + nextOffset, nextElement);
					if(father != null)
						currentElement.File.AddOverridePointer(currentElement.AbsoluteOffset + fatherOffset, father);

					nextElement = currentElement;
					s.Goto(currentElement + prevOffset);
					currentElement = s.SerializePointer(default, allowInvalid: true, name: "LST2_Validate_Previous");
				});
			}
		}
    }
}
