using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.Exporter {
    public class UnityConverter : JsonConverter {

        struct EVector2 {
            public float x;
            public float y;
        }

        struct EVector2Int {
            public int x;
            public int y;
        }

        struct EVector3 {
            public float x;
            public float y;
            public float z;
        }

        struct EVector3Int {
            public int x;
            public int y;
            public int z;
        }

        struct EVector4 {
            public float x;
            public float y;
            public float z;
            public float w;
        }

        Type[] types = new Type[]
        {
                typeof(Vector2),
                typeof(Vector2Int),
                typeof(Vector3),
                typeof(Vector3Int),
                typeof(Vector4)
        };

        public override bool CanConvert(Type objectType)
        {
            return types.Contains(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Vector2) {
                var jt = JToken.FromObject(new EVector2()
                {
                    x = ((Vector2)value).x,
                    y = ((Vector2)value).y,
                });
                jt.WriteTo(writer);
            }
            if (value is Vector2Int) {
                var jt = JToken.FromObject(new EVector2Int()
                {
                    x = ((Vector2Int)value).x,
                    y = ((Vector2Int)value).y,
                });
                jt.WriteTo(writer);
            }
            if (value is Vector3) {
                var jt = JToken.FromObject(new EVector3()
                {
                    x = ((Vector3)value).x,
                    y = ((Vector3)value).y,
                    z = ((Vector3)value).z,
                });
                jt.WriteTo(writer);
            }
            if (value is Vector3Int) {
                var jt = JToken.FromObject(new EVector3Int()
                {
                    x = ((Vector3Int)value).x,
                    y = ((Vector3Int)value).y,
                    z = ((Vector3Int)value).z,
                });
                jt.WriteTo(writer);
            }
            if (value is Vector4) {
                var jt = JToken.FromObject(new EVector4()
                {
                    x = ((Vector4)value).x,
                    y = ((Vector4)value).y,
                    z = ((Vector4)value).z,
                    w = ((Vector4)value).w
                });
                jt.WriteTo(writer);
            }

        }
    }

}