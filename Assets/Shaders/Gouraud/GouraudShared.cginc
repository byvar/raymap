#ifndef SHARED_GOURAUD
#define SHARED_GOURAUD

#include "UnityCG.cginc"
//uniform float4 _LightColor0; // color of light source (from "Lighting.cginc")

// User-specified properties
uniform float4 _AmbientCoef;
uniform float4 _DiffuseCoef;

float _NumTextures;
sampler2D _Tex0;
sampler2D _Tex1;
sampler2D _Tex2;
sampler2D _Tex3;
uniform float4 _Tex0_ST;
uniform float4 _Tex1_ST;
uniform float4 _Tex2_ST;
uniform float4 _Tex3_ST;
float4 _Tex0Params;
float4 _Tex1Params;
float4 _Tex2Params;
float4 _Tex3Params;
float4 _Tex0Params2;
float4 _Tex1Params2;
float4 _Tex2Params2;
float4 _Tex3Params2;

// Lighting
float _Billboard;
//uniform float4 _SectorAmbient;
uniform float4 _SectorFog;
uniform float4 _SectorFogParams;
#ifndef GOURAUD_NUM_LIGHTS
float4 _StaticLightPos[512];
float4 _StaticLightDir[512];
float4 _StaticLightCol[512];
float4 _StaticLightParams[512];
#else
float4 _StaticLightPos[GOURAUD_NUM_LIGHTS];
float4 _StaticLightDir[GOURAUD_NUM_LIGHTS];
float4 _StaticLightCol[GOURAUD_NUM_LIGHTS];
float4 _StaticLightParams[GOURAUD_NUM_LIGHTS];
#endif
float _StaticLightCount = 0;
float _Luminosity = 0.5;
float _Saturate = 1.0;
float _DisableLighting = 0;

struct v2f {
	float4 pos : SV_POSITION;
	float3 uv1 : TEXCOORD0; // The first UV coordinate.
	float3 uv2 : TEXCOORD1; // The second UV coordinate.
	float3 uv3 : TEXCOORD2; // The second UV coordinate.
	float3 uv4 : TEXCOORD3; // The second UV coordinate.
	float4 diffuseColor : TEXCOORD4;
	float3 normal : TEXCOORD5;
	float3 multipliedPosition : TEXCOORD6;
	float3 fogViewPos : TEXCOORD7;
	//UNITY_FOG_COORDS(3)
};

float CalcSphereAttenuation(float distance, float near, float far) {
	if (distance <= near) {
		return 1.0;
	} else {
		return 1.0 - (distance - near) / (far - near); // TODO: Get correct attenuation
		/*float attenuation = 1.0;
		float nearNear = near*near;
		float farFar = far*far;
		if (nearNear != farFar) {
			attenuation = 1.0 / (farFar - nearNear);
		}
		return (farFar - distance) * attenuation;*/
	}
}

float CalcNormalFactor(float3 normalDirection, float3 lightDirection) {
	if (_Billboard == 1.0) {
		return 1.0;
	} else {
		return max(0.0, dot(normalDirection, lightDirection));
	}
}

