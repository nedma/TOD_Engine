#ifndef HTOD_SCATTERING_INCLUDED
#define HTOD_SCATTERING_INCLUDED

#ifndef HTOD_SCATTERING_SAMPLES
#define HTOD_SCATTERING_SAMPLES 2
#endif

#ifndef HTOD_SCATTERING_MIE
#define HTOD_SCATTERING_MIE 1
#endif

#ifndef HTOD_SCATTERING_RAYLEIGH
#define HTOD_SCATTERING_RAYLEIGH 1
#endif

#ifndef HTOD_BAYER_DIM
#define HTOD_BAYER_DIM 8.0
#endif

inline float Scale(float inCos)
{
	float x = 1.0 - inCos;
	return 0.25 * exp(-0.00287 + x*(0.459 + x*(3.83 + x*(-6.80 + x*5.25))));
}

inline float MiePhase(float eyeCos, float eyeCos2)
{
	return HTOD_kBetaMie.x * (1.0 + eyeCos2) / pow(HTOD_kBetaMie.y + HTOD_kBetaMie.z * eyeCos, 1.5);
}

inline float RayleighPhase(float eyeCos2)
{
	return 0.75 + 0.75 * eyeCos2;
}

inline float CloudPhase(float eyeCos, float eyeCos2)
{
	const float g = 0.3;
	const float g2 = g * g;
	return HTOD_CloudScattering * (1.5 * (1.0 - g2) / (2.0 + g2) * (1.0 + eyeCos2) / (1.0 + g2 - 2.0 * g * eyeCos) + g * eyeCos);
}

inline float3 NightPhase(float3 dir)
{
	dir.y = max(0, dir.y);

	return HTOD_MoonSkyColor * (1.0 - 0.75 * dir.y);
}

inline float3 MoonPhase(float3 dir)
{
	return HTOD_MoonHaloColor * pow(max(0, dot(dir, HTOD_LocalMoonDirection)), HTOD_MoonHaloPower);
}

inline float3 PostProcess(float3 col, float3 dir)
{
	// Lerp to ground color
	col = lerp(col, HTOD_GroundColor, saturate(-dir.y));

	// Lerp to fog color
	col = lerp(col, HTOD_FogColor, HTOD_Fogginess);

	// Adjust output color
	col = pow(col * HTOD_Brightness, HTOD_Contrast);

	return col;
}

#if HTOD_SCATTERING_RAYLEIGH && HTOD_SCATTERING_MIE
inline void ScatteringCoefficients(float3 dir, out float3 inscatter, out float3 outscatter)
#elif HTOD_SCATTERING_RAYLEIGH
inline void ScatteringCoefficients(float3 dir, out float3 inscatter)
#else
inline void ScatteringCoefficients(float3 dir, out float3 outscatter)
#endif
{
	dir = normalize(float3(dir.x, max(0, dir.y), dir.z));

	float kInnerRadius  = HTOD_kRadius.x;
	float kInnerRadius2 = HTOD_kRadius.y;
	float kOuterRadius2 = HTOD_kRadius.w;

	float kScale               = HTOD_kScale.x;
	float kScaleOverScaleDepth = HTOD_kScale.z;
	float kCameraHeight        = HTOD_kScale.w;

	float3 kKr4PI = HTOD_k4PI.xyz;
	float  kKm4PI = HTOD_k4PI.w;

	float3 kKrESun = HTOD_kSun.xyz;
	float  kKmESun = HTOD_kSun.w;

	// Current camera position
	float3 cameraPos = float3(0, kInnerRadius + kCameraHeight, 0);

	// Length of the atmosphere
	float far = sqrt(kOuterRadius2 + kInnerRadius2 * dir.y * dir.y - kInnerRadius2) - kInnerRadius * dir.y;

	// Ray starting position and its scattering offset
	float startDepth  = exp(kScaleOverScaleDepth * (-kCameraHeight));
	float startAngle  = dot(dir, cameraPos) / (kInnerRadius + kCameraHeight);
	float startOffset = startDepth * Scale(startAngle);

	// Scattering loop variables
	float  sampleLength = far / float(HTOD_SCATTERING_SAMPLES);
	float  scaledLength = sampleLength * kScale;
	float3 sampleRay    = dir * sampleLength;
	float3 samplePoint  = cameraPos + sampleRay * 0.5;

	float3 sunColor = float3(0.0, 0.0, 0.0);

	// Loop through the sample rays
	for (int i = 0; i < int(HTOD_SCATTERING_SAMPLES); i++)
	{
		float height    = max(1, length(samplePoint));
		float invHeight = 1.0 / height;

		float depth = exp(kScaleOverScaleDepth * (kInnerRadius - height));
		float atten = depth * scaledLength;

		float cameraAngle = dot(dir, samplePoint) * invHeight;
		float sunAngle    = dot(HTOD_LocalSunDirection,  samplePoint) * invHeight;
		float sunScatter  = startOffset + depth * (Scale(sunAngle)  - Scale(cameraAngle));

		float3 sunAtten = exp(-sunScatter * (kKr4PI + kKm4PI));

		sunColor    += sunAtten * atten;
		samplePoint += sampleRay;
	}

	// Sun scattering
	#if HTOD_SCATTERING_RAYLEIGH
	inscatter = HTOD_SunSkyColor * sunColor * kKrESun;
	#endif

	#if HTOD_SCATTERING_MIE
	outscatter = HTOD_SunSkyColor * sunColor * kKmESun;
	#endif
}

