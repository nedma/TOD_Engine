using UnityEngine;

public partial class HTOD_Sky : MonoBehaviour
{
	private float timeSinceLightUpdate      = float.MaxValue;
	private float timeSinceAmbientUpdate    = float.MaxValue;
	private float timeSinceReflectionUpdate = float.MaxValue;

	private void UpdateQualitySettings()
	{
		if (Headless) return;

		Mesh spaceMesh      = null;
		Mesh atmosphereMesh = null;
		Mesh clearMesh      = null;
		Mesh cloudMesh      = null;
		Mesh moonMesh       = null;
		Mesh starMesh       = null;

		switch (MeshQuality)
		{
			case HTOD_MeshQualityType.Low:
				spaceMesh      = Resources.SkyLOD2;
				atmosphereMesh = Resources.SkyLOD2;
				clearMesh      = Resources.SkyLOD2;
				cloudMesh      = Resources.CloudsLOD2;
				moonMesh       = Resources.MoonLOD2;
				break;
			case HTOD_MeshQualityType.Medium:
				spaceMesh      = Resources.SkyLOD1;
				atmosphereMesh = Resources.SkyLOD1;
				clearMesh      = Resources.SkyLOD2;
				cloudMesh      = Resources.CloudsLOD1;
				moonMesh       = Resources.MoonLOD1;
				break;
			case HTOD_MeshQualityType.High:
				spaceMesh      = Resources.SkyLOD0;
				atmosphereMesh = Resources.SkyLOD0;
				clearMesh      = Resources.SkyLOD2;
				cloudMesh      = Resources.CloudsLOD0;
				moonMesh       = Resources.MoonLOD0;
				break;
		}

		switch (StarQuality)
		{
			case HTOD_StarQualityType.Low:
				starMesh = Resources.StarsLOD2;
				break;
			case HTOD_StarQualityType.Medium:
				starMesh = Resources.StarsLOD1;
				break;
			case HTOD_StarQualityType.High:
				starMesh = Resources.StarsLOD0;
				break;
		}

		if (Components.SpaceMeshFilter && Components.SpaceMeshFilter.sharedMesh != spaceMesh)
		{
			Components.SpaceMeshFilter.mesh = spaceMesh;
		}

		if (Components.MoonMeshFilter && Components.MoonMeshFilter.sharedMesh != moonMesh)
		{
			Components.MoonMeshFilter.mesh = moonMesh;
		}

		if (Components.AtmosphereMeshFilter && Components.AtmosphereMeshFilter.sharedMesh != atmosphereMesh)
		{
			Components.AtmosphereMeshFilter.mesh = atmosphereMesh;
		}

		if (Components.ClearMeshFilter && Components.ClearMeshFilter.sharedMesh != clearMesh)
		{
			Components.ClearMeshFilter.mesh = clearMesh;
		}

		if (Components.CloudMeshFilter && Components.CloudMeshFilter.sharedMesh != cloudMesh)
		{
			Components.CloudMeshFilter.mesh = cloudMesh;
		}

		if (Components.StarMeshFilter && Components.StarMeshFilter.sharedMesh != starMesh)
		{
			Components.StarMeshFilter.mesh = starMesh;
		}
	}

	private void UpdateRenderSettings()
	{
		if (Headless) return;

		// Fog color
		{
			UpdateFog();
		}

		// Ambient light
		if (!Application.isPlaying || timeSinceAmbientUpdate >= Ambient.UpdateInterval)
		{
			timeSinceAmbientUpdate = 0;
			UpdateAmbient();
		}
		else
		{
			timeSinceAmbientUpdate += Time.deltaTime;
		}

		// Reflection cubemap
		if (!Application.isPlaying || timeSinceReflectionUpdate >= Reflection.UpdateInterval)
		{
			timeSinceReflectionUpdate = 0;
			UpdateReflection();
		}
		else
		{
			timeSinceReflectionUpdate += Time.deltaTime;
		}
	}

