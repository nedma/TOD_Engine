Shader "Hidden/Time of Day/Scattering"
{
	Properties
	{
		_MainTex ("Base", 2D) = "white" {}
		_SkyMask ("Sky", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "HTOD_Base.cginc"
	#include "HTOD_Scattering.cginc"

	uniform sampler2D _MainTex;
	uniform sampler2D _SkyMask;
	uniform sampler2D_float _CameraDepthTexture;

	uniform float4x4 _FrustumCornersWS;
	uniform float4 _MainTex_TexelSize;
	uniform float4 _MainTex_ST;
	uniform float4 _SkyMask_ST;
	uniform float4 _CameraDepthTexture_ST;

	struct v2f
	{
		float4 pos       : SV_POSITION;
		float2 uv        : TEXCOORD0;
		float2 uv_depth  : TEXCOORD2;
#if HTOD_OUTPUT_DITHERING
		float2 uv_dither : TEXCOORD3;
#endif
		float4 cameraRay : TEXCOORD4;
		HTOD_VERTEX_OUTPUT_STEREO
	};

	v2f vert(appdata_img v)
	{
		v2f o;

		o.pos = HTOD_TRANSFORM_VERT(v.vertex);

		o.uv        = v.texcoord.xy;
		o.uv_depth  = v.texcoord.xy;

#if HTOD_OUTPUT_DITHERING
		o.uv_dither = DitheringCoords(v.texcoord.xy);
#endif

#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1-o.uv.y;
#endif

		int frustumIndex = v.texcoord.x + (2 * o.uv.y);
		o.cameraRay = _FrustumCornersWS[frustumIndex];
		o.cameraRay.w = frustumIndex;

		return o;
	}

	half4 frag(v2f i) : COLOR
	{
		half4 color = tex2D(_MainTex, HTOD_UV(i.uv, _MainTex_ST));

		float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, HTOD_UV(i.uv_depth, _CameraDepthTexture_ST));
		float depth = Linear01Depth(rawDepth);
		float4 cameraToWorldPos = depth * i.cameraRay;
		float3 worldPos = _WorldSpaceCameraPos + cameraToWorldPos;

		half4 mask = tex2D(_SkyMask, HTOD_UV(i.uv, _SkyMask_ST));
		half4 scattering = AtmosphericScattering(i.cameraRay, worldPos, depth, mask);

#if HTOD_OUTPUT_DITHERING
		scattering.rgb += DitheringColor(i.uv_dither);
#endif

#if !HTOD_OUTPUT_HDR
		scattering.rgb = HTOD_HDR2LDR(scattering.rgb);
#endif

#if !HTOD_OUTPUT_LINEAR
		scattering.rgb = HTOD_LINEAR2GAMMA(scattering.rgb);
#endif

		if (depth == 1)
		{
#if HTOD_SCATTERING_SINGLE_PASS
			color.rgb += scattering.rgb;
#endif
		}
		else
		{
			color.rgb = lerp(color.rgb, scattering.rgb, scattering.a);
		}

		return color;
	}
	ENDCG

	SubShader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ HTOD_OUTPUT_HDR
			#pragma multi_compile _ HTOD_OUTPUT_LINEAR
			#pragma multi_compile _ HTOD_OUTPUT_DITHERING
			#pragma multi_compile _ HTOD_SCATTERING_SINGLE_PASS
			ENDCG
		}
	}

	Fallback Off
}
