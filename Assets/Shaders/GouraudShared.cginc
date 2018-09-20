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
float _ShadingMode;

// Lighting
uniform float4 _SectorAmbient;
uniform float4 _SectorFog;
uniform float4 _SectorFogParams;
float4 _StaticLightPos[128];
float4 _StaticLightDir[128];
float4 _StaticLightCol[128];
float4 _StaticLightParams[128];
float _StaticLightCount = 0;

struct v2f {
	float4 pos : SV_POSITION;
	float3 uv1 : TEXCOORD0; // The first UV coordinate.
	float2 uv2 : TEXCOORD1; // The second UV coordinate.
	float4 diffuseColor : TEXCOORD2;
	float3 normal : TEXCOORD3;
	float3 multipliedPosition : TEXCOORD4;
	float fog : TEXCOORD5;
	//UNITY_FOG_COORDS(3)
};

float4 ApplyStaticLights(float3 colRgb, float3 normalDirection, float3 multipliedPosition) {
	float3 lightDirection;
	float3 vertexToLightSource;
	float attenuation;
	float near;
	float far;
	float distance;
	float alpha = 1.0;
	float3 diffuseReflection = float3(0.0, 0.0, 0.0);
	for (int i = 0; i < _StaticLightCount; i++) {
		if (_StaticLightPos[i].w == 1) {
			attenuation = 1.0; // no attenuation
			lightDirection = normalize(_StaticLightDir[i].xyz);
			diffuseReflection = diffuseReflection + attenuation * _StaticLightCol[i].xyz
				* colRgb * _DiffuseCoef.xyz //* _DiffuseCoef.w
				* max(0.0, dot(normalDirection, lightDirection));
			if(_StaticLightParams[i].w == 0) alpha = _StaticLightCol[i].w;
		} else if (_StaticLightPos[i].w == 2) {
			vertexToLightSource = _StaticLightPos[i].xyz - multipliedPosition;
			distance = length(vertexToLightSource);
			far = _StaticLightParams[i].y;
			if (distance < far) {
				near = _StaticLightParams[i].x;
				if (distance <= near) {
					attenuation = 1.0;
				} else {
					attenuation = 1.0 - (distance - near) / (far - near);
				}
				lightDirection = normalize(vertexToLightSource);
				diffuseReflection = diffuseReflection + attenuation * _StaticLightCol[i].xyz
					* colRgb * _DiffuseCoef.xyz //* _DiffuseCoef.w
					* max(0.0, dot(normalDirection, lightDirection));
				if (_StaticLightParams[i].w == 0) alpha = lerp(alpha, _StaticLightCol[i].w, attenuation);
			}
		} else if (_StaticLightPos[i].w == 7) {
			vertexToLightSource = _StaticLightPos[i].xyz - multipliedPosition;
			distance = length(vertexToLightSource);
			far = _StaticLightParams[i].y;
			if (distance < far) {
				near = _StaticLightParams[i].x;
				if (distance <= near) {
					attenuation = 1.0;
				} else {
					attenuation = 1.0 - (distance - near) / (far - near);
				}
				lightDirection = normalize(_StaticLightDir[i].xyz);
				diffuseReflection = diffuseReflection + attenuation * _StaticLightCol[i].xyz
					* colRgb * _DiffuseCoef.xyz //* _DiffuseCoef.w
					* max(0.0, dot(normalDirection, lightDirection));
				if (_StaticLightParams[i].w == 0) alpha = lerp(alpha, _StaticLightCol[i].w, attenuation);
			}
		}
	}
	return float4(diffuseReflection, alpha);
}

