// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Time of Day/Atmosphere"
{
	Properties
	{
		[KeywordEnum(None, SHOW_SCATTERING, SHOW_CLOUD, SHOW_FOG, SHOW_ALPHA, SHOW_EXTINCTION, SHOW_MIE)] DB("Debug Mode", Float) = 0
	}

    SubShader
    {
        Tags
        {
            "Queue"="Transparent-470"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
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
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

			#pragma shader_feature _ DB_SHOW_SCATTERING DB_SHOW_CLOUD DB_SHOW_FOG DB_SHOW_EXTINCTION DB_SHOW_MIE


            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float3 TOD_LocalSunDirection;
            uniform float3 TOD_LocalMoonDirection;
            uniform float3 TOD_SunColor;
            uniform float3 TOD_MoonColor;
            uniform float4 TOD_AdditiveColor;
            uniform float3 TOD_CloudColor;
            uniform float3 TOD_MoonHaloColor;

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
            uniform float3 _BetaNight;

			// [nedma]Mie scattering irradiance
            inline float3 L(float3 viewdir, float3 sundir) {
                float3 res;

                // Angle between sun and viewdir
                float cosTheta = max(0, dot(viewdir, sundir));

                // Angular dependency
                // See [3] page 2 equation (2) and (4)
                float angular = (1 + cosTheta*cosTheta);

                // Rayleigh and mie scattering factors
                // See [3] page 2 equation (3) and (4)
                float3 betaTheta = _BetaRayleighTheta
                                 + _BetaMieTheta / pow(_BetaMiePhase.x - _BetaMiePhase.y * cosTheta, 1.5);

                // Scattering solution
                // See [5] page 11 - Inscattering Irradiance
                res = angular * betaTheta * _OneOverBeta;
				
				//res = 1 / pow(_BetaMiePhase.x - _BetaMiePhase.y * cosTheta, 1.5);  // [nedma]This item determines the mie inscattering shape, dependent on "g" in the phase function

                return res;
            }

            inline float3 L() {
                return _BetaNight;
            }

			// [nedma]Àà±ÈextinctionÏî
			// Calculate attenuated spectral distribution(by optical depth)
			// See [7] page 28 equation 2.7,2.8
            inline float3 T(float height) {
                float3 res;

                // Parameter value
                // See [7] page 70 equation (5.7)
                float h = clamp(height + _Horizon, 0.001, 1);
                float f = pow(h, _Haziness);

                // Optical depth integral approximation
                // See [7] page 71 equation (5.8)
                // See [7] page 71 equation (5.10)
                // See [7] page 76 equation (6.1)
                float sh = (1 - f) * 190000;
                float sr = sh + f * (_OpticalDepth.x - sh);
                float sm = sh + f * (_OpticalDepth.y - sh);

                // Rayleigh and mie scattering factors
                // See [3] page 2 equation (1) and (2)
                float3 beta = _BetaRayleigh * sr
                            + _BetaMie * sm;

				//beta = _BetaRayleigh * sr * 10;
				//beta = _BetaMie * sm * 100;

                // Scattering solution
                // See [5] page 11
                res = exp(-beta);

                return res;
            }

            struct v2f {
                float4 position : POSITION;
                fixed4 color    : COLOR;
            };

            v2f vert(appdata_base v) {
                v2f o;

                // Scattering values
                float3 T_val  = T(v.normal.y);
                float3 E_sun  = TOD_SunColor;
                float3 E_moon = TOD_MoonColor;
                float3 L_sun  = L(-v.normal, TOD_LocalSunDirection);
                float3 L_moon = L();

				o.color.rgb = 0;
				o.color.a = 1;

                // Add inscattering color(un-extincted light through the atmosphere from the sun/moon )
				float4 scatteringColor;
				scatteringColor.rgb = (1-T_val) * (E_sun*L_sun + E_moon*L_moon);
				scatteringColor.a   = 10 * max(max(scatteringColor.r, scatteringColor.g), scatteringColor.b);

                // Add simple moon halo
				float3 moonHalo = TOD_MoonHaloColor * pow(max(0, dot(TOD_LocalMoonDirection, -v.normal)), 10);

                // Add additive color

                // Add fog color
				o.color = scatteringColor + TOD_AdditiveColor;
				o.color.rgb += moonHalo;
				o.color.rgb = lerp(o.color.rgb, TOD_CloudColor, _Fogginess);
				o.color.a += _Fogginess;

#if   defined(DB_SHOW_SCATTERING)
				o.color = scatteringColor;
				o.color.a = 1;
#elif defined(DB_SHOW_EXTINCTION)
				o.color.rgb = 1 - T_val; // sun color(T_val) --> blue
				o.color.a = 1;
#elif defined(DB_SHOW_MIE)
				o.color.rgb = L_sun * 0.05;  // mie scattering before applying extinction
				o.color.a = 1;
#elif defined(DB_SHOW_CLOUD)
				o.color.rgb = TOD_CloudColor;
				o.color.a = 1;
#elif defined(DB_SHOW_FOG)
				o.color.rgb = _Fogginess;
				o.color.a = 1;
#endif

                // Clamp alpha to [0, 1]
                o.color.a = saturate(o.color.a);

                // Adjust output color according to gamma value
                o.color.rgb = pow(o.color.rgb, _Contrast);

                // Write position
                o.position = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 frag(v2f i) : COLOR {
                return i.color;
            }

            ENDCG
        }
    }

    Fallback off
}
