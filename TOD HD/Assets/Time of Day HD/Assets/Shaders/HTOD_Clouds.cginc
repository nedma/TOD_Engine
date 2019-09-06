#ifndef HTOD_CLOUDS_INCLUDED
#define HTOD_CLOUDS_INCLUDED

inline float3 CloudPosition(float3 viewDir, float3 offset)
{
	float mult = 1.0 / lerp(0.1, 1.0, viewDir.y);
	return (float3(viewDir.x * mult + offset.x, 0, viewDir.z * mult + offset.z)) / HTOD_CloudSize;
}

inline float4 CloudUV(float3 viewDir, float3 offset)
{
	float3 cloudPos = CloudPosition(viewDir, offset);
	float2 uv1 = cloudPos.xz + HTOD_CloudOffset.xz + HTOD_CloudWind.xz;
	float2 uv2 = mul(HTOD_ROTATION_UV(radians(10.0)), cloudPos.xz) + HTOD_CloudOffset.xz + HTOD_CloudWind.xz;
	return float4(uv1.x, uv1.y, uv2.x, uv2.y);
}

inline float4 CloudUV(float3 viewDir)
{
	return CloudUV(viewDir, 0);
}

inline float3 CloudColor(float3 viewDir, float3 lightDir)
{
	float lerpValue = saturate(1 + 4 * lightDir.y) * saturate(dot(viewDir, lightDir) + 1.25);
	float3 cloudColor = lerp(HTOD_MoonCloudColor, HTOD_SunCloudColor, lerpValue);
	float3 fogColor = HTOD_FogColor;
	return HTOD_Brightness * lerp(cloudColor, fogColor, HTOD_Fogginess);
}

inline half3 CloudLayerDensity(sampler2D densityTex, float4 uv, float3 viewDir)
{
	half3 density = 0;

#if HTOD_CLOUDS_DENSITY
	const float thickness = 0.1;
	const half4 stepoffset = half4(0.0, 1.0, 2.0, 3.0) * thickness;
	const half4 sumy = half4(0.5000, 0.2500, 0.1250, 0.0625) / half4(1, 2, 3, 4);
	const half4 sumz = half4(0.5000, 0.2500, 0.1250, 0.0625);

	half2 uv1 = uv.xy + viewDir.xz * stepoffset.x;
	half2 uv2 = uv.zw + viewDir.xz * stepoffset.y;
	half2 uv3 = uv.xy + viewDir.xz * stepoffset.z;
	half2 uv4 = uv.zw + viewDir.xz * stepoffset.w;

	half4 n1 = tex2D(densityTex, uv1);
	half4 n2 = tex2D(densityTex, uv2);
	half4 n3 = tex2D(densityTex, uv3);
	half4 n4 = tex2D(densityTex, uv4);

	// Noise when marching in up direction
	half4 ny = half4(n1.r, n1.g + n2.g, n1.b + n2.b + n3.b, n1.a + n2.a + n3.a + n4.a);

	// Noise when marching in view direction
	half4 nz = half4(n1.r, n2.g, n3.b, n4.a);

	// Density when marching in up direction
	density.y += dot(ny, sumy);

	// Density when marching in view direction
	density.z += dot(nz, sumz);

	// Coverage
	half2 stepA = HTOD_CloudCoverage;
	half2 stepB = HTOD_CloudCoverage + HTOD_CloudSharpness;
	half2 stepC = HTOD_CloudDensity;
	density.yz = smoothstep(stepA, stepB, density.yz) + saturate(density.yz - stepB) * stepC;

	// Opacity
	density.x = saturate(density.z);

	// Shading
	density.yz *= half2(HTOD_CloudAttenuation, HTOD_CloudSaturation);

	// Remap
	density.yz = 1.0 - exp2(-density.yz);
#else
	half4 n = tex2D(densityTex, uv.xy);

	// Density when marching in up direction
	density.y += n.r;

	// Density when marching in view direction
	density.z += n.r;

	// Coverage
	density.yz = (density.yz - HTOD_CloudCoverage) * half2(HTOD_CloudAttenuation, HTOD_CloudDensity);

	// Opacity
	density.x = saturate(density.z);
#endif

	return density;
}

inline half4 CloudLayerColor(sampler2D densityTex, float4 uv, float4 color, float3 viewDir, float3 lightDir, float3 lightCol)
{
	half3 density = CloudLayerDensity(densityTex, uv, viewDir);

	half4 res = 0;
	res.a = density.x;
	res.rgb = 1.0 - density.y;

	res *= color;

#if HTOD_CLOUDS_BUMPED
	res.rgb += saturate((1.0 - density.z) * (1.0 - HTOD_Fogginess)) * lightCol;
#endif

	return res;
}

inline half CloudShadow(sampler2D densityTex, float4 uv)
{
	return CloudLayerDensity(densityTex, uv, float3(0, 1, 0)).x;
}

inline half3 CloudBillboardDensity(sampler2D densityTex, float2 uv, float3 viewDir)
{
	half3 density = 0;

	half4 tex = tex2D(densityTex, uv.xy);

	// Density when marching in up direction
	density.y = tex.a;

	// Density when marching in view direction
#if HTOD_CLOUDS_DENSITY
	density.z = lerp(lerp(tex.r, tex.g, saturate(viewDir.x)), tex.b, saturate(-viewDir.x));
#else
	density.z = tex.r;
#endif

	// Opacity
	density.x = saturate(density.z);

	// Shading
	density.y *= HTOD_CloudAttenuation;

	return density;
}

inline half4 CloudBillboardColor(sampler2D densityTex, sampler2D normalTex, float4 uv, float4 color, float3 viewDir, float3 lightDir, float3 lightCol)
{
	half3 density = CloudBillboardDensity(densityTex, uv.xy, viewDir);

	half4 res = 0;
	res.a = density.x;
	res.rgb = 1.0 - density.y;

#if HTOD_CLOUDS_DENSITY
	half3 normal = UnpackNormal(tex2D(normalTex, uv.zw));
	half NdotS = dot(normal, lightDir);
	res.rgb += 0.25 * (1.0 - HTOD_Fogginess) * NdotS;
#endif

	res *= color;

#if HTOD_CLOUDS_BUMPED
	res.rgb += saturate((1.0 - density.z) * (1.0 - HTOD_Fogginess)) * lightCol;
#endif

	return res;
}

inline float CloudShadow(float3 cameraToWorldPos)
{
	float3 worldPos = _WorldSpaceCameraPos + cameraToWorldPos;
	float3 skyPos = mul((float3x3)HTOD_World2Sky, worldPos);

	float4 cloudUV = CloudUV(HTOD_LocalLightDirection, skyPos);

	return HTOD_CloudOpacity * CloudShadow(HTOD_CloudTexture, cloudUV);
}

#endif
