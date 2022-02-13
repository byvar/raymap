using UnityEngine;
using UnityEditor;
namespace OpenSpace
{

    public class Dynam // sizeof is 0xC
    {

        // Raw data
        public LegacyPointer offset;
        public LegacyPointer off_dynamics;

        // Processed data
        public Dynamics dynamics;

        public Dynam(LegacyPointer offset)
        {
            this.offset = offset;
        }

        public static Dynam Read(Reader reader, LegacyPointer offset) {
            Dynam dynam = new Dynam(offset);
			//MapLoader.Loader.print("Dynam " + offset);

            dynam.off_dynamics = LegacyPointer.Read(reader);

            if (dynam.off_dynamics != null) {

                LegacyPointer.Goto(ref reader, dynam.off_dynamics);
                dynam.dynamics = Dynamics.Read(reader, dynam.off_dynamics);

            }

            return dynam;
        }
    }

}