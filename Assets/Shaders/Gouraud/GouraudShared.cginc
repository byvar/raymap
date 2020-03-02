#ifndef SHARED_GOURAUD
#define SHARED_GOURAUD
#pragma multi_compile_instancing
#include "UnityCG.cginc"
//uniform float4 _LightColor0; // color of light source (from "Lighting.cginc")

// User-specified properties
uniform float4 _AmbientCoef;
uniform float4 _DiffuseCoef;

float _Prelit;
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
UNITY_INSTANCING_BUFFER_START(Props)
UNITY_DEFINE_INSTANCED_PROP(float4, _SectorFog)
UNITY_DEFINE_INSTANCED_PROP(float4, _SectorFogParams)
#ifndef GOURAUD_NUM_LIGHTS
UNITY_DEFINE_INSTANCED_PROP(float4, _StaticLightPos[512])
UNITY_DEFINE_INSTANCED_PROP(float4, _StaticLightDir[512])
UNITY_DEFINE_INSTANCED_PROP(float4, _StaticLightCol[512])
UNITY_DEFINE_INSTANCED_PROP(float4, _StaticLightParams[512])
#else
UNITY_DEFINE_INSTANCED_PROP(float4, _StaticLightPos[GOURAUD_NUM_LIGHTS])
UNITY_DEFINE_INSTANCED_PROP(float4, _StaticLightDir[GOURAUD_NUM_LIGHTS])
UNITY_DEFINE_INSTANCED_PROP(float4, _StaticLightCol[GOURAUD_NUM_LIGHTS])
UNITY_DEFINE_INSTANCED_PROP(float4, _StaticLightParams[GOURAUD_NUM_LIGHTS])
#endif
UNITY_DEFINE_INSTANCED_PROP(float, _StaticLightCount)
UNITY_INSTANCING_BUFFER_END(Props)
float _Luminosity = 0.5;
float _Saturate = 1.0;
float _DisableLighting = 0;
float _DisableLightingLocal = 0;
float _DisableFog = 0;