	private void UpdateShaderKeywords()
	{
		if (Headless) return;

		switch (ColorSpace)
		{
			case HTOD_ColorSpaceType.Auto:
				if (QualitySettings.activeColorSpace == UnityEngine.ColorSpace.Linear)
				{
					Shader.EnableKeyword("HTOD_OUTPUT_LINEAR");
				}
				else
				{
					Shader.DisableKeyword("HTOD_OUTPUT_LINEAR");
				}
				break;

			case HTOD_ColorSpaceType.Linear:
				Shader.EnableKeyword("HTOD_OUTPUT_LINEAR");
				break;

			case HTOD_ColorSpaceType.Gamma:
				Shader.DisableKeyword("HTOD_OUTPUT_LINEAR");
				break;
		}

		switch (ColorRange)
		{
			case HTOD_ColorRangeType.Auto:
				if (Components.Camera && Components.Camera.HDR)
				{
					Shader.EnableKeyword("HTOD_OUTPUT_HDR");
				}
				else
				{
					Shader.DisableKeyword("HTOD_OUTPUT_HDR");
				}
				break;

			case HTOD_ColorRangeType.HDR:
				Shader.EnableKeyword("HTOD_OUTPUT_HDR");
				break;

			case HTOD_ColorRangeType.LDR:
				Shader.DisableKeyword("HTOD_OUTPUT_HDR");
				break;
		}

		switch (ColorOutput)
		{
			case HTOD_ColorOutputType.Raw:
				Shader.DisableKeyword("HTOD_OUTPUT_DITHERING");
				break;

			case HTOD_ColorOutputType.Dithered:
				Shader.EnableKeyword("HTOD_OUTPUT_DITHERING");
				break;
		}

		switch (SkyQuality)
		{
			case HTOD_SkyQualityType.PerVertex:
				Shader.DisableKeyword("HTOD_SCATTERING_PER_PIXEL");
				break;

			case HTOD_SkyQualityType.PerPixel:
				Shader.EnableKeyword("HTOD_SCATTERING_PER_PIXEL");
				break;
		}

		switch (CloudQuality)
		{
			case HTOD_CloudQualityType.Low:
				Shader.DisableKeyword("HTOD_CLOUDS_DENSITY");
				Shader.DisableKeyword("HTOD_CLOUDS_BUMPED");
				break;

			case HTOD_CloudQualityType.Medium:
				Shader.EnableKeyword("HTOD_CLOUDS_DENSITY");
				Shader.DisableKeyword("HTOD_CLOUDS_BUMPED");
				break;

			case HTOD_CloudQualityType.High:
				Shader.EnableKeyword("HTOD_CLOUDS_DENSITY");
				Shader.EnableKeyword("HTOD_CLOUDS_BUMPED");
				break;
		}
	}