#if HTOD_SCATTERING_RAYLEIGH && HTOD_SCATTERING_MIE
inline float4 ScatteringColor(float3 dir, float3 inscatter, float3 outscatter)
#elif HTOD_SCATTERING_RAYLEIGH
inline float4 ScatteringColor(float3 dir, float3 inscatter)
#else
inline float4 ScatteringColor(float3 dir, float3 outscatter)
#endif
{
	float3 resultColor = float3(0, 0, 0);

	float sunCos  = dot(HTOD_LocalSunDirection, dir);
	float sunCos2 = sunCos * sunCos;

	#if HTOD_SCATTERING_RAYLEIGH
	resultColor += NightPhase(dir);
	#endif

	#if HTOD_SCATTERING_MIE
	resultColor += MoonPhase(dir);
	#endif

	#if HTOD_SCATTERING_RAYLEIGH
	resultColor += RayleighPhase(sunCos2) * inscatter;
	#endif

	#if HTOD_SCATTERING_MIE
	resultColor += MiePhase(sunCos, sunCos2) * outscatter;
	#endif

	return float4(PostProcess(resultColor, dir), 1.0);
}

inline float4 ScatteringColor(float3 dir)
{
	#if HTOD_SCATTERING_RAYLEIGH && HTOD_SCATTERING_MIE
	float3 inscatter, outscatter;
	ScatteringCoefficients(dir, inscatter, outscatter);
	return ScatteringColor(dir, inscatter, outscatter);
	#elif HTOD_SCATTERING_RAYLEIGH
	float3 inscatter;
	ScatteringCoefficients(dir, inscatter);
	return ScatteringColor(dir, inscatter);
	#else
	float3 outscatter;
	ScatteringCoefficients(dir, outscatter);
	return ScatteringColor(dir, outscatter);
	#endif
}

inline float FogDensityMaximum(float3 worldPos)
{
	float3 objectPos = worldPos;
	float3 cameraPos = _WorldSpaceCameraPos;

	float startDistance = HTOD_ScatterDensity.w;
	float cameraDistance = length(objectPos - cameraPos);

	float globalDensity = HTOD_ScatterDensity.z;
	float heightDensity = max(0.01, HTOD_ScatterDensity.x * (min(objectPos.y, cameraPos.y) - HTOD_ScatterDensity.y));
	float heightDensityExpInv = exp(-heightDensity);

	// Get base fog intensity
	float fogIntensity = max(0, cameraDistance - startDistance);

	// Apply height falloff
	fogIntensity *= heightDensityExpInv * (1.0 - heightDensityExpInv) / heightDensity;

	// Clamp intensity
	fogIntensity = min(10, globalDensity * fogIntensity);

	return 1.0 - exp(-fogIntensity);
}