struct v2f {
	float4 pos : SV_POSITION;
	float4 uv1 : TEXCOORD0; // The 1st UV coordinate.
	float4 uv2 : TEXCOORD1; // The 2nd UV coordinate.
	float4 uv3 : TEXCOORD2; // The 3rd UV coordinate.
	float4 uv4 : TEXCOORD3; // The 4th UV coordinate.
	float4 diffuseColor : TEXCOORD4;
	float3 normal : TEXCOORD5;
	float3 multipliedPosition : TEXCOORD6;
	float3 fogViewPos : TEXCOORD7;
	UNITY_VERTEX_INPUT_INSTANCE_ID
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
	if(_DisableLighting == 1.0 || _DisableLightingLocal == 1.0) return float4(1.0, 1.0, 1.0, 1.0);

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
	float num_lights = UNITY_ACCESS_INSTANCED_PROP(Props, _StaticLightCount);
#ifdef GOURAUD_NUM_LIGHTS
	if (num_lights > GOURAUD_NUM_LIGHTS) num_lights = GOURAUD_NUM_LIGHTS;
#endif
#if defined(FALLBACK) && defined(GOURAUD_NUM_LIGHTS)
	for (int i = 0; i < GOURAUD_NUM_LIGHTS; i++) {
		if(i < num_lights) {
#else
	for (int i = 0; i < num_lights; i++) {
#endif
		float4 lightPos = UNITY_ACCESS_INSTANCED_PROP(Props, _StaticLightPos)[i];
		float4 lightDir = UNITY_ACCESS_INSTANCED_PROP(Props, _StaticLightDir)[i];
		float4 lightCol = UNITY_ACCESS_INSTANCED_PROP(Props, _StaticLightCol)[i];
		float4 lightParams = UNITY_ACCESS_INSTANCED_PROP(Props, _StaticLightParams)[i];
		if (lightPos.w == 1) {
			attenuation = 1.0; // no attenuation
			lightDirection = normalize(lightDir.xyz);
			/*if (lightParams.z == 0) {
				normalFactor = max(0.0, dot(normalDirection, lightDirection));
			} else normalFactor = 1.0;*/
			normalFactor = CalcNormalFactor(normalDirection, lightDirection);
			if (lightParams.w != 1) diffuseReflection = diffuseReflection + attenuation * lightCol.xyz * normalFactor;
			//* colRgb * _DiffuseCoef.xyz //* _DiffuseCoef.w
			if (lightParams.w != 2) alpha = alpha + lightCol.w * normalFactor;// * _DiffuseCoef.w;
		} else if (lightPos.w == 2) {
			vertexToLightSource = lightPos.xyz - multipliedPosition;
			distance = length(vertexToLightSource);
			far = lightParams.y;
			if (distance < far) {
				near = lightParams.x;
				attenuation = CalcSphereAttenuation(distance, near, far);
				if (lightParams.z == 0) {
					lightDirection = normalize(vertexToLightSource);
					normalFactor = CalcNormalFactor(normalDirection, lightDirection);
				} else normalFactor = 1.0; // Painting light
				if (normalFactor != 0) {
					if (lightParams.w != 1) diffuseReflection = diffuseReflection + attenuation * lightCol.xyz * normalFactor;
					//* colRgb * _DiffuseCoef.xyz //* _DiffuseCoef.w
					if (lightParams.w != 2) alpha = alpha + attenuation * lightCol.w * normalFactor;// * _DiffuseCoef.w;
				}
			}
		} else if (lightPos.w == 4) {
			if (lightParams.w != 1) ambient.xyz = ambient.xyz + lightCol.xyz * _DiffuseCoef.xyz;
			//* colRgb * _DiffuseCoef.xyz //* _DiffuseCoef.w
			if (lightParams.w != 2) ambient.w = ambient.w + lightCol.w * _DiffuseCoef.w;// * _DiffuseCoef.w;
		} else if (lightPos.w == 7) {
			vertexToLightSource = lightPos.xyz - multipliedPosition;
			distance = length(vertexToLightSource);
			far = lightParams.y;
			if (distance < far) {
				near = lightParams.x;
				attenuation = CalcSphereAttenuation(distance, near, far);
				if (lightParams.z == 0) {
					lightDirection = normalize(lightDir.xyz);
					normalFactor = CalcNormalFactor(normalDirection, lightDirection);
				} else normalFactor = 1.0; // Painting light
				if (normalFactor != 0) {
					if (lightParams.w != 1) diffuseReflection = diffuseReflection + attenuation * lightCol.xyz * normalFactor;
					//* colRgb * _DiffuseCoef.xyz //* _DiffuseCoef.w
					if (lightParams.w != 2) alpha = alpha + attenuation * lightCol.w * normalFactor;// *_DiffuseCoef.w;
				}
			}
		}
#if defined(FALLBACK) && defined(GOURAUD_NUM_LIGHTS)
	}
#endif
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
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v, o);
	o.uv1 = float4(TRANSFORM_TEX(TransformUV( v.texcoord.xy, _Tex0Params, _Tex0Params2), _Tex0),  v.texcoord.z,  v.texcoord.w);
	o.uv2 = float4(TRANSFORM_TEX(TransformUV(v.texcoord1.xy, _Tex1Params, _Tex1Params2), _Tex1), v.texcoord1.z, v.texcoord1.w);
	o.uv3 = float4(TRANSFORM_TEX(TransformUV(v.texcoord2.xy, _Tex2Params, _Tex2Params2), _Tex2), v.texcoord2.z, v.texcoord2.w);
	o.uv4 = float4(TRANSFORM_TEX(TransformUV(v.texcoord3.xy, _Tex3Params, _Tex3Params2), _Tex3), v.texcoord3.z, v.texcoord3.w);

	float4x4 modelMatrix = unity_ObjectToWorld;
	float4x4 modelMatrixInverse = unity_WorldToObject;
	float3 normalDirection, multipliedPosition;

	o.pos = UnityObjectToClipPos(v.vertex);
	multipliedPosition = mul(modelMatrix, v.vertex).xyz; // Vertex in world space
	normalDirection = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz); // Normal in object space

	float3 lightDirection;
	float attenuation;

	float alpha = 1.0;