	private void UpdateShaderProperties()
	{
		if (Headless) return;

		Shader.SetGlobalColor(Resources.ID_SunLightColor,  SunLightColor);
		Shader.SetGlobalColor(Resources.ID_MoonLightColor, MoonLightColor);

		Shader.SetGlobalColor(Resources.ID_SunSkyColor,  SunSkyColor);
		Shader.SetGlobalColor(Resources.ID_MoonSkyColor, MoonSkyColor);

		Shader.SetGlobalColor(Resources.ID_SunMeshColor,  SunMeshColor);
		Shader.SetGlobalColor(Resources.ID_MoonMeshColor, MoonMeshColor);

		Shader.SetGlobalColor(Resources.ID_SunCloudColor,  SunCloudColor);
		Shader.SetGlobalColor(Resources.ID_MoonCloudColor, MoonCloudColor);

		Shader.SetGlobalColor(Resources.ID_FogColor,     FogColor);
		Shader.SetGlobalColor(Resources.ID_GroundColor,  GroundColor);
		Shader.SetGlobalColor(Resources.ID_AmbientColor, AmbientColor);

		Shader.SetGlobalVector(Resources.ID_SunDirection,   SunDirection);
		Shader.SetGlobalVector(Resources.ID_MoonDirection,  MoonDirection);
		Shader.SetGlobalVector(Resources.ID_LightDirection, LightDirection);

		Shader.SetGlobalVector(Resources.ID_LocalSunDirection,   LocalSunDirection);
		Shader.SetGlobalVector(Resources.ID_LocalMoonDirection,  LocalMoonDirection);
		Shader.SetGlobalVector(Resources.ID_LocalLightDirection, LocalLightDirection);

		Shader.SetGlobalFloat(Resources.ID_Contrast,       Atmosphere.Contrast);
		Shader.SetGlobalFloat(Resources.ID_Brightness,     Atmosphere.Brightness);
		Shader.SetGlobalFloat(Resources.ID_Fogginess,      Atmosphere.Fogginess);
		Shader.SetGlobalFloat(Resources.ID_Directionality, Atmosphere.Directionality);

		Shader.SetGlobalFloat(Resources.ID_MoonHaloPower, 1f / Moon.HaloSize);
		Shader.SetGlobalColor(Resources.ID_MoonHaloColor, MoonHaloColor);

		float cloudCoverage    = Mathf.Lerp(0.8f, 0.0f, Clouds.Coverage);
		float cloudDensity     = Mathf.Lerp(3.0f, 9.0f, Clouds.Sharpness);
		float cloudAttenuation = Mathf.Lerp(0.0f, 1.0f, Clouds.Attenuation);
		float cloudSaturation  = Mathf.Lerp(0.0f, 2.0f, Clouds.Saturation);

		Shader.SetGlobalFloat(Resources.ID_CloudOpacity,     Clouds.Opacity);
		Shader.SetGlobalFloat(Resources.ID_CloudCoverage,    cloudCoverage);
		Shader.SetGlobalFloat(Resources.ID_CloudSharpness,   1f / cloudDensity);
		Shader.SetGlobalFloat(Resources.ID_CloudDensity,     cloudDensity);
		Shader.SetGlobalFloat(Resources.ID_CloudColoring,    Clouds.Coloring);
		Shader.SetGlobalFloat(Resources.ID_CloudAttenuation, cloudAttenuation);
		Shader.SetGlobalFloat(Resources.ID_CloudSaturation,  cloudSaturation);
		Shader.SetGlobalFloat(Resources.ID_CloudScattering,  Clouds.Scattering);
		Shader.SetGlobalFloat(Resources.ID_CloudBrightness,  Clouds.Brightness);
		Shader.SetGlobalVector(Resources.ID_CloudOffset,     Components.Animation.OffsetUV);
		Shader.SetGlobalVector(Resources.ID_CloudWind,       Components.Animation.CloudUV);
		Shader.SetGlobalVector(Resources.ID_CloudSize,       new Vector3(Clouds.Size * 4, Clouds.Size, Clouds.Size * 4));

		Shader.SetGlobalFloat(Resources.ID_StarSize,       Stars.Size);
		Shader.SetGlobalFloat(Resources.ID_StarBrightness, Stars.Brightness);
		Shader.SetGlobalFloat(Resources.ID_StarVisibility, (1-Atmosphere.Fogginess) * (1-LerpValue));

		Shader.SetGlobalFloat(Resources.ID_SunMeshContrast,   1f / Mathf.Max(0.001f, Sun.MeshContrast));
		Shader.SetGlobalFloat(Resources.ID_SunMeshBrightness, Sun.MeshBrightness * (1-Atmosphere.Fogginess));

		Shader.SetGlobalFloat(Resources.ID_MoonMeshContrast,   1f / Mathf.Max(0.001f, Moon.MeshContrast));
		Shader.SetGlobalFloat(Resources.ID_MoonMeshBrightness, Moon.MeshBrightness * (1-Atmosphere.Fogginess));

		Shader.SetGlobalVector(Resources.ID_kBetaMie, kBetaMie);
		Shader.SetGlobalVector(Resources.ID_kSun,     kSun);
		Shader.SetGlobalVector(Resources.ID_k4PI,     k4PI);
		Shader.SetGlobalVector(Resources.ID_kRadius,  kRadius);
		Shader.SetGlobalVector(Resources.ID_kScale,   kScale);

		Shader.SetGlobalMatrix(Resources.ID_World2Sky, Components.DomeTransform.worldToLocalMatrix);
		Shader.SetGlobalMatrix(Resources.ID_Sky2World, Components.DomeTransform.localToWorldMatrix);
	}
}
