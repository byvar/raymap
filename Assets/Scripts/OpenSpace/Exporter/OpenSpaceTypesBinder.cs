using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.Exporter {
    public class OpenSpaceTypesBinder : ISerializationBinder {

        public Type BindToType(string assemblyName, string typeName)
        {
            throw new NotImplementedException();
            //return KnownTypes.SingleOrDefault(t => t.Name == typeName);
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.UnderlyingSystemType.ToString();
            typeName = typeName.Replace("OpenSpace.Exporter", "OpenSpaceImplementation.LevelLoading");
            typeName = typeName.Replace("OpenSpace.", "OpenSpaceImplementation.");
        }
    }
}
