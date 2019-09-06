#ifndef HTOD_BASE_INCLUDED
#define HTOD_BASE_INCLUDED

#include "UnityCG.cginc"

uniform sampler2D HTOD_BayerTexture;
uniform sampler2D HTOD_CloudTexture;

uniform float4x4 HTOD_World2Sky;
uniform float4x4 HTOD_Sky2World;

uniform float3 HTOD_SunLightColor;
uniform float3 HTOD_MoonLightColor;

uniform float3 HTOD_SunSkyColor;
uniform float3 HTOD_MoonSkyColor;

uniform float3 HTOD_SunMeshColor;
uniform float3 HTOD_MoonMeshColor;

uniform float3 HTOD_SunCloudColor;
uniform float3 HTOD_MoonCloudColor;

uniform float3 HTOD_FogColor;
uniform float3 HTOD_GroundColor;
uniform float3 HTOD_AmbientColor;

uniform float3 HTOD_SunDirection;
uniform float3 HTOD_MoonDirection;
uniform float3 HTOD_LightDirection;

uniform float3 HTOD_LocalSunDirection;
uniform float3 HTOD_LocalMoonDirection;
uniform float3 HTOD_LocalLightDirection;

uniform float HTOD_Contrast;
uniform float HTOD_Brightness;
uniform float HTOD_Fogginess;

uniform float HTOD_MoonHaloPower;
uniform float3 HTOD_MoonHaloColor;

uniform float HTOD_CloudOpacity;
uniform float HTOD_CloudCoverage;
uniform float HTOD_CloudSharpness;
uniform float HTOD_CloudDensity;
uniform float HTOD_CloudColoring;
uniform float HTOD_CloudAttenuation;
uniform float HTOD_CloudSaturation;
uniform float HTOD_CloudScattering;
uniform float HTOD_CloudBrightness;
uniform float3 HTOD_CloudOffset;
uniform float3 HTOD_CloudWind;
uniform float3 HTOD_CloudSize;

uniform float HTOD_CloudShadowCutoff;
uniform float HTOD_CloudShadowFade;
uniform float HTOD_CloudShadowIntensity;

uniform float HTOD_StarSize;
uniform float HTOD_StarBrightness;
uniform float HTOD_StarVisibility;

uniform float HTOD_SunMeshContrast;
uniform float HTOD_SunMeshBrightness;

uniform float HTOD_MoonMeshContrast;
uniform float HTOD_MoonMeshBrightness;

uniform float4 HTOD_ScatterDensity;

uniform float3 HTOD_kBetaMie;
uniform float4 HTOD_kSun;
uniform float4 HTOD_k4PI;
uniform float4 HTOD_kRadius;
uniform float4 HTOD_kScale;

// Vertex transform used by the entire sky dome
#define HTOD_TRANSFORM_VERT(vert) UnityObjectToClipPos(vert)

// UV rotation matrix constructor
#define HTOD_ROTATION_UV(angle) float2x2(cos(angle), -sin(angle), sin(angle), cos(angle))

// Fast and simple tonemapping
#define HTOD_HDR2LDR(color) (1.0 - exp2(-HTOD_Brightness * color))

// Approximates gamma by 2.0 instead of 2.2
#define HTOD_GAMMA2LINEAR(color) (color * color)
#define HTOD_LINEAR2GAMMA(color) sqrt(color)

// Matrices
#define HTOD_Object2World unity_ObjectToWorld
#define HTOD_World2Object unity_WorldToObject

// Screen space adjust
#define HTOD_UV(x, y) UnityStereoScreenSpaceUVAdjust(x, y)

// Stereo output
#define HTOD_VERTEX_OUTPUT_STEREO UNITY_VERTEX_OUTPUT_STEREO
#define HTOD_INITIALIZE_VERTEX_OUTPUT_STEREO(o) UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o)

// Instancing
#define HTOD_INSTANCE_ID UNITY_VERTEX_INPUT_INSTANCE_ID
#define HTOD_SETUP_INSTANCE_ID(o) UNITY_SETUP_INSTANCE_ID(o)

#endif
