#ifndef SHARED_TEXTUREBLEND
#define SHARED_TEXTUREBLEND

// User-specified properties
uniform float4 _Color;
uniform float4 _AmbientCoef;
uniform float4 _DiffuseCoef;
uniform float4 _SpecularCoef;
uniform float _SpecularFactor;
uniform float4 _EmissionColor;
sampler2D _MainTex;
sampler2D _MainTex2;
float _UVSec;
float _Blend;
float _ShadingMode;
half _Glossiness;
half _Metallic;

// Lighting
uniform float4 _SectorAmbient;
uniform float4 _SectorFog;
uniform float4 _SectorFogParams;
float4 _StaticLightPos[128];
float4 _StaticLightDir[128];
float4 _StaticLightCol[128];
float4 _StaticLightParams[128];
float _StaticLightCount = 0;

struct Input {
	float2 uv_MainTex;
	float2 uv2_MainTex2;
	float blendValue;
	half fog;
};

void process_vert(inout appdata_full v, out Input o) {
	UNITY_INITIALIZE_OUTPUT(Input, o);
	o.blendValue = v.texcoord.z;
	float fogz = length(WorldSpaceViewDir(v.vertex));
	o.fog = saturate((fogz - _SectorFogParams.z) / (_SectorFogParams.w - _SectorFogParams.z));
}


void process_color(Input IN, SurfaceOutputStandard o, inout fixed4 color) {
	float3 colRgb = _Color.rgb * _Color.w + float3(1.0, 1.0, 1.0) * (1.0 - _Color.w);
	float3 ambientLighting = float3(0.0, 0.0, 0.0);
#ifndef UNITY_PASS_FORWARDADD
	//ambientLighting = _AmbientCoef.xyz * colRgb;
	ambientLighting =( _SectorAmbient.rgb * _AmbientCoef.w + _AmbientCoef.xyz * colRgb) * o.Albedo;
#endif
	float3 diffuseReflection = colRgb * _DiffuseCoef.xyz; //* _DiffuseCoef.w
	color.rgb = color.rgb * diffuseReflection + ambientLighting;

	// Add fog
	if (_SectorFog.w == 1) {
		fixed3 fogColor = _SectorFog.xyz;
#ifdef UNITY_PASS_FORWARDADD
		fogColor = 0;
#endif
		color.rgb = lerp(color.rgb, fogColor, IN.fog);
	}
}

void process_surf(Input IN, inout SurfaceOutputStandard o, float clipAlpha) {
	fixed2 uv1 = IN.uv_MainTex;
	fixed2 uv2 = IN.uv2_MainTex2;
	float blendfactor = IN.blendValue;
	if (_UVSec == 0) {
		uv2 = uv1;
	}
	float4 c;
	if (_Blend == 1) {
		c = lerp(tex2D(_MainTex, uv1), tex2D(_MainTex2, uv2), blendfactor);
	} else {
		c = tex2D(_MainTex, uv1);
	}
	clip(clipAlpha * (c.a - 1.0));
	c.rgb = c.rgb * (1 + (_EmissionColor.rgb * 2 * _EmissionColor.a));
	

	o.Albedo = c;
	// Metallic and smoothness come from slider variables
	o.Metallic = _Metallic;
	o.Smoothness = _Glossiness;
	o.Alpha = c.a;

}
#endif // SHARED_TEXTUREBLEND