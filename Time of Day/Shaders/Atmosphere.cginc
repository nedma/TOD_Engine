   #pragma once
			// region, uniform variables for computing one-pass blue cloud back side color
            uniform float3 TOD_LocalSunDirection;
            uniform float3 TOD_SunColor;
            uniform float4 TOD_AdditiveColor;
            uniform float3 TOD_CloudColor;
			uniform float3 TOD_LightColor;
            uniform float _Contrast;
            uniform float _Haziness;
            uniform float _Horizon;
            uniform float _Fogginess;
            uniform float2 _OpticalDepth;
            uniform float3 _OneOverBeta;
            uniform float3 _BetaRayleigh;
            uniform float3 _BetaRayleighTheta;
            uniform float3 _BetaMie;
            uniform float3 _BetaMieTheta;
            uniform float2 _BetaMiePhase;

			// region uv animation 
            uniform float _CloudScale; // uv anim
			uniform float _CloudSpeed; // uv anim
            uniform float4 _CloudUV; // uv anim
			uniform float _UvPow; // to decide the cloud is more oppersive or more high.

			// region texture
            uniform sampler2D _SCloud; // texture

			// region ray-marching
			uniform float4x4 _IV_MVP; // inverse of MVP to calculate a screen point's world position
			uniform float4 _CloudVLitOffset; // help to calculate the blur direction towards sun
			uniform float _CloudCover; // use to decide the cloud's density
			uniform float _CloudIntensity; // to decide the cloud's intensity
			uniform fixed _SunnyDegree;


			// functions for cloud raymarching
			float2 GetScreenUv(float3 wpos)
			{
				float4 posh = mul(UNITY_MATRIX_VP, float4(wpos,1));
				posh.xy /= posh.w;
				posh.xy = posh.xy * 0.5 + 0.5; // yeah, since we are in glsl mode
				return posh.xy;
			}

			float2 GetCloudUvMMM(float3 vertexM, float speed, float scale)
			{
				float3 vertnorm = normalize(vertexM);
                float2 vertuv   = vertnorm.xz / pow(vertnorm.y + 0.1, _UvPow);

                return (vertuv + _CloudUV.xy * speed) / scale;
			}

			float2 GetCloudUv(float2 screenUV, float speed, float scale)
			{
				float4 posH = float4(screenUV * 2.0 - 1.0, 1.0, 1.0);
				float4 posM = mul(_IV_MVP, posH);

				return GetCloudUvMMM(posM, speed, scale);
//				float3 vertnorm = normalize(posM.xyz);
//                float2 vertuv   = vertnorm.xz / pow(vertnorm.y + 0.1, _UvPow);

//                return (vertuv + _CloudUV.xy * speed) / scale;
			}

			fixed4 GetCloudDensity(fixed4 clTexColor)
			{
				// blend 4 channel of the cloud texture according to cloud cover 
				fixed cv = _CloudCover;
				fixed4 vDensity = abs(cv - fixed4(0.25, 0.5, 0.75, 1.0)) / 0.25f;
				vDensity = saturate(1.0 - vDensity);

				fixed finalDensity = dot(clTexColor, vDensity);
				return finalDensity;
			}


			// functions for one-pass atmosphere color, 
            inline float3 L(float3 viewdir, float3 sundir) 
			{
                float3 res;
                float cosTheta = max(0, dot(viewdir, sundir));
                float angular = (1 + cosTheta*cosTheta);
                float3 betaTheta = _BetaRayleighTheta + _BetaMieTheta / pow(_BetaMiePhase.x - _BetaMiePhase.y * cosTheta, 1.5);
                res = angular * betaTheta * _OneOverBeta;
                return res;
            }

            inline float3 T(float height) 
			{
                float3 res;
                float h = clamp(height + _Horizon, 0.001, 1);
                float f = pow(h, _Haziness);
                float sh = (1 - f) * 190000;
                float sr = sh + f * (_OpticalDepth.x - sh);
                float sm = sh + f * (_OpticalDepth.y - sh);
                float3 beta = _BetaRayleigh * sr + _BetaMie * sm;
                res = exp(-beta);
                return res;
            }

