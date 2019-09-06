Shader "Time of Day/Cloud Billboard Far"
{
	Properties
	{
		_MainTex ("Density Map (RGBA)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "HTOD_Base.cginc"
	#include "HTOD_Clouds.cginc"
	#include "HTOD_Scattering.cginc"

	uniform sampler2D _MainTex;
	uniform sampler2D _BumpMap;

	uniform float4 _MainTex_ST;
	uniform float4 _BumpMap_ST;

	struct v2f
	{
		float4 position : SV_POSITION;
		float4 color    : TEXCOORD0;
		float4 texcoord : TEXCOORD1;
		float3 viewDir  : TEXCOORD2;
		float3 lightDir : TEXCOORD3;
		float3 lightCol : TEXCOORD4;
	};

	v2f vert(appdata_tan v)
	{
		v2f o;

		o.position = HTOD_TRANSFORM_VERT(v.vertex);

		o.viewDir  = normalize(mul(HTOD_Object2World, v.vertex).xyz);
		o.lightDir = HTOD_SunDirection;

		o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.texcoord.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

		float3 inscatter, outscatter;
		ScatteringCoefficients(o.viewDir, inscatter, outscatter);

		float3 cloudColor = CloudColor(o.viewDir, o.lightDir);

		float sunCos  = dot(o.lightDir, o.viewDir);
		float sunCos2 = sunCos * sunCos;

		float3 nightScattering    = NightPhase(o.viewDir);
		float3 moonScattering     = MoonPhase(o.viewDir);
		float3 rayleighScattering = RayleighPhase(sunCos2) * inscatter;
		float3 mieScattering      = MiePhase(sunCos, sunCos2) * CloudPhase(sunCos, sunCos2) * outscatter;

		float3 dirColor  = PostProcess(moonScattering + mieScattering, o.viewDir);
		float3 baseColor = PostProcess(nightScattering + rayleighScattering, o.viewDir);

		o.lightCol  = dirColor;
		o.color.rgb = HTOD_CloudBrightness * lerp(baseColor, cloudColor, HTOD_CloudColoring);
		o.color.a   = HTOD_CloudOpacity;

		TANGENT_SPACE_ROTATION;
		o.viewDir  = normalize(mul(rotation, o.viewDir));
		o.lightDir = normalize(mul(rotation, o.lightDir));

#if !HTOD_CLOUDS_BUMPED

#if !HTOD_OUTPUT_HDR
		o.color.rgb = HTOD_HDR2LDR(o.color.rgb);
		o.lightCol.rgb = HTOD_HDR2LDR(o.lightCol.rgb);
#endif

#if !HTOD_OUTPUT_LINEAR
		o.color.rgb = HTOD_LINEAR2GAMMA(o.color.rgb);
		o.lightCol.rgb = HTOD_LINEAR2GAMMA(o.lightCol.rgb);
#endif

#endif

		return o;
	}

	half4 frag(v2f i) : COLOR
	{
		half4 color = CloudBillboardColor(_MainTex, _BumpMap, i.texcoord, i.color, i.viewDir, i.lightDir, i.lightCol);

#if HTOD_CLOUDS_BUMPED

#if !HTOD_OUTPUT_HDR
		color.rgb = HTOD_HDR2LDR(color.rgb);
#endif

#if !HTOD_OUTPUT_LINEAR
		color.rgb = HTOD_LINEAR2GAMMA(color.rgb);
#endif

#endif

		return color;
	}
	ENDCG

	SubShader
	{
		Tags
		{
			"Queue"="Geometry+520"
			"RenderType"="Background"
			"IgnoreProjector"="True"
		}

		Pass
		{
			ZWrite Off
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ HTOD_OUTPUT_HDR
			#pragma multi_compile _ HTOD_OUTPUT_LINEAR
			#pragma multi_compile _ HTOD_CLOUDS_DENSITY
			#pragma multi_compile _ HTOD_CLOUDS_BUMPED
			ENDCG
		}
	}

	Fallback Off
}
