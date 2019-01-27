using UnityEngine;
using UnityEditor;
namespace OpenSpace
{

    public class Dynam // sizeof is 0xC
    {

        // Raw data
        public Pointer offset;
        public Pointer off_dynamics;

        // Processed data
        public Dynamics dynamics;

        public Dynam(Pointer offset)
        {
            this.offset = offset;
        }

        public static Dynam Read(Reader reader, Pointer offset) {
            Dynam dynam = new Dynam(offset);
			MapLoader.Loader.print("Dynam " + offset);

            dynam.off_dynamics = Pointer.Read(reader);

            Pointer.Goto(ref reader, dynam.off_dynamics);
            dynam.dynamics = Dynamics.Read(reader, dynam.off_dynamics);

            return dynam;
        }
    }

}