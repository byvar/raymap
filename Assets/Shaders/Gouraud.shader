Shader "Custom/Gouraud" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_MainTex2("Albedo 2 (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_DiffuseCoef("Diffuse Coef", Vector) = (1,1,1,1)
		_AmbientCoef("Ambient Coef", Vector) = (1,1,1,1)
		_SpecularCoef("Specular Coef", Vector) = (1,1,1,1)
		_SpecularFactor("Shininess", Float) = 0
		[MaterialToggle] _UVSec("Use secondary UVs", Float) = 0
		[MaterialToggle] _Blend("Use secondary texture", Float) = 0
	}
	SubShader{
		Pass{
			Tags{ "LightMode" = "ForwardBase" }
			// pass for ambient light and first light source

			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members blendValue)
#pragma exclude_renderers d3d11

			#pragma vertex vert  
			#pragma fragment frag 

			#include "UnityCG.cginc"
			uniform float4 _LightColor0; // color of light source (from "Lighting.cginc")

			// User-specified properties
			uniform float4 _Color;
			uniform float4 _AmbientCoef;
			uniform float4 _DiffuseCoef;
			uniform float4 _SpecularCoef;
			uniform float _SpecularFactor;
			sampler2D _MainTex;
			sampler2D _MainTex2;
			float _UVSec;
			float _Blend;

			struct v2f {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float2 uv1 : TEXCOORD0; // The first UV coordinate.
				float2 uv2 : TEXCOORD1; // The second UV coordinate.
				float blendValue;
			};

			v2f vert(appdata_full v) {
				v2f o;
				o.uv1 = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.texcoord1, _MainTex2);
				float4x4 modelMatrix = unity_ObjectToWorld;
				float3x3 modelMatrixInverse = unity_WorldToObject;
				float3 normalDirection = normalize(mul(input.normal, modelMatrixInverse));
				float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, v.vertex).xyz);
				float3 lightDirection;
				float attenuation;

				if (0.0 == _WorldSpaceLightPos0.w) { // directional light?
					attenuation = 1.0; // no attenuation
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				} else { // point or spot light
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - mul(modelMatrix, v.vertex).xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance; // linear attenuation 
					lightDirection = normalize(vertexToLightSource);
				}

				float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb * _AmbientCoef.xyz * _AmbientCoef.w;
				ambientLighting = ambientLighting + _Color.rgb * (1.0 - _AmbientCoef.w);

				float3 diffuseReflection = attenuation * _LightColor0.rgb
					* _Color.rgb
					* _DiffuseCoef.xzy * _DiffuseCoef.w
					* max(0.0, dot(normalDirection, lightDirection));

				float3 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0) { // light source on the wrong side?
					specularReflection = float3(0.0, 0.0, 0.0); // no specular reflection
				} else { // light source on the right side
					specularReflection = attenuation * _LightColor0.rgb
						* _SpecularCoef.xyz * _SpecularCoef.w
						* pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _SpecularFactor);
				}

				o.col = float4(ambientLighting + diffuseReflection + specularReflection, 1.0);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.blendValue = v.texcoord.z;
				return output;
			}

			float4 frag(v2f i) : COLOR {
				fixed2 uv1 = i.uv1;
				fixed2 uv2 = i.uv2;
				if (_UVSec == 0) {
					uv2 = uv1;
				}
				float4 c;
				if (_Blend == 1) {
					c = lerp(tex2D(_MainTex, uv1), tex2D(_MainTex2, uv2), blendfactor) * i.col;
				} else {
					c = tex2D(_MainTex, uv1) * i.col;
				}
				c.rgb = c.rgb * (1 + (_EmissionColor.rgb * 2 * _EmissionColor.a));
				return c;
			}

			ENDCG
		}

		Pass{
			Tags{ "LightMode" = "ForwardAdd" }
			// pass for additional light sources
			Blend One One // additive blending 

			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag 

			#include "UnityCG.cginc"
			uniform float4 _LightColor0; // color of light source (from "Lighting.cginc")

			// User-specified properties
			uniform float4 _Color;
			uniform float4 _AmbientCoef;
			uniform float4 _DiffuseCoef;
			uniform float4 _SpecularCoef;
			uniform float _SpecularFactor;

			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
			};

			vertexOutput vert(vertexInput input) {
				vertexOutput output;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float3x3 modelMatrixInverse = unity_WorldToObject;
				float3 normalDirection = normalize(
					mul(input.normal, modelMatrixInverse));
				float3 viewDirection = normalize(_WorldSpaceCameraPos
					- mul(modelMatrix, input.vertex).xyz);
				float3 lightDirection;
				float attenuation;

				if (0.0 == _WorldSpaceLightPos0.w) { // directional light?
					attenuation = 1.0; // no attenuation
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				} else { // point or spot light
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz
						- mul(modelMatrix, input.vertex).xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance; // linear attenuation 
					lightDirection = normalize(vertexToLightSource);
				}

				float3 diffuseReflection = attenuation * _LightColor0.rgb
					* _Color.rgb
					* _DiffuseCoef.rgb * _DiffuseCoef.a
					* max(0.0, dot(normalDirection, lightDirection));

				float3 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0) { // light source on the wrong side?
					specularReflection = float3(0.0, 0.0, 0.0); // no specular reflection
				} else { // light source on the right side
					specularReflection = attenuation * _LightColor0.rgb
						* _SpecularCoef.rgb * _SpecularCoef.a
						* pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _SpecularFactor);
				}

				output.col = float4(diffuseReflection + specularReflection, 1.0); // no ambient contribution in this pass
				output.pos = UnityObjectToClipPos(input.vertex);
				return output;
			}

			float4 frag(vertexOutput input) : COLOR {
				return input.col;
			}

			ENDCG
		}
	}
	Fallback "Specular"
}