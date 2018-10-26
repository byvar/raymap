Shader "Custom/GizmoIcon" {
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Overlay-1"
			"IgnoreProjector" = "True"
			"SortingLayer" = "Resources_Sprites"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
			"DisableBatching" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Always
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#pragma target 2.0
#include "UnityCG.cginc"

			//            uniform Float _Time;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				float2 worldScale = float2(
					length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
					length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)) // scale y axis
					);

				float4 scaledVertex = float4(IN.vertex.z, IN.vertex.y, 0.0, 0.0) * float4(worldScale.x, worldScale.y, 1.0, 1.0);
				OUT.vertex = mul(UNITY_MATRIX_P,
					float4(UnityObjectToViewPos(float3(0.0, 0.0, 0.0)), 1.0)
					+ scaledVertex);

				return OUT;
			}

			sampler2D _MainTex;

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				c.rgb *= c.a;
				return c;
			}
			ENDCG
		}
	}
}