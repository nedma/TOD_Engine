Shader "Time of Day/Atmosphere"
{
	Properties
	{
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "HTOD_Base.cginc"
	#include "HTOD_Scattering.cginc"

	struct v2f
	{
		float4 position   : SV_POSITION;
#if HTOD_OUTPUT_DITHERING
		float2 texcoord   : TEXCOORD0;
#endif
#if HTOD_SCATTERING_PER_PIXEL
		float3 inscatter  : TEXCOORD1;
		float3 outscatter : TEXCOORD2;
		float3 viewDir    : TEXCOORD3;
#else
		float4 color      : TEXCOORD1;
#endif
	};

	float4 Adjust(float4 color)
	{
#if !HTOD_OUTPUT_HDR
		color = HTOD_HDR2LDR(color);
#endif

#if !HTOD_OUTPUT_LINEAR
		color = HTOD_LINEAR2GAMMA(color);
#endif

		return color;
	}

	v2f vert(appdata_base v)
	{
		v2f o;

		o.position = HTOD_TRANSFORM_VERT(v.vertex);

		float3 vertnorm = normalize(v.vertex.xyz);

#if HTOD_SCATTERING_PER_PIXEL
		o.viewDir = vertnorm;
		ScatteringCoefficients(o.viewDir, o.inscatter, o.outscatter);
#else
		o.color = Adjust(ScatteringColor(vertnorm));
#endif

#if HTOD_OUTPUT_DITHERING
		float4 projPos = ComputeScreenPos(o.position);
		o.texcoord = DitheringCoords(projPos.xy / projPos.w);
#endif

		return o;
	}

	float4 frag(v2f i) : COLOR
	{
#if HTOD_SCATTERING_PER_PIXEL
		float4 color = Adjust(ScatteringColor(normalize(i.viewDir), i.inscatter, i.outscatter));
#else
		float4 color = i.color;
#endif

#if HTOD_OUTPUT_DITHERING
		color.rgb += DitheringColor(i.texcoord);
#endif

		return float4(color.rgb, 0);
	}
	ENDCG

	SubShader
	{
		Tags
		{
			"Queue"="Background+50"
			"RenderType"="Background"
			"IgnoreProjector"="True"
		}

		Pass
		{
			ZWrite Off
			ZTest LEqual
			Blend One One
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ HTOD_OUTPUT_HDR
			#pragma multi_compile _ HTOD_OUTPUT_LINEAR
			#pragma multi_compile _ HTOD_OUTPUT_DITHERING
			#pragma multi_compile _ HTOD_SCATTERING_PER_PIXEL
			ENDCG
		}
	}

	Fallback Off
}
