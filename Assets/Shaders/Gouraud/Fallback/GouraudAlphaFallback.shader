Shader "Custom/GouraudAlphaFallback" {
	Properties{
		_NumTextures("Number of textures", Float) = 0

		_Tex0("Texture 0 (RGBA)", 2D) = "white" {}
		_Tex0Params("Texture 0 Parameters", Vector) = (0,0,0,0)
		_Tex0Params2("Texture 0 Parameters 2", Vector) = (0,0,0,0)

		_Tex1("Texture 1 (RGBA)", 2D) = "white" {}
		_Tex1Params("Texture 1 Parameters", Vector) = (0,0,0,0)
		_Tex1Params2("Texture 1 Parameters 2", Vector) = (0,0,0,0)

		_Tex2("Texture 2 (RGBA)", 2D) = "white" {}
		_Tex2Params("Texture 2 Parameters", Vector) = (0,0,0,0)
		_Tex2Params2("Texture 2 Parameters 2", Vector) = (0,0,0,0)

		_Tex3("Texture 3 (RGBA)", 2D) = "white" {}
		_Tex3Params("Texture 3 Parameters", Vector) = (0,0,0,0)
		_Tex3Params2("Texture 3 Parameters 2", Vector) = (0,0,0,0)

		_DiffuseCoef("Diffuse Coef", Vector) = (1,1,1,1)
		_AmbientCoef("Ambient Coef", Vector) = (1,1,1,1)

		// Lighting
		[MaterialToggle] _Billboard("Is billboard", Float) = 0
		//_SectorAmbient("Sector Ambient light", Vector) = (1,1,1,1)
		_SectorFog("Sector fog", Vector) = (0,0,0,0)
		_SectorFogParams("Sector fog params", Vector) = (0,0,0,0)
	}
	SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Lighting Off
		// extra pass that renders to depth buffer only
		/*Pass {
			ZWrite On
			ColorMask 0
			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag 
			#pragma multi_compile_fog

			#include "GouraudShared.cginc"

			v2f vert(appdata_full v) {
				return process_vert(v, 1.0, 0.0);
			}
			float4 frag(v2f i) : COLOR{
				float4 col = process_frag(i, -1.0, 0.0);
				clip(col.a - 0.9);
				return col;
			}
			ENDCG
		}*/

		Pass{
			ZWrite Off
			// pass for ambient light and first light source
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag 
			#pragma multi_compile_fog
			
			#define GOURAUD_NUM_LIGHTS 16
			#define FALLBACK 1
			#include "../GouraudShared.cginc"

			v2f vert(appdata_full v) {
				return process_vert(v, 0.0, 0.0);
			}
			float4 frag(v2f i) : COLOR { 
				return process_frag(i, -1.0, 0.0);
			}
			ENDCG
		}

		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		Pass{
			ZWrite On
			// pass for ambient light and first light source
			Blend One Zero

			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag 

			#define GOURAUD_NUM_LIGHTS 16
			#define FALLBACK 1
			#include "../GouraudShared.cginc"

			v2f vert(appdata_full v) {
				return process_vert(v, 0.0, 0.0);
			}
			float4 frag(v2f i) : COLOR{
				return process_frag(i, 1.0, 0.0);
			}
			ENDCG
		}

	}
	Fallback Off
}