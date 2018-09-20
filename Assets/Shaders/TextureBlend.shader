Shader "Custom/Texture Blend" {
	Properties{
		_MainTex("Texture 1 (RGBA)", 2D) = "white" {}
		_MainTex2("Texture 2 (RGBA)", 2D) = "white" {}
		_Color("Color", Vector) = (1,1,1,1)
		_DiffuseCoef("Diffuse Coef", Vector) = (1,1,1,1)
		_AmbientCoef("Ambient Coef", Vector) = (1,1,1,1)
		_SpecularCoef("Specular Coef", Vector) = (1,1,1,1)
		_SpecularFactor("Shininess", Float) = 1
		_Glossiness("Smoothness", Range(0,1)) = 0.0
		_Metallic("Metallic", Range(0,1)) = 0.0
		_EmissionColor("Emission color", Color) = (0,0,0,1)
		[MaterialToggle] _UVSec("Use secondary UVs", Float) = 0
		[MaterialToggle] _ShadingMode("Is fragment shaded", Float) = 0
		[MaterialToggle] _Blend("Use secondary texture", Float) = 0

		// Lighting
		_SectorAmbient("Sector Ambient light", Vector) = (1,1,1,1)
		_SectorFog("Sector fog", Vector) = (0,0,0,0)
		_SectorFogParams("Sector fog params", Vector) = (0,0,0,0)
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		//#pragma surface surf Standard fullforwardshadows alpha
		#pragma surface surf Standard vertex:process_vert finalcolor:process_color
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#include "TextureBlendShared.cginc"

		void surf(Input IN, inout SurfaceOutputStandard o) {
			process_surf(IN, o, 0.0);
		}
		ENDCG
	}
	FallBack "Diffuse"
}