v2f process_vert(appdata_full v, float isAdd) {
	v2f o;
	o.uv1 = float3(TRANSFORM_TEX(v.texcoord, _MainTex), v.texcoord.z);
	if (_UVSec == 0) {
		o.uv2 = TRANSFORM_TEX(v.texcoord1, _MainTex);
	} else {
		o.uv2 = TRANSFORM_TEX(v.texcoord1, _MainTex2);
	}
	o.pos = UnityObjectToClipPos(v.vertex);
	float4x4 modelMatrix = unity_ObjectToWorld;
	float4x4 modelMatrixInverse = unity_WorldToObject;

	float3 normalDirection = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz); // Normal in object space
	float3 multipliedPosition = mul(modelMatrix, v.vertex).xyz;
	//float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, v.vertex).xyz);
	float3 lightDirection;
	float attenuation;

	float alpha = 1.0;
	float3 colRgb = _Color.rgb * _Color.w + float3(1.0, 1.0, 1.0) * (1.0 - _Color.w);

	float3 ambientLighting = 0.0;
	if (isAdd == 0.0) {
		ambientLighting = _SectorAmbient.rgb * _AmbientCoef.w;
		ambientLighting = ambientLighting + _AmbientCoef.xyz * colRgb;

		//ambientLighting = _SectorAmbient.rgb * _AmbientCoef.w;
		//ambientLighting = ambientLighting + _AmbientCoef.xyz * colRgb;
		
		//ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * colRgb * _AmbientCoef.xyz * _AmbientCoef.w;
		//ambientLighting = ambientLighting + colRgb * (1.0 - _AmbientCoef.w);
		
		//UNITY_LIGHTMODEL_AMBIENT.rgb * colRgb * _AmbientCoef.xyz * (1.0-(_SpecularCoef.w/100.0));
		//ambientLighting = ambientLighting + colRgb * (_SpecularCoef.w/100.0);
	}
	float3 diffuseReflection = float3(0.0, 0.0, 0.0);
	//if (isAdd == 1.0) {
	if (0.0 == _WorldSpaceLightPos0.w) { // directional light?
		attenuation = 1.0; // no attenuation
		lightDirection = normalize(_WorldSpaceLightPos0.xyz);
	} else { // point or spot light
		float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - multipliedPosition;
		float distance = length(vertexToLightSource);
		attenuation = 1.0 / distance; // linear attenuation
		lightDirection = normalize(vertexToLightSource);
	}
	diffuseReflection = attenuation * _LightColor0.rgb
		* colRgb
		* _DiffuseCoef.xyz //* _DiffuseCoef.w
		* max(0.0, dot(normalDirection, lightDirection));
	if (_ShadingMode == 0.0 && isAdd == 0.0) {
		float4 lightCol = ApplyStaticLights(colRgb, normalDirection, multipliedPosition);
		diffuseReflection = diffuseReflection + lightCol.xyz;
		alpha = lightCol.w;
	}
	/*} else {
		for (int i = 0; i < 4; i++) {
			float4 lightPosition = float4(unity_4LightPosX0[i], unity_4LightPosY0[i], unity_4LightPosZ0[i], 1.0);
			float3 difference = lightPosition.xyz - o.pos.xyz;
			float squaredDistance = dot(difference, difference);
			attenuation = 1.0 / (1.0 + unity_4LightAtten0[i] * squaredDistance);
			lightDirection = normalize(difference);

			diffuseReflection = diffuseReflection + attenuation * unity_LightColor[i].rgb
				* colRgb
				* _DiffuseCoef.xyz //* _DiffuseCoef.w
				* max(0.0, dot(normalDirection, lightDirection));
		}
	}*/
	o.normal = normalDirection;
	o.multipliedPosition = multipliedPosition;
	o.diffuseColor = float4(ambientLighting + diffuseReflection, alpha);
	if (_SectorFog.w == 1) {
		float fogz = length(WorldSpaceViewDir(v.vertex));
		o.fog = saturate((fogz - _SectorFogParams.z) / (_SectorFogParams.w - _SectorFogParams.z));
		//o.diffuseColor = float4(ambientLighting + diffuseReflection, fog);
	}
	//o.specularColor = specularReflection;
	//UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

float4 process_frag(v2f i, float clipAlpha, float isAdd) : SV_TARGET {
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
	c.a = c.a * i.diffuseColor.w;
	clip(clipAlpha * (c.a - 1.0));
	c.rgb = c.rgb * (1 + (_EmissionColor.rgb * 2 * _EmissionColor.a));
	if (_ShadingMode == 0.0 || isAdd == 1.0) {
		c = float4(i.diffuseColor.xyz * c, c.a);
	} else {
		float3 colRgb = _Color.rgb * _Color.w + float3(1.0, 1.0, 1.0) * (1.0 - _Color.w);
		float4 lightColor = ApplyStaticLights(colRgb, normalize(i.normal), i.multipliedPosition);
		c = float4((i.diffuseColor.xyz + lightColor) * c, c.a * lightColor.w);
		clip(clipAlpha * (c.a - 1.0));
	}
	// Add fog
	if (_SectorFog.w == 1) {
		float fog = i.fog;
		if (isAdd == 1.0) {
			c.rgb = lerp(c.rgb, float3(0, 0, 0), fog);
		} else {
			c.rgb = lerp(c.rgb, _SectorFog.xyz, fog);
		}
		//c.rgb = lerp(c.rgb, _SectorFog.xyz, fog);
	}
	//UNITY_APPLY_FOG(i.fogCoord, c);
	//return float4(i.specularColor + i.diffuseColor * c, c.a);
	return c;
}
#endif // SHARED_GOURAUD