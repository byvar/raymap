Shader "Custom/Texture Blend (Alpha)" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGBA)", 2D) = "white" {}
		_MainTex2("Albedo 2 (RGBA)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.0
		_Metallic("Metallic", Range(0,1)) = 0.0
		_EmissionColor("Emission color", Color) = (0,0,0,1)
		[MaterialToggle] _UVSec("Use secondary UVs", Float) = 0
		[MaterialToggle] _UseAlpha("Use texture 2 as alpha mask", Float) = 0
		[MaterialToggle] _Blend("Use secondary texture", Float) = 0
	}
		SubShader{
		//Tags{ "RenderType" = "Cutout" }
		Tags{ "Queue" = "Transparent" "RenderType" = "Fade" }
		//Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
//#pragma surface surf Standard fullforwardshadows
#pragma surface surf Standard alpha:fade vertex:vert
		//#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
	sampler2D _MainTex2;

	struct Input {
		float2 uv_MainTex;
		float2 uv2_MainTex2;
		float blendValue;
	};

	half _Glossiness;
	half _Metallic;
	float _UVSec;
	float _UseAlpha;
	float _Blend;
	fixed4 _Color;
	fixed4 _EmissionColor;

	void vert(inout appdata_full v, out Input o) {
		UNITY_INITIALIZE_OUTPUT(Input, o);
		o.blendValue = v.texcoord.z;
	}

	void surf(Input IN, inout SurfaceOutputStandard o) {
		fixed2 uv1 = IN.uv_MainTex;
		fixed2 uv2 = IN.uv2_MainTex2;
		float blendfactor = IN.blendValue;
		if (_UVSec == 0) {
			uv2 = uv1;
		}
		if (_UseAlpha == 1) {
			fixed4 c = tex2D(_MainTex, uv1) * _Color;
			fixed4 c2 = tex2D(_MainTex2, uv2) * _Color;
			o.Albedo = c.rgb * (1 + (_EmissionColor.rgb * 2 * _EmissionColor.a));
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = lerp(c2.a, c.a, blendfactor);
		} else {
			fixed4 c;
			if (_Blend == 1) {
				c = lerp(tex2D(_MainTex, uv1), tex2D(_MainTex2, uv2), blendfactor) * _Color;
			} else {
				c = tex2D(_MainTex, uv1) * _Color;
			}
			o.Albedo = c.rgb * (1 + (_EmissionColor.rgb * 2 * _EmissionColor.a));
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
	}
	ENDCG
	}
		FallBack "Diffuse"
}