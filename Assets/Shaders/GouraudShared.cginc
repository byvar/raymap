#ifndef SHARED_GOURAUD
#define SHARED_GOURAUD

#include "UnityCG.cginc"
uniform float4 _LightColor0; // color of light source (from "Lighting.cginc")

// User-specified properties
uniform float4 _Color;
uniform float4 _AmbientCoef;
uniform float4 _DiffuseCoef;
uniform float4 _SpecularCoef;
uniform float _SpecularFactor;
uniform float4 _EmissionColor;
sampler2D _MainTex;
sampler2D _MainTex2;
uniform float4 _MainTex_ST;
uniform float4 _MainTex2_ST;
float _UVSec;
float _Blend;

struct v2f {
	float4 pos : SV_POSITION;
	float3 uv1 : TEXCOORD0; // The first UV coordinate.
	float2 uv2 : TEXCOORD1; // The second UV coordinate.
	float3 diffuseColor : TEXCOORD2;
	UNITY_FOG_COORDS(3)
};

v2f process_vert(appdata_full v, float addAmbient) {
	v2f o;
	o.uv1 = float3(TRANSFORM_TEX(v.texcoord, _MainTex), v.texcoord.z);
	if (_UVSec == 0) {
		o.uv2 = TRANSFORM_TEX(v.texcoord1, _MainTex);
	} else {
		o.uv2 = TRANSFORM_TEX(v.texcoord1, _MainTex2);
	}
	float4x4 modelMatrix = unity_ObjectToWorld;
	float4x4 modelMatrixInverse = unity_WorldToObject;

	float3 normalDirection = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz);
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

	float3 colRgb = _Color.rgb * _Color.w + float3(1.0, 1.0, 1.0) * (1.0 - _Color.w);

	float3 ambientLighting = 0.0;
	if (addAmbient == 1.0) {
		ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _AmbientCoef.w;
		ambientLighting = ambientLighting + _AmbientCoef.xyz * colRgb;
		//ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * colRgb * _AmbientCoef.xyz * _AmbientCoef.w;
		//ambientLighting = ambientLighting + colRgb * (1.0 - _AmbientCoef.w);
		//UNITY_LIGHTMODEL_AMBIENT.rgb * colRgb * _AmbientCoef.xyz * (1.0-(_SpecularCoef.w/100.0));
		//ambientLighting = ambientLighting + colRgb * (_SpecularCoef.w/100.0);
	}

	float3 diffuseReflection = attenuation * _LightColor0.rgb
		* colRgb
		* _DiffuseCoef.xyz //* _DiffuseCoef.w
		* max(0.0, dot(normalDirection, lightDirection));

	float3 specularReflection;
	if (dot(normalDirection, lightDirection) < 0.0) { // light source on the wrong side?
		specularReflection = float3(0.0, 0.0, 0.0); // no specular reflection
	} else { // light source on the right side
		specularReflection = attenuation * _LightColor0.rgb
			* _SpecularCoef.xyz * 0.1
			* pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _SpecularFactor);
	}
	o.diffuseColor = ambientLighting + diffuseReflection;
	//o.specularColor = specularReflection;
	o.pos = UnityObjectToClipPos(v.vertex);
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

float4 process_frag(v2f i, float clipAlpha) : SV_TARGET {
	fixed2 uv1 = i.uv1.xy;
	fixed2 uv2 = i.uv2;
	if (_UVSec == 0) {
		uv2 = uv1;
	}
	float4 c;
	float blendfactor = i.uv1.z;
	if (_Blend == 1) {
		c = lerp(tex2D(_MainTex, uv1), tex2D(_MainTex2, uv2), blendfactor);
	} else {
		c = tex2D(_MainTex, uv1);
	}
	clip(clipAlpha * (c.a - 1.0));
	c.rgb = c.rgb * (1 + (_EmissionColor.rgb * 2 * _EmissionColor.a));
	c = float4(i.diffuseColor * c, c.a);
	float4 fogcol = float4(c.rgb, 1);
	UNITY_APPLY_FOG(i.fogCoord, c);
	//return float4(i.specularColor + i.diffuseColor * c, c.a);
	return c;
}
#endif // SHARED_GOURAUD