Shader "Custom/GouraudLight" {
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
		[PerRendererData] _SectorFog("Sector fog", Vector) = (0,0,0,0)
		[PerRendererData] _SectorFogParams("Sector fog params", Vector) = (0,0,0,0)
	}
	SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" }
		ZWrite Off
		Cull Off
		Lighting Off
		Pass{
			//Tags{ "LightMode" = "ForwardBase" }
			Blend SrcAlpha OneMinusSrcColor
			//Blend One One
			// pass for ambient light and first light source

			CGPROGRAM

			#pragma target 3.0
			#pragma vertex vert  
			#pragma fragment frag
			#pragma multi_compile_fog

			#define GOURAUD_NUM_LIGHTS 3
			#include "GouraudShared.cginc"

			v2f vert(appdata_full v) {
				return process_vert(v, 1.0, 1.0);
			}
			float4 frag(v2f i) : COLOR{
				return process_frag(i, 0.0, 1.0);
			}
			ENDCG
		}
		/*Pass{
			Tags{ "LightMode" = "ForwardAdd" }
			Blend SrcAlpha One
			// pass for additional light sources
			//Blend One One // additive blending 

			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag 
			#pragma multi_compile_fog

			#include "GouraudShared.cginc"

			v2f vert(appdata_full v) {
				return process_vert(v, 0.0, 1.0);
			}
			float4 frag(v2f i) : COLOR{
				return process_frag(i, 0.0, 1.0);
			}
			ENDCG
		}*/

	}
	Fallback Off
}