using UnityEngine;

namespace LibR3 {
    /// <summary>
    /// Everything a superobject can contain.Gao gives the unity GameObject representation of it.
    /// </summary>
    public interface IR3Data {
        GameObject Gao {
            get;
        }

        R3SuperObject SuperObject {
            get;
        }
    }
}
