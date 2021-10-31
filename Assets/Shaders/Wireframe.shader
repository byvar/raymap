// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/Wireframe"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		//_WireColor("WireColor", Color) = (1,0,0,1)
		_Color("Color", Color) = (1,1,1,1)
	}
		SubShader
	{

		Tags {
			"RenderType" = "Opaque"
		}

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma target 4.0
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			uniform sampler2D _MainTex;
			half4 _Color;

			struct v2g
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				float3 normal: NORMAL;
			};

			struct g2f
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				float3 dist : TEXCOORD1;
				float3 normal: NORMAL;
			};

			v2g vert(appdata_base v)
			{
				v2g OUT;
				OUT.pos = UnityObjectToClipPos(v.vertex);
				OUT.uv = v.texcoord;
				OUT.normal = v.normal;
				return OUT;
			}

			[maxvertexcount(3)]
			void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
			{

				float2 WIN_SCALE = float2(_ScreenParams.x / 2.0, _ScreenParams.y / 2.0);

				//frag position
				float2 p0 = WIN_SCALE * IN[0].pos.xy / IN[0].pos.w;
				float2 p1 = WIN_SCALE * IN[1].pos.xy / IN[1].pos.w;
				float2 p2 = WIN_SCALE * IN[2].pos.xy / IN[2].pos.w;

				//barycentric position
				float2 v0 = p2 - p1;
				float2 v1 = p2 - p0;
				float2 v2 = p1 - p0;
				//triangles area
				float area = abs(v1.x * v2.y - v1.y * v2.x);

				g2f OUT;
				OUT.pos = IN[0].pos;
				OUT.uv = IN[0].uv;
				OUT.dist = float3(area / length(v0),0,0);
				OUT.normal = IN[0].normal;
				triStream.Append(OUT);

				OUT.pos = IN[1].pos;
				OUT.uv = IN[1].uv;
				OUT.dist = float3(0,area / length(v1),0);
				OUT.normal = IN[1].normal;
				triStream.Append(OUT);

				OUT.pos = IN[2].pos;
				OUT.uv = IN[2].uv;
				OUT.dist = float3(0,0,area / length(v2));
				OUT.normal = IN[2].normal;
				triStream.Append(OUT);

			}

			half4 frag(g2f IN) : COLOR
			{
				//distance of frag from triangles center
				float d = min(IN.dist.x, min(IN.dist.y, IN.dist.z));
			//fade based on dist from center
			float I = exp2(-4.0 * d * d);

			float mult = 1.0;
			if (IN.normal.y < 0.7071067691) { // 1/sqrt(2)
				mult = 0.6;
			}
			else {
				mult = 1.0;
			}

			float4 baseColor = _Color * tex2D(_MainTex, IN.uv) * mult;
			float4 wireColor = baseColor*0.25;
			if ((baseColor.r + baseColor.g) < 1.0) {
				wireColor = float4(1.0, 1.0, 1.0, 1.0) - wireColor;
			}
			wireColor.a = 1.0;

			return lerp(baseColor, wireColor, I);
		}

		ENDCG

	}
	}
}