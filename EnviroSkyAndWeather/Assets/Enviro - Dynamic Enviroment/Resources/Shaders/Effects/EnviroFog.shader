// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Enviro/Fog" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "black" {}
	_Clouds ("Clouds", 2D) = "black" {}
	_Background ("Background", 2D) = "black" {}
}

CGINCLUDE

	#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	uniform sampler2D_float _CameraDepthTexture;
	uniform float4 _HeightParams;
	uniform float4 _DistanceParams;
	uniform float4 _FogColorTint;
	int4 _SceneFogMode;
	float4 _SceneFogParams;
	#ifndef UNITY_APPLY_FOG
	half4 unity_FogColor;
	half4 unity_FogDensity;
	#endif	
	uniform float4 _MainTex_TexelSize;
	uniform float4x4 _FrustumCornersWS;
	uniform float4 _CameraWS;

	// Scattering
	uniform float3 _Br;
	uniform float3 _Bm;
	uniform float3 _mieG;
	uniform float _SunIntensity;
	uniform float _Exposure;
	uniform float _SkyLuminance;
	uniform float _scatteringPower;
	uniform float _SunDiskSize;
	uniform float _SunDiskIntensity;
	uniform float4 _scatteringColor;
	uniform float4 _sunDiskColor;
	uniform float _SkyColorPower;
	uniform float3 _SunDir;
	uniform float _scatteringStrenght;
	uniform float _noiseScale;
	uniform half _noiseIntensity;
	uniform half _distanceFogIntensity;
	uniform half _heightFogIntensity;
	uniform half _skyFogIntensity;
	uniform float _maximumFogDensity;
	uniform float4 _weatherSkyMod;
	uniform float4 _weatherFogMod;

	uniform sampler2D _Clouds;
	uniform sampler2D _Background;
	uniform float _SkyFogHeight;
	uniform float _SunBlocking;
	uniform float _hdr;

	uniform sampler3D _NoiseTexture;
	// x: scale, y: intensity, z: intensity offset
    uniform float4 _NoiseData;
    // x: x velocity, y: z velocity
    uniform float4 _NoiseVelocity;

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 uv_depth : TEXCOORD1;
		float4 interpolatedRay : TEXCOORD2;
		float3 sky : TEXCOORD3;
	};
	
	v2f vert (appdata_img v)
	{
		v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		half index = v.vertex.z;
		v.vertex.z = 0.1;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		o.uv_depth = v.texcoord.xy;

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1-o.uv.y;
		#endif				
		
		o.interpolatedRay = _FrustumCornersWS[(int)index];
		o.interpolatedRay.w = index;

		o.sky.x = saturate( _SunDir.y + 0.25 );   
		o.sky.y = saturate(clamp(1.0 - _SunDir.y,0.0,0.5));

		return o;
	}

	// Applies one of standard fog formulas, given fog coordinate (i.e. distance)
	half ComputeFogFactor (float coord)
	{
		float fogFac = 0.0;
		if (_SceneFogMode.x == 1) // linear
		{
			// factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
			fogFac = coord * _SceneFogParams.z + _SceneFogParams.w;
		}
		if (_SceneFogMode.x == 2) // exp
		{
			// factor = exp(-density*z)
			fogFac = _SceneFogParams.y * coord; fogFac = exp2(-fogFac);
		}
		if (_SceneFogMode.x == 3) // exp2
		{
			// factor = exp(-(density*z)^2)
			fogFac = _SceneFogParams.x * coord; fogFac = exp2(-fogFac*fogFac);
		}
		return saturate(fogFac);
	}

	// Distance-based fog
	float ComputeDistance (float3 camDir, float zdepth)
	{
		float dist; 
		if (_SceneFogMode.y == 1)
			dist = length(camDir);
		else
			dist = zdepth * _ProjectionParams.z;
		// Built-in fog starts at near plane, so match that by
		// subtracting the near value. Not a perfect approximation
		// if near plane is very large, but good enough.
		dist -= _ProjectionParams.y;
		return dist;
	}

	// Linear half-space fog, from https://www.terathon.com/lengyel/Lengyel-UnifiedFog.pdf
	float ComputeHalfSpace (float3 wsDir)
	{
		float3 wpos = _CameraWS + wsDir;
		float FH = _HeightParams.x;
		float3 C = _CameraWS;
		float3 V = wsDir;
		float3 P = wpos;
		float3 aV = (_HeightParams.w * _heightFogIntensity) * V;
		float noise = tex3D(_NoiseTexture, frac(wpos * _NoiseData.x + float3(_Time.y * _NoiseVelocity.x, 0, _Time.y * _NoiseVelocity.y)));
		noise = saturate(noise - _NoiseData.z) * _NoiseData.y;
	    aV *= noise;
		float FdotC = _HeightParams.y;
		float k = _HeightParams.z;
		float FdotP = P.y-FH;
		float FdotV = wsDir.y;
		float c1 = k * (FdotP + FdotC);
		float c2 = (1-2*k) * FdotP;
		float g = min(c2, 0.0);
		g = -length(aV) * (c1 - g * g / abs(FdotV+1.0e-5f));
		return g;
	}

	half4 ComputeFog (v2f i, bool distance, bool height) : SV_Target
	{
		half4 sceneColor = tex2D(_MainTex, UnityStereoTransformScreenSpaceTex(i.uv));
	
		float rawDepth;
		//rawDepth = SAMPLE_DEPTH_TEXTURE(_CustomDepth,UnityStereoTransformScreenSpaceTex(i.uv_depth));
		rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(i.uv_depth));



		float dpth = Linear01Depth(rawDepth);
		float4 wsDir = dpth * i.interpolatedRay;
		float4 wsPos = _CameraWS + wsDir;
		float4 clouds = tex2D(_Clouds,UnityStereoTransformScreenSpaceTex(i.uv));
		float4 bg = tex2D(_Background,UnityStereoTransformScreenSpaceTex(i.uv));

	/////-------------------------------Scattering----------------------------///////////////
			   float3 viewDir = normalize(dpth * i.interpolatedRay);
			   float cosTheta = dot( viewDir, _SunDir );
			   viewDir = normalize(dpth * i.interpolatedRay) + float3(0.0, 0.1 ,0.0);

			   float zen = acos(saturate(viewDir.y));
			   float alb = (cos(zen) + 0.5 * pow(93.885 - ((zen * 180.0) / 3.141592), - 0.253)); // pi
			   float3 fex  = exp(-(_Br * (4 / alb)  + _Bm * (1.25 / alb)));

			   float rayPhase = 2.5 + pow(cosTheta,2);                 
			   float3 mg = _mieG;
			 //   mg *= 1 * (clamp(lerp(0,0.01,(1 - dpth)),0,1) * 10);
	
			   float  miePhase = _mieG.x / pow(_mieG.y - _mieG.z * cosTheta, 1); 
			    
			   float3 BrTheta  = 0.059683 * _Br * rayPhase; //(3.0/(16.0*pi)) 
			   float3 BmTheta  = 0.079577  * _Bm * miePhase; //(1.0/(4.0*pi))
			   float3 BrmTheta = (BrTheta + BmTheta * 2.0) / ((_Bm + _Br) * 0.75);   

			   float3 scattering = BrmTheta * _SunIntensity * (1.0 - fex);
			   scattering *= saturate((lerp(_scatteringPower , pow(2000.0 * BrmTheta * fex,0.7),i.sky.y) * 0.05));
			   scattering *= _SkyLuminance * _scatteringColor.rgb;
			   scattering *= pow((1.0 - fex),1.0);
			   scattering *= i.sky.x;

			   float3 sunClr = lerp(fex,_sunDiskColor.rgb,0.5) * _SunDiskIntensity;
			   float3 sunDisk = min(2, pow((1 - cosTheta) * (_SunDiskSize * 100) , -2 )) * sunClr;
			   sunDisk *= saturate(_sunDiskColor);

			   float3 skyFinalize = saturate((pow( 1.0 - fex, 2.0) * 0.234) * (1 - i.sky.x)); 

			   skyFinalize *= _SkyLuminance;
			   skyFinalize = saturate(lerp(float4(0.1,0.1,0.1,0), skyFinalize, saturate(dot(viewDir.y + 0.3, float3(0,1,0)))) * (1-fex));

			   float4 fogScattering = float4((scattering + skyFinalize), 1);

				// ToneMapping
				if(_hdr == 0)
			  		 fogScattering = saturate( 1.0 - exp(-_Exposure * fogScattering));

			   fogScattering = pow(fogScattering,_SkyColorPower);
			   fogScattering = lerp(fogScattering,lerp(fogScattering,_weatherSkyMod,_weatherSkyMod.a), _weatherSkyMod.a);

	/////-------------------------------FOG----------------------------///////////////
		float g = _DistanceParams.x;
		if (distance)
		{
			g += ComputeDistance (wsDir, dpth);
			g *= _distanceFogIntensity;
		}
		half gAdd = 0;
		half fogFacHeight = 0;
		if (height)
		{
			gAdd = ComputeHalfSpace (wsDir);
			g += gAdd;
			//fogFacHeight = ComputeFogFactor (max(0.0,gAdd));
		} 

		// Compute fog amount
		half fogFac = ComputeFogFactor (max(0.0,g));

		fogFac = lerp(_maximumFogDensity,1,fogFac);
		float4 finalFog = lerp(fogScattering,lerp(fogScattering,_weatherFogMod,0.5),_weatherFogMod.a);

		// SKY
		if (dpth >= 0.99999) 
		{
			float f =  saturate((_SkyFogHeight * (2-_skyFogIntensity) * dot(normalize(wsPos - _CameraWS), float3(0,1,0))));
			fogFac = f;
			clouds = lerp(bg,clouds,clouds.a);
	    	float sunMask = 1;
             if(clouds.a <= 0.05) sunMask = lerp(0.1,1,clouds.a);
            else sunMask = 0;

            float4 i = min(2, pow((1 - cosTheta) * (_SunDiskSize * 100) , -2 ));
			finalFog = lerp (finalFog,lerp(finalFog,finalFog + ((clouds * clouds)* sunMask),i),_SunBlocking);

		}

		// Color bandińg fix
		float2 wcoord = (wsPos.xy/wsPos.w) * _noiseScale;
		float4 dither = ( dot( float2( 171.0f, 231.0f ), wcoord.xy ) );
		dither.rgb = frac( dither / float3( 103.0f, 71.0f, 97.0f ) ) - float3( 0.5f, 0.5f, 0.5f );

		float4 final = lerp (finalFog, sceneColor, fogFac);

		return final + (dither/255.0f) * _noiseIntensity;
	}

ENDCG

SubShader
{
	ZTest Always Cull Off ZWrite Off Fog { Mode Off }

	// 0: distance + height
	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0
		half4 frag (v2f i) : SV_Target { return ComputeFog (i, true, true); }
		ENDCG
	}
	// 1: distance
	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0
		half4 frag (v2f i) : SV_Target { return ComputeFog (i, true, false); }
		ENDCG
	}
	// 2: height
	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0
		half4 frag (v2f i) : SV_Target { return ComputeFog (i, false, true); }
		ENDCG
	}
}

Fallback off

}
