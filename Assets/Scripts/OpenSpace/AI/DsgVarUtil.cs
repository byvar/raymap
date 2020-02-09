using OpenSpace;
using OpenSpace.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.AI {

    public class DsgVarUtil {

        public static string DsgVarEntryToCSharpAssignment(DsgVar dsgVar, int index) {

            DsgVarInfoEntry dsgVarEntry = dsgVar.dsgVarInfos[index];
            string text = "";
            string typeText = DsgVarInfoEntry.GetCSharpStringFromType(dsgVarEntry.type);

            text += typeText + " " + "dsgVar_" + dsgVarEntry.number;

            if (dsgVar.defaultValues != null) {
                text += " = ";

                string stringVal = dsgVar.defaultValues[index].ToString();

                text += stringVal;
            }

            text += ";";

            return text;
        }
    }
}
