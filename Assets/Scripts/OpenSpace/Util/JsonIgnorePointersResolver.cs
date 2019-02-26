using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace {
	public class JsonIgnorePointersResolver : DefaultContractResolver {
		public static JsonIgnorePointersResolver Instance { get; } = new JsonIgnorePointersResolver();

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
			JsonProperty property = base.CreateProperty(member, memberSerialization);
			if (member.MemberType == MemberTypes.Property || typeof(Pointer).IsAssignableFrom(member.DeclaringType)) {
				property.Ignored = true;
			}
			return property;
		}
	}
}
