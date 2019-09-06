Shader "Time of Day/Skybox"
{
	Properties
	{
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "HTOD_Base.cginc"
	#define HTOD_SCATTERING_MIE 0
	#include "HTOD_Scattering.cginc"

	struct v2f
	{
		float4 position : SV_POSITION;
		float3 color    : TEXCOORD0;
	};

	v2f vert(appdata_base v)
	{
		v2f o;

		o.position = HTOD_TRANSFORM_VERT(v.vertex);

		float3 vertex = normalize(mul((float3x3)HTOD_World2Sky, mul((float3x3)HTOD_Object2World, v.vertex.xyz)));

		o.color = (vertex.y < 0) ? pow(HTOD_GroundColor, HTOD_Contrast) : ScatteringColor(vertex.xyz).rgb;

#if !HTOD_OUTPUT_HDR
		o.color = HTOD_HDR2LDR(o.color);
#endif

#if !HTOD_OUTPUT_LINEAR
		o.color = HTOD_LINEAR2GAMMA(o.color);
#endif

		return o;
	}

	float4 frag(v2f i) : COLOR
	{
		return half4(i.color, 1);
	}
	ENDCG

	SubShader
	{
		Tags
		{
			"Queue"="Background"
			"RenderType"="Background"
			"PreviewType"="Skybox"
		}

		Pass
		{
			Cull Off
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ HTOD_OUTPUT_HDR
			#pragma multi_compile _ HTOD_OUTPUT_LINEAR
			ENDCG
		}
	}

	Fallback Off
}
