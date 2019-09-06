Shader "Time of Day/Stars"
{
	Properties
	{
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "HTOD_Base.cginc"

	struct v2f
	{
		float4 position : SV_POSITION;
		half3  color    : COLOR;
		half3  tex      : TEXCOORD0;
	};

	v2f vert(appdata_full v)
	{
		v2f o;

		float tanFovHalf = 1.0 / max(0.1, UNITY_MATRIX_P[0][0]); // (r-l) / (2*n)
		float radius = 4.0 * tanFovHalf / _ScreenParams.x;
		float alpha = HTOD_StarBrightness * HTOD_StarVisibility * 0.00001 * v.color.a / (radius * radius);
		float size = HTOD_StarSize * radius;

		float3 u_vec = v.tangent;
		float3 v_vec = cross(v.normal, v.tangent);

		float u_fac = v.texcoord.x - 0.5;
		float v_fac = v.texcoord.y - 0.5;

		v.vertex.xyz -= u_vec * u_fac * size;
		v.vertex.xyz -= v_vec * v_fac * size;

		o.position = HTOD_TRANSFORM_VERT(v.vertex);

		float3 skyPos = mul(HTOD_World2Sky, mul(HTOD_Object2World, v.vertex)).xyz;

		o.tex.xy = 2.0 * v.texcoord - 1.0;
		o.tex.z  = skyPos.y * 25;

		o.color = half3(alpha, alpha, alpha);

#if !HTOD_OUTPUT_LINEAR
		o.color = HTOD_LINEAR2GAMMA(o.color);
#endif

		return o;
	}

	half4 frag(v2f i) : COLOR
	{
		half  dist  = length(i.tex.xy);
		half  spot  = saturate(1.0 - dist);
		half  alpha = saturate(i.tex.z) * spot;

		return half4(i.color * alpha, 0);
	}
	ENDCG

	SubShader
	{
		Tags
		{
			"Queue"="Background+20"
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
			#pragma multi_compile _ HTOD_OUTPUT_LINEAR
			ENDCG
		}
	}

	Fallback Off
}