	float3 ambientLighting = 0.0;

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
		//diffuseReflection = float3(1.0, 1.0, 1.0);
		diffuseReflection = _AmbientCoef.xyz +_DiffuseCoef.xyz;
	}
	if (/*_ShadingMode == 0.0 && */isAdd == 0.0 && isLight == 0.0) {
		float4 lightCol = ApplyStaticLights(normalDirection, multipliedPosition);
		diffuseReflection = diffuseReflection + lightCol.xyz;
		alpha = lightCol.w;
	}
	o.normal = normalDirection;
	o.multipliedPosition = multipliedPosition;
	o.diffuseColor = float4(ambientLighting + diffuseReflection, alpha);
	if (_Prelit == 1 && !(_DisableLighting == 1.0 || _DisableLightingLocal == 1.0)) {
		// Prelit
		//o.diffuseColor = v.texcoord2; // RGBA, so both need to be vector4
		o.diffuseColor = _DiffuseCoef * v.color; // RGBA, so both need to be vector4
	}
	// Process mirror or portal, this weird construction is so that it's faster
	if (_NumTextures > 0) {
		if (_NumTextures == 1) {
			if (_Tex0Params.z > 0) o.uv1 = ComputeScreenPos(o.pos);
		} else if (_NumTextures == 2) {
			if (_Tex1Params.z > 0) o.uv2 = ComputeScreenPos(o.pos);
		} else if (_NumTextures == 3) {
			if (_Tex2Params.z > 0) o.uv3 = ComputeScreenPos(o.pos);
		} else if (_NumTextures == 4) {
			if (_Tex3Params.z > 0) o.uv4 = ComputeScreenPos(o.pos);
		}
	}
	if (_SectorFog.w != 0) o.fogViewPos = UnityObjectToViewPos(v.vertex).xyz;
	return o;
}

float4 TextureOp(float4 color_in, float4 diffuseColor, sampler2D tex, float4 uv, float4 tex_params, float4 tex_params_2, float index) {
	//if (tex_params.w == 1 && index > 0) return color_in; // Mirrors are not supported yet
	if (tex_params.z > 0) { // mirror / portal
		if (tex_params.z == 1) { // mirror
			float4 reflColor = tex2Dproj(tex, UNITY_PROJ_COORD(uv));
			return color_in + diffuseColor * float4(reflColor.xyz, 1);
		} else { // portal
			float4 reflColor = tex2Dproj(tex, UNITY_PROJ_COORD(uv));
			return color_in + diffuseColor * float4(reflColor.xyz, 1);
		}
	}
	float4 texColor = tex2D(tex, uv.xy);
	if (tex_params.x == 1 && index > 0) {
		// Additive

		//return (min(color_in + texColor, float4(1, 1, 1, 1)) * uv.z + color_in * (1.0 - uv.z));
		return color_in + float4(texColor.xyz * texColor.w * uv.z, 0);
	/*} else if (tex_params.x == 1) {
		return (min(color_in + texColor, float4(1,1,1,1)) * uv.z + color_in * (1.0 - uv.z));*/
	} else if (tex_params.x == 2) {
		// Opaque
		/*if (index > 0) {
			return color_in -texColor * uv.z;
		} else {
			return lerp(color_in, diffuseColor * texColor, uv.z);
		}*/
		return lerp(color_in, diffuseColor * texColor, uv.z);
		//return color_in + texColor * diffuseColor;
		//return float4(lerp(color_in.xyz, ))
		//return color_in + float4(texColor.xyz, texColor.w * uv.z);
		//return lerp(color_in, float4(diffuseColor.xyz * texColor.xyz, texColor.w), uv.z * texColor.w);
	} else if (tex_params.x == 3) {
		// Transparent
		return lerp(color_in, diffuseColor * texColor, uv.z);
		//return lerp(color_in, diffuseColor * float4(texColor.xyz, 1), uv.z * texColor.w);
	} else if (tex_params.x == 4) {
		// Multiply
		return color_in * float4(texColor.xyz * texColor.w * uv.z, 1);
	} else if (tex_params.x == 5) {
		// Should be additive, but let's just lerp
		return lerp(color_in, diffuseColor * texColor, uv.z);
	} else if (tex_params.x == 6) {
		// Lightmap
		return color_in + float4(texColor.xyz, 0);
	} else if (tex_params.x != 50 && tex_params.x != 51 && tex_params.x != 52) {
		return lerp(color_in, diffuseColor * texColor, uv.z);
		//color_out.a = color_out.a = color_out.a * i.diffuseColor.w;
		//return diffuseColor * color_out;
	} else {
		// Lightmap (custom)
		return color_in + float4(texColor.xyz, 0);
	}
}

