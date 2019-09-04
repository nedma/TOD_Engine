// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Time of Day/Cloud PBR"
{
    Properties
    {
		_SCloud("Cloud Texture RGBA", 2D) = "white" {}
    }
    SubShader
    {
		LOD 300

        Tags
        {
            "Queue" = "Geometry+1"
            "RenderType"="Opaque"
        }

        Fog
        {
            Mode Off
        }

        Pass
        {
            Cull Front
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
			#pragma glsl
			#pragma target 3.0
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma glsl_no_auto_normalization
			#pragma multi_compile DUMMY USING_RT

            #pragma vertex vert
            #pragma fragment frag

			#include "UnityCG.cginc"
			#include "Atmosphere.cginc"

			struct v2f 
			{
                float4 position : SV_POSITION;
				float4 suvpack0 : TEXCOORD0;
				float4 suvpack1 : TEXCOORD1;
				float4 suvpack2 : TEXCOORD2;
				float4 suvpack3 : TEXCOORD3;
				half fade : TEXCOORD4;
				half4 acolor : TEXCOORD5;
            };


            v2f vert(appdata_base v)
			{
			// to calculate one-pass atmosphere color
                v2f o;

                float3 T_val  = T(v.normal.y);
                float3 E_sun  = TOD_SunColor;
                float3 L_sun  = L(-v.normal, TOD_LocalSunDirection);

				half4 acolor;
                acolor.rgb = (1-T_val) * (E_sun*L_sun);
                acolor.a   = 10 * max(max(acolor.r, acolor.g), acolor.b);
                acolor += TOD_AdditiveColor;
                acolor.rgb = lerp(acolor.rgb, TOD_CloudColor, _Fogginess);
                acolor.a += _Fogginess;
                acolor.a = saturate(acolor.a);
                acolor.rgb = pow(acolor.rgb, _Contrast);
				o.acolor = acolor;

                o.position   = UnityObjectToClipPos(v.vertex);
				//o.position.z = o.position.w * 0.99999999;// Make it at the farplane
#ifdef UNITY_REVERSED_Z
				o.position.z = 0;// Make it at the farplane
#else
				o.position.z = o.position.w;// Make it at the farplane
#endif

				float3 vertnorm = normalize(v.vertex.xyz);
				o.fade = saturate(100 * vertnorm.y * vertnorm.y * vertnorm.y);

				float2 puv = o.position.xy / o.position.w;
				puv = puv * 0.5 + float2(0.5, 0.5); // unity pitfall , the y
				#if USING_RT && UNITY_UV_STARTS_AT_TOP
				puv.y = 1.0-puv.y;
				#endif
				float2 blurdir = normalize(puv * _CloudVLitOffset.xy + _CloudVLitOffset.zw) * 0.009f;
				o.suvpack0 = float4(GetCloudUv(puv + blurdir * 0, _CloudSpeed, _CloudScale), GetCloudUv(puv + blurdir * 1, _CloudSpeed, _CloudScale));
				o.suvpack1 = float4(GetCloudUv(puv + blurdir * 2, _CloudSpeed, _CloudScale), GetCloudUv(puv + blurdir * 3, _CloudSpeed, _CloudScale));
				o.suvpack2 = float4(GetCloudUv(puv + blurdir * 4, _CloudSpeed, _CloudScale), GetCloudUv(puv + blurdir * 5, _CloudSpeed, _CloudScale));
				o.suvpack3 = float4(GetCloudUv(puv + blurdir * 6, _CloudSpeed, _CloudScale), GetCloudUv(puv + blurdir * 7, _CloudSpeed, _CloudScale));


                return o;
            }


            half4 frag(v2f i) : COLOR 
			{
				half scaleCoeff = 0.6f;

				half4 orgColor = GetCloudDensity(tex2D(_SCloud, i.suvpack0.xy));

                half4 clDensity = orgColor; // 0
				clDensity += GetCloudDensity(tex2D(_SCloud, i.suvpack0.zw) * scaleCoeff); // 1
				clDensity += GetCloudDensity(tex2D(_SCloud, i.suvpack1.xy) * scaleCoeff); // 2
				clDensity += GetCloudDensity(tex2D(_SCloud, i.suvpack1.zw) * scaleCoeff); // 3
				clDensity += GetCloudDensity(tex2D(_SCloud, i.suvpack2.xy) * scaleCoeff); // 4
				clDensity += GetCloudDensity(tex2D(_SCloud, i.suvpack2.zw) * scaleCoeff); // 5
				clDensity += GetCloudDensity(tex2D(_SCloud, i.suvpack3.xy) * scaleCoeff); // 6
				clDensity += GetCloudDensity(tex2D(_SCloud, i.suvpack3.zw) * scaleCoeff); // 7
				clDensity /= 8.0;

				half4 clLit = 1.0-clDensity;

				half darkCloudCoeff = distance(clLit.rgb, half3(1,1,1)); // evaluate the distance from the cloud color, actually this formular is not linear, but just fine here
				clLit.rgb = lerp(clLit.rgb, i.acolor.rgb*0.9, i.acolor.a*darkCloudCoeff*0.7*_SunnyDegree);

				// to light cloud
				half3 clCloud = clLit.rgb * TOD_LightColor * _CloudIntensity;

				half4 color;
				color.rgb = clCloud.rgb;
				orgColor.a = smoothstep(0, 1, orgColor.a);
				color.a = orgColor.a * i.fade;

				color.rgb = pow(color.rgb, lerp(half3(0.9, 0.72, 0.56), half3(0.8, 0.72, 0.6), _SunnyDegree));

				color.rgb = lerp(i.acolor, color.rgb, color.a);

				return color;
            }

            ENDCG
        }
    }

    SubShader
    {
		LOD 200

        Tags
        {
            "Queue" = "Geometry+1"
            "RenderType"="Opaque"
        }

        Fog
        {
            Mode Off
        }

        Pass
        {
            Cull Front
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
			#pragma glsl
			#pragma target 3.0
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma glsl_no_auto_normalization
			#pragma multi_compile DUMMY USING_RT

            #pragma vertex vert
            #pragma fragment frag

			#include "UnityCG.cginc"
			#include "Atmosphere.cginc"

			struct v2f 
			{
                float4 position : SV_POSITION;
				float4 suvpack0 : TEXCOORD0;
				float4 suvpack1 : TEXCOORD1;
				half fade : TEXCOORD4;
				half4 acolor : TEXCOORD5;
            };


            v2f vert(appdata_base v)
			{
			// to calculate one-pass atmosphere color
                v2f o;

                float3 T_val  = T(v.normal.y);
                float3 E_sun  = TOD_SunColor;
                float3 L_sun  = L(-v.normal, TOD_LocalSunDirection);

				half4 acolor;
                acolor.rgb = (1-T_val) * (E_sun*L_sun);
                acolor.a   = 10 * max(max(acolor.r, acolor.g), acolor.b);
                acolor += TOD_AdditiveColor;
                acolor.rgb = lerp(acolor.rgb, TOD_CloudColor, _Fogginess);
                acolor.a += _Fogginess;
                acolor.a = saturate(acolor.a);
                acolor.rgb = pow(acolor.rgb, _Contrast);
				o.acolor = acolor;

                o.position   = UnityObjectToClipPos(v.vertex);
				//o.position.z = o.position.w * 0.99999999;// Make it at the farplane
#ifdef UNITY_REVERSED_Z
				o.position.z = 0;// Make it at the farplane
#else
				o.position.z = o.position.w;// Make it at the farplane
#endif

				float3 vertnorm = normalize(v.vertex.xyz);
				o.fade = saturate(100 * vertnorm.y * vertnorm.y * vertnorm.y);

				float2 puv = o.position.xy / o.position.w;
				puv = puv * 0.5 + float2(0.5, 0.5); // unity pitfall , the y
				#if USING_RT && UNITY_UV_STARTS_AT_TOP
				puv.y = 1.0-puv.y;
				#endif
				float2 blurdir = normalize(puv * _CloudVLitOffset.xy + _CloudVLitOffset.zw) * 0.018f;
				o.suvpack0 = float4(GetCloudUv(puv + blurdir * 0, _CloudSpeed, _CloudScale), GetCloudUv(puv + blurdir * 1, _CloudSpeed, _CloudScale));
				o.suvpack1 = float4(GetCloudUv(puv + blurdir * 2, _CloudSpeed, _CloudScale), GetCloudUv(puv + blurdir * 3, _CloudSpeed, _CloudScale));
                return o;
            }


            half4 frag(v2f i) : COLOR 
			{
				half scaleCoeff = 0.6f;

				half4 orgColor = GetCloudDensity(tex2D(_SCloud, i.suvpack0.xy));

                half4 clDensity = orgColor; // 0
				clDensity += GetCloudDensity(tex2D(_SCloud, i.suvpack0.zw) * scaleCoeff); // 1
				clDensity += GetCloudDensity(tex2D(_SCloud, i.suvpack1.xy) * scaleCoeff); // 2
				clDensity += GetCloudDensity(tex2D(_SCloud, i.suvpack1.zw) * scaleCoeff); // 3
				clDensity /= 4.0;

				half4 clLit = 1.0-clDensity;

				half darkCloudCoeff = distance(clLit.rgb, half3(1,1,1)); // evaluate the distance from the cloud color, actually this formular is not linear, but just fine here
				clLit.rgb = lerp(clLit.rgb, i.acolor.rgb*0.9, i.acolor.a*darkCloudCoeff*0.7*_SunnyDegree);

				// to light cloud
				half3 clCloud = clLit.rgb * TOD_LightColor * _CloudIntensity;

				half4 color;
				color.rgb = clCloud.rgb;
				orgColor.a = smoothstep(0, 1, orgColor.a);
				color.a = orgColor.a * i.fade;

				color.rgb = pow(color.rgb, lerp(half3(0.9, 0.72, 0.56), half3(0.8, 0.72, 0.6), _SunnyDegree));

				color.rgb = lerp(i.acolor, color.rgb, color.a);

				return color;
            }

            ENDCG
        }
    }
    SubShader
    {
		LOD 100

        Tags
        {
            "Queue" = "Geometry+1"
            "RenderType"="Opaque"
        }

        Fog
        {
            Mode Off
        }

        Pass
        {
            Cull Front
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
			#pragma glsl
			#pragma target 3.0
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma glsl_no_auto_normalization
			#pragma multi_compile DUMMY USING_RT

            #pragma vertex vert
            #pragma fragment frag

			#include "UnityCG.cginc"
			#include "Atmosphere.cginc"

			struct v2f 
			{
                float4 position : SV_POSITION;
				float2 uv : TEXCOORD1;
				half fade : TEXCOORD4;
				half4 acolor : TEXCOORD5;
            };

            v2f vert(appdata_base v)
			{
			// to calculate one-pass atmosphere color
                v2f o;

                float3 T_val  = T(v.normal.y);
                float3 E_sun  = TOD_SunColor;
                float3 L_sun  = L(-v.normal, TOD_LocalSunDirection);

				half4 acolor;
                acolor.rgb = (1-T_val) * (E_sun*L_sun);
                acolor.a   = 10 * max(max(acolor.r, acolor.g), acolor.b);
                acolor += TOD_AdditiveColor;
                acolor.rgb = lerp(acolor.rgb, TOD_CloudColor, _Fogginess);
                acolor.a += _Fogginess;
                acolor.a = saturate(acolor.a);
                acolor.rgb = pow(acolor.rgb, _Contrast);
				o.acolor = acolor;

                o.position   = UnityObjectToClipPos(v.vertex);
				//o.position.z = o.position.w * 0.99999999;// Make it at the farplane
#ifdef UNITY_REVERSED_Z
				o.position.z = 0;// Make it at the farplane
#else
				o.position.z = o.position.w;// Make it at the farplane
#endif

				float3 vertnorm = normalize(v.vertex.xyz);
				o.fade = saturate(100 * vertnorm.y * vertnorm.y * vertnorm.y) * 0.57;

				o.uv = GetCloudUvMMM(v.vertex.xyz, _CloudSpeed, _CloudScale);

                return o;
            }


            half4 frag(v2f i) : COLOR 
			{
				half4 orgColor = GetCloudDensity(tex2D(_SCloud, i.uv));
				half3 clCloud = TOD_LightColor * _CloudIntensity;

				clCloud = lerp(i.acolor.rgb*0.5, clCloud, max(_SunnyDegree, 0.3));

				half4 color;
				color.rgb = clCloud.rgb;
				color.a = orgColor.a * i.fade;

				color.rgb = lerp(i.acolor, color.rgb, color.a);

				return color;
            }

            ENDCG
        }
    }

    Fallback off
}