float4 ApplyStaticLights(float3 normalDirection, float3 multipliedPosition) {
	if(_DisableLighting == 1.0) return float4(1.0, 1.0, 1.0, 1.0);

	/* Alpha light flags:
	    0 = Affect color and alpha
	    1 = Only affect alpha
	    2 = Only affect color
	*/

	float3 lightDirection;
	float3 vertexToLightSource;
	float attenuation;
	float near;
	float far;
	float distance;
	float normalFactor;
	float alpha = 0.0;
	float3 diffuseReflection = float3(0.0, 0.0, 0.0);
	float3 luminosity = float3(_Luminosity - 0.5, _Luminosity - 0.5, _Luminosity - 0.5);
	float4 ambient = _AmbientCoef;
	float num_lights = _StaticLightCount;
#ifdef GOURAUD_NUM_LIGHTS
	if (num_lights > GOURAUD_NUM_LIGHTS) num_lights = GOURAUD_NUM_LIGHTS;
#endif
	for (int i = 0; i < num_lights; i++) {
		if (_StaticLightPos[i].w == 1) {
			attenuation = 1.0; // no attenuation
			lightDirection = normalize(_StaticLightDir[i].xyz);
			/*if (_StaticLightParams[i].z == 0) {
				normalFactor = max(0.0, dot(normalDirection, lightDirection));
			} else normalFactor = 1.0;*/
			normalFactor = CalcNormalFactor(normalDirection, lightDirection);
			if (_StaticLightParams[i].w != 1) diffuseReflection = diffuseReflection + attenuation * _StaticLightCol[i].xyz * normalFactor;
				//* colRgb * _DiffuseCoef.xyz //* _DiffuseCoef.w
			if (_StaticLightParams[i].w != 2) alpha = alpha + _StaticLightCol[i].w * normalFactor;// * _DiffuseCoef.w;
		} else if (_StaticLightPos[i].w == 2) {
			vertexToLightSource = _StaticLightPos[i].xyz - multipliedPosition;
			distance = length(vertexToLightSource);
			far = _StaticLightParams[i].y;
			if (distance < far) {
				near = _StaticLightParams[i].x;
				attenuation = CalcSphereAttenuation(distance, near, far);
				if (_StaticLightParams[i].z == 0) {
					lightDirection = normalize(vertexToLightSource);
					normalFactor = CalcNormalFactor(normalDirection, lightDirection);
				} else normalFactor = 1.0; // Painting light
				if (normalFactor != 0) {
					if (_StaticLightParams[i].w != 1) diffuseReflection = diffuseReflection + attenuation * _StaticLightCol[i].xyz * normalFactor;
					//* colRgb * _DiffuseCoef.xyz //* _DiffuseCoef.w
					if (_StaticLightParams[i].w != 2) alpha = alpha + attenuation * _StaticLightCol[i].w * normalFactor;// * _DiffuseCoef.w;
				}
			}
		} else if (_StaticLightPos[i].w == 4) {
			if (_StaticLightParams[i].w != 1) ambient.xyz = ambient.xyz + _StaticLightCol[i].xyz * _DiffuseCoef.xyz;
			//* colRgb * _DiffuseCoef.xyz //* _DiffuseCoef.w
			if (_StaticLightParams[i].w != 2) ambient.w = ambient.w + _StaticLightCol[i].w * _DiffuseCoef.w;// * _DiffuseCoef.w;
		} else if (_StaticLightPos[i].w == 7) {
			vertexToLightSource = _StaticLightPos[i].xyz - multipliedPosition;
			distance = length(vertexToLightSource);
			far = _StaticLightParams[i].y;
			if (distance < far) {
				near = _StaticLightParams[i].x;
				attenuation = CalcSphereAttenuation(distance, near, far);
				if (_StaticLightParams[i].z == 0) {
					lightDirection = normalize(_StaticLightDir[i].xyz);
					normalFactor = CalcNormalFactor(normalDirection, lightDirection);
				} else normalFactor = 1.0; // Painting light
				if (normalFactor != 0) {
					if (_StaticLightParams[i].w != 1) diffuseReflection = diffuseReflection + attenuation * _StaticLightCol[i].xyz * normalFactor;
					//* colRgb * _DiffuseCoef.xyz //* _DiffuseCoef.w
					if (_StaticLightParams[i].w != 2) alpha = alpha + attenuation * _StaticLightCol[i].w * normalFactor;// *_DiffuseCoef.w;
				}
			}
		}
	}
	//float3 ambientLighting = ambient.xyz * _DiffuseCoef.xyz;
	diffuseReflection = luminosity + ambient.xyz + diffuseReflection * (luminosity + _DiffuseCoef.xyz);
	alpha = ambient.w + alpha * _DiffuseCoef.w;
	if (_Saturate == 1.0) {
		diffuseReflection.x = saturate(diffuseReflection.x);
		diffuseReflection.y = saturate(diffuseReflection.y);
		diffuseReflection.z = saturate(diffuseReflection.z);
		alpha = saturate(alpha);
	}
	return float4(diffuseReflection, alpha);
}

float2 TransformUV(float2 uv_in, float4 tex_params, float4 tex_params_2) {
	float2 uv_out = uv_in + tex_params_2.xy;
	if (tex_params.y > 0) {
		// y is scroll mode.
		// 0: None
		// 1: Regular scroll
		// 2: Rotate
		float time = floor(_Time.w * 20); // = Time.y (which is just time in seconds) * 60. Time.w = Time.y * 3
		//float time = floor(_Time.y * 45);
		if (tex_params.y == 1) {
			uv_out.x = uv_out.x + time * tex_params_2.z;
			uv_out.y = uv_out.y + time * tex_params_2.w;
		}
	}
	return uv_out;
}