float4 process_frag(v2f i, float clipAlpha, float isAdd) : SV_TARGET {
	UNITY_SETUP_INSTANCE_ID(i);
	float4 c = float4(0.0, 0.0, 0.0, 0.0);
	if (_NumTextures > 1 && _Tex1Params.x == 50 && _DisableLighting != 1.0 && _DisableLightingLocal != 1.0) {
		/*float4 diffuseReflection = i.diffuseColor;
		diffuseReflection = TextureOp(diffuseReflection, i.diffuseColor, _Tex1, i.uv2, _Tex1Params, _Tex1Params2, 1);

		if (_Saturate == 1.0) {
			diffuseReflection.x = saturate(diffuseReflection.x);
			diffuseReflection.y = saturate(diffuseReflection.y);
			diffuseReflection.z = saturate(diffuseReflection.z);
			diffuseReflection.w = saturate(diffuseReflection.w);
		}
		i.diffuseColor = diffuseReflection;*/
		float4 diffuseReflection = float4(_Luminosity - 0.5, _Luminosity - 0.5, _Luminosity - 0.5, 1);
		diffuseReflection = TextureOp(diffuseReflection, i.diffuseColor, _Tex1, i.uv2, _Tex1Params, _Tex1Params2, 1);
		diffuseReflection = diffuseReflection * 2;

		if (_Saturate == 1.0) {
			diffuseReflection.x = saturate(diffuseReflection.x);
			diffuseReflection.y = saturate(diffuseReflection.y);
			diffuseReflection.z = saturate(diffuseReflection.z);
			diffuseReflection.w = saturate(diffuseReflection.w);
		}
		i.diffuseColor = diffuseReflection;
	}
	if (_NumTextures > 0) {
		c = TextureOp(c, i.diffuseColor, _Tex0, i.uv1, _Tex0Params, _Tex0Params2, 0);
		if (_NumTextures > 1 && _Tex1Params.x != 50) {
			c = TextureOp(c, i.diffuseColor, _Tex1, i.uv2, _Tex1Params, _Tex1Params2, 1);
			if (_NumTextures > 2) {
				c = TextureOp(c, i.diffuseColor, _Tex2, i.uv3, _Tex2Params, _Tex2Params2, 2);
				if (_NumTextures > 3) {
					c = TextureOp(c, i.diffuseColor, _Tex3, i.uv4, _Tex3Params, _Tex3Params2, 3);
				}
			}
		}
	}
	/*float maxColor = max(max(max(c.x, c.y), c.z), c.w);
	if (maxColor > 1.0) {
		c = c / maxColor;
	}*/

	if (clipAlpha < 0) {
		clip(clipAlpha * (c.a - 1.0));
	} else {
		clip(clipAlpha * (c.a - 0.999));
	}

	if (_DisableFog != 1.0) {
		// Add fog
		float4 sectFog = UNITY_ACCESS_INSTANCED_PROP(Props, _SectorFog);
		if (sectFog.w != 0) {
			float4 sectFogParams = UNITY_ACCESS_INSTANCED_PROP(Props, _SectorFogParams);
			float fog;
			if (sectFogParams.x != sectFogParams.y) { // Blend near != Blend far
				float fogz = length(i.fogViewPos);
				fog = sectFogParams.x +
					saturate((fogz - sectFogParams.z) / (sectFogParams.w - sectFogParams.z))
					* saturate(sectFogParams.y - sectFogParams.x); // Otherwise fog can cause increased brightness for values above 1f
			} else {
				fog = sectFogParams.y;
			}
			if (isAdd == 1.0) {
				c.rgb = lerp(c.rgb, float3(0, 0, 0), fog * sectFog.w);
			} else {
				c.rgb = lerp(c.rgb, sectFog.xyz, fog * sectFog.w);
			}
		}
	}
	return c;
}
#endif // SHARED_GOURAUD