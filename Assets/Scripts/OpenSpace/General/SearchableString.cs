using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace {
    public class SearchableString {
        public string String;
        public GameObject RelatedGameObject;
        public string LocationString;

        public SearchableString(string String, GameObject RelatedGameObject, string LocationString)
        {
            this.String = String ?? "";
            this.RelatedGameObject = RelatedGameObject;
            this.LocationString = LocationString;
        }
    }
}