v2f process_vert(appdata_full v, float isLight, float isAdd) {
	v2f o;
	o.uv1 = float3(TRANSFORM_TEX(TransformUV( v.texcoord.xy, _Tex0Params, _Tex0Params2), _Tex0),  v.texcoord.z);
	o.uv2 = float3(TRANSFORM_TEX(TransformUV(v.texcoord1.xy, _Tex1Params, _Tex1Params2), _Tex1), v.texcoord1.z);
	o.uv3 = float3(TRANSFORM_TEX(TransformUV(v.texcoord2.xy, _Tex2Params, _Tex2Params2), _Tex2), v.texcoord2.z);
	o.uv4 = float3(TRANSFORM_TEX(TransformUV(v.texcoord3.xy, _Tex3Params, _Tex3Params2), _Tex3), v.texcoord3.z);

	float4x4 modelMatrix = unity_ObjectToWorld;
	float4x4 modelMatrixInverse = unity_WorldToObject;
	float3 normalDirection, multipliedPosition;
	/*if (_Billboard == 1.0) {
		// Billboard / LookAt as it is called in the engine
				float2 worldScale = float2(
					length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
					length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)) // scale y axis
					);

				o.pos = mul(UNITY_MATRIX_P,
					float4(UnityObjectToViewPos(float3(0.0, 0.0, 0.0)), 1.0)
					+ scaledVertex);
		float4 viewSpaceCenter = float4(UnityObjectToViewPos(float3(0.0, 0.0, 0.0)), 1.0);
		float4 scaledVertex = float4(v.vertex.z, v.vertex.y, 0.0, 0.0) * float4(worldScale.x, worldScale.y, 1.0, 1.0);
		//float4 objectCenter = mul(modelMatrix, float4(0, 0, 0, 1.0));
		//float4 worldVertex = mul(modelMatrix, v.vertex) - objectCenter;
		//float4 viewSpaceVertex = viewSpaceCenter + float4(-worldVertex.z, worldVertex.y, 0, 0);
		//float4 scaledVertex = float4(mul((float3x3)unity_ObjectToWorld, IN.vertex.xyz).x, mul((float3x3)unity_ObjectToWorld, IN.vertex.xyz).y, 0.0, 1.0);
		viewSpaceVertex = viewSpaceCenter + scaledVertex;
		o.pos = mul(UNITY_MATRIX_P, viewSpaceVertex);
		//float4 viewSpaceNormal = viewSpaceVertex + float4(0.0, 0.0, -1.0, 0.0);
		//normalDirection = normalize(mul(float4(mul(UNITY_MATRIX_I_V, viewSpaceNormal).xyz, 0.0), modelMatrixInverse).xyz); // Normal in object space
		multipliedPosition = mul(UNITY_MATRIX_I_V, viewSpaceVertex).xyz; // Vertex in world space
	} else {*/
	o.pos = UnityObjectToClipPos(v.vertex);
	multipliedPosition = mul(modelMatrix, v.vertex).xyz; // Vertex in world space
	normalDirection = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz); // Normal in object space

	//}
	//float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, v.vertex).xyz);
	float3 lightDirection;
	float attenuation;

	float alpha = 1.0;
	//float3 colRgb = _Color.rgb * _Color.w + float3(1.0, 1.0, 1.0) * (1.0 - _Color.w);

	float3 ambientLighting = 0.0;
	/*if (isAdd == 0.0) {
		ambientLighting = _SectorAmbient.rgb * _AmbientCoef.w;
		ambientLighting = ambientLighting + _AmbientCoef.xyz * colRgb;

		//ambientLighting = _SectorAmbient.rgb * _AmbientCoef.w;
		//ambientLighting = ambientLighting + _AmbientCoef.xyz * colRgb;
		
		//ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * colRgb * _AmbientCoef.xyz * _AmbientCoef.w;
		//ambientLighting = ambientLighting + colRgb * (1.0 - _AmbientCoef.w);
		
		//UNITY_LIGHTMODEL_AMBIENT.rgb * colRgb * _AmbientCoef.xyz * (1.0-(_SpecularCoef.w/100.0));
		//ambientLighting = ambientLighting + colRgb * (_SpecularCoef.w/100.0);
	}*/
	float3 diffuseReflection = float3(0.0, 0.0, 0.0);
	if (isLight == 0.0) {
		//if (isAdd == 1.0) {
		/*if (0.0 == _WorldSpaceLightPos0.w) { // directional light?
			attenuation = 1.0; // no attenuation
			lightDirection = normalize(_WorldSpaceLightPos0.xyz);
		} else { // point or spot light
			float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - multipliedPosition;
			float distance = length(vertexToLightSource);
			attenuation = 1.0 / distance; // linear attenuation
			lightDirection = normalize(vertexToLightSource);
		}
		diffuseReflection = attenuation * _LightColor0.rgb
			//* colRgb
			* _DiffuseCoef.xyz //* _DiffuseCoef.w
			* max(0.0, dot(normalDirection, lightDirection));*/
	} else {
		diffuseReflection = float3(1.0, 1.0, 1.0);
	}
	if (/*_ShadingMode == 0.0 && */isAdd == 0.0 && isLight == 0.0) {
		float4 lightCol = ApplyStaticLights(normalDirection, multipliedPosition);
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
	if (_SectorFog.w != 0) o.fogViewPos = UnityObjectToViewPos(v.vertex).xyz;
	//o.specularColor = specularReflection;
	//UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

float4 TextureOp(float4 color_in, float4 diffuseColor, sampler2D tex, float3 uv, float4 tex_params, float4 tex_params_2, float index) {
	float4 texColor = tex2D(tex, uv.xy);
	//if (tex_params.w == 1 && index > 0) return color_in; // Mirrors are not supported yet
	if (tex_params.x == 1 && index > 0) {
		// Additive
		return color_in + float4(texColor.xyz * texColor.w * uv.z, 0);
	} /*else if (tex_params.w == 8888 && index > 0) {
		float alpha = (diffuseColor.w * texColor.w * uv.z);
		return color_in + float4(diffuseColor.xyz * texColor.xyz * alpha, alpha);
	}*/
	/*else if (tex_params.x == 3) {
		// Transparent
		return lerp(color_in, diffuseColor * float4(texColor.xyz, 1), uv.z);
	}*/ else {
		return lerp(color_in, diffuseColor * texColor, uv.z);
		//color_out.a = color_out.a = color_out.a * i.diffuseColor.w;
		//return diffuseColor * color_out;
	}
}

float4 process_frag(v2f i, float clipAlpha, float isAdd) : SV_TARGET {
	float4 c = float4(0.0, 0.0, 0.0, 0.0);
	if (_NumTextures > 0) {
		c = TextureOp(c, i.diffuseColor, _Tex0, i.uv1, _Tex0Params, _Tex0Params2, 0);
		if (_NumTextures > 1) {
			c = TextureOp(c, i.diffuseColor, _Tex1, i.uv2, _Tex1Params, _Tex1Params2, 1);
			if (_NumTextures > 2) {
				c = TextureOp(c, i.diffuseColor, _Tex2, i.uv3, _Tex2Params, _Tex2Params2, 2);
				if (_NumTextures > 3) {
					c = TextureOp(c, i.diffuseColor, _Tex3, i.uv4, _Tex3Params, _Tex3Params2, 3);
				}
			}
		}
	}

	/*float blendfactor = i.uv2.z;
	if (_Blend == 1) {
		c = lerp(tex2D(_MainTex, uv1), tex2D(_MainTex2, uv2), blendfactor);
	} else {
		c = tex2D(_MainTex, uv1.xy);
	}*/
	//c.a = c.a * i.diffuseColor.w;
	if (clipAlpha < 0) {
		clip(clipAlpha * (c.a - 1.0));
	} else {
		clip(clipAlpha * (c.a - 0.999));
	}
	//c.rgb = c.rgb * (1 + (_EmissionColor.rgb * 2 * _EmissionColor.a));
	//if (_ShadingMode == 0.0 || isAdd == 1.0) {
		//c = float4(i.diffuseColor.xyz * c, c.a);
	/*} else {
		float3 colRgb = _Color.rgb * _Color.w + float3(1.0, 1.0, 1.0) * (1.0 - _Color.w);
		float4 lightColor = ApplyStaticLights(colRgb, normalize(i.normal), i.multipliedPosition);
		c = float4((i.diffuseColor.xyz + lightColor) * c, c.a * lightColor.w);
		clip(clipAlpha * (c.a - 1.0));
	}*/
	// Add fog
	if (_SectorFog.w != 0) {
		float fog;
		if (_SectorFogParams.x != _SectorFogParams.y) { // Blend near != Blend far
			float fogz = length(i.fogViewPos);
			fog = _SectorFogParams.x +
				saturate((fogz - _SectorFogParams.z) / (_SectorFogParams.w - _SectorFogParams.z))
				* (_SectorFogParams.y - _SectorFogParams.x);
		} else {
			fog = _SectorFogParams.y;
		}
		if (isAdd == 1.0) {
			c.rgb = lerp(c.rgb, float3(0, 0, 0), fog * _SectorFog.w);
		} else {
			c.rgb = lerp(c.rgb, _SectorFog.xyz, fog * _SectorFog.w);
		}
		//c.rgb = lerp(c.rgb, _SectorFog.xyz, fog);
	}
	//UNITY_APPLY_FOG(i.fogCoord, c);
	//return float4(i.specularColor + i.diffuseColor * c, c.a);
	return c;
}
#endif // SHARED_GOURAUD