inline float FogDensityAverage(float3 worldPos)
{
	float3 objectPos = worldPos;
	float3 cameraPos = _WorldSpaceCameraPos;

	float startDistance = HTOD_ScatterDensity.w;
	float cameraDistance = length(objectPos - cameraPos);

	float globalDensity = HTOD_ScatterDensity.z;
	float heightDensityObject = max(0.01, HTOD_ScatterDensity.x * (objectPos.y - HTOD_ScatterDensity.y));
	float heightDensityCamera = max(0.01, HTOD_ScatterDensity.x * (cameraPos.y - HTOD_ScatterDensity.y));
	float heightDensityObjectExpInv = exp(-heightDensityObject);
	float heightDensityCameraExpInv = exp(-heightDensityCamera);

	// Get base fog intensity
	float fogIntensity = max(0, cameraDistance - startDistance);

	// Apply height falloff
	float fogIntensityObject = heightDensityObjectExpInv * (1.0 - heightDensityObjectExpInv) / heightDensityObject;
	float fogIntensityCamera = heightDensityCameraExpInv * (1.0 - heightDensityCameraExpInv) / heightDensityCamera;

	fogIntensity *= (fogIntensityObject + fogIntensityCamera) * 0.5;

	// Clamp intensity
	fogIntensity = min(10, globalDensity * fogIntensity);

	return 1.0 - exp(-fogIntensity);
}

inline float FogDensity(float3 worldPos)
{
	return FogDensityAverage(worldPos);
}

inline float4 AtmosphericScattering(float3 cameraRay, float3 worldPos, float depth, float mask)
{
	float3 dir = normalize(mul((float3x3)HTOD_World2Sky, cameraRay));

	#if HTOD_SCATTERING_RAYLEIGH && HTOD_SCATTERING_MIE
	float3 inscatter, outscatter;
	ScatteringCoefficients(dir, inscatter, outscatter);
	#elif HTOD_SCATTERING_RAYLEIGH
	float3 inscatter;
	ScatteringCoefficients(dir, inscatter);
	#else
	float3 outscatter;
	ScatteringCoefficients(dir, outscatter);
	#endif

	if (depth != 1)
	{
		depth = FogDensity(worldPos);

		#if HTOD_SCATTERING_MIE
		outscatter = outscatter * depth * mask;
		#endif

		#if HTOD_SCATTERING_RAYLEIGH
		inscatter = inscatter * depth;
		#endif
	}

	#if HTOD_SCATTERING_RAYLEIGH && HTOD_SCATTERING_MIE
	float3 color = ScatteringColor(dir, inscatter, outscatter).rgb;
	#elif HTOD_SCATTERING_RAYLEIGH
	float3 color = ScatteringColor(dir, inscatter).rgb;
	#else
	float3 color = ScatteringColor(dir, outscatter).rgb;
	#endif

	return float4(color, depth);
}

inline float4 AtmosphericScattering(float3 cameraRay, float3 worldPos, float depth)
{
	const float mask = 1; // This should be sampled from the scattering occlusion mask

	return AtmosphericScattering(cameraRay, worldPos, depth, mask);
}

inline float4 AtmosphericScattering(float3 cameraRay, float3 worldPos)
{
	float3 dir = normalize(mul((float3x3)HTOD_World2Sky, cameraRay));

	float depth = FogDensity(worldPos);
	float3 color = ScatteringColor(dir).rgb;

	return float4(color, depth);
}

inline float2 DitheringCoords(float2 screenPos)
{
	return screenPos * _ScreenParams.xy * (1.0 / HTOD_BAYER_DIM);
}

inline float DitheringColor(float2 uv)
{
	return tex2D(HTOD_BayerTexture, uv).a * (1.0 / (HTOD_BAYER_DIM * HTOD_BAYER_DIM + 1.0));
}

#endif
