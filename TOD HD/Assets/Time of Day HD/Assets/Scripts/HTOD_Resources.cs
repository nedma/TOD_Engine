using UnityEngine;

/// Material and mesh wrapper class.
///
/// Component of the sky dome parent game object.

public class HTOD_Resources : MonoBehaviour
{
	public Material Skybox;

	public Mesh MoonLOD0;
	public Mesh MoonLOD1;
	public Mesh MoonLOD2;

	public Mesh SkyLOD0;
	public Mesh SkyLOD1;
	public Mesh SkyLOD2;

	public Mesh CloudsLOD0;
	public Mesh CloudsLOD1;
	public Mesh CloudsLOD2;

	public Mesh StarsLOD0;
	public Mesh StarsLOD1;
	public Mesh StarsLOD2;

	public int ID_SunLightColor  { get; private set; }
	public int ID_MoonLightColor { get; private set; }

	public int ID_SunSkyColor  { get; private set; }
	public int ID_MoonSkyColor { get; private set; }

	public int ID_SunMeshColor  { get; private set; }
	public int ID_MoonMeshColor { get; private set; }

	public int ID_SunCloudColor  { get; private set; }
	public int ID_MoonCloudColor { get; private set; }

	public int ID_FogColor     { get; private set; }
	public int ID_GroundColor  { get; private set; }
	public int ID_AmbientColor { get; private set; }

	public int ID_SunDirection   { get; private set; }
	public int ID_MoonDirection  { get; private set; }
	public int ID_LightDirection { get; private set; }

	public int ID_LocalSunDirection   { get; private set; }
	public int ID_LocalMoonDirection  { get; private set; }
	public int ID_LocalLightDirection { get; private set; }

	public int ID_Contrast       { get; private set; }
	public int ID_Brightness     { get; private set; }
	public int ID_Fogginess      { get; private set; }
	public int ID_Directionality { get; private set; }

	public int ID_MoonHaloPower { get; private set; }
	public int ID_MoonHaloColor { get; private set; }

	public int ID_CloudSize        { get; private set; }
	public int ID_CloudOpacity     { get; private set; }
	public int ID_CloudCoverage    { get; private set; }
	public int ID_CloudSharpness   { get; private set; }
	public int ID_CloudDensity     { get; private set; }
	public int ID_CloudColoring    { get; private set; }
	public int ID_CloudAttenuation { get; private set; }
	public int ID_CloudSaturation  { get; private set; }
	public int ID_CloudScattering  { get; private set; }
	public int ID_CloudBrightness  { get; private set; }
	public int ID_CloudMultiplier  { get; private set; }
	public int ID_CloudOffset      { get; private set; }
	public int ID_CloudWind        { get; private set; }

	public int ID_StarSize       { get; private set; }
	public int ID_StarBrightness { get; private set; }
	public int ID_StarVisibility { get; private set; }

	public int ID_SunMeshContrast   { get; private set; }
	public int ID_SunMeshBrightness { get; private set; }

	public int ID_MoonMeshContrast   { get; private set; }
	public int ID_MoonMeshBrightness { get; private set; }

	public int ID_kBetaMie { get; private set; }
	public int ID_kSun     { get; private set; }
	public int ID_k4PI     { get; private set; }
	public int ID_kRadius  { get; private set; }
	public int ID_kScale   { get; private set; }

	public int ID_World2Sky { get; private set; }
	public int ID_Sky2World { get; private set; }

	/// Initializes all resource references.
	public void Initialize()
	{
		ID_SunLightColor  = Shader.PropertyToID("HTOD_SunLightColor");
		ID_MoonLightColor = Shader.PropertyToID("HTOD_MoonLightColor");

		ID_SunSkyColor  = Shader.PropertyToID("HTOD_SunSkyColor");
		ID_MoonSkyColor = Shader.PropertyToID("HTOD_MoonSkyColor");

		ID_SunMeshColor  = Shader.PropertyToID("HTOD_SunMeshColor");
		ID_MoonMeshColor = Shader.PropertyToID("HTOD_MoonMeshColor");

		ID_SunCloudColor  = Shader.PropertyToID("HTOD_SunCloudColor");
		ID_MoonCloudColor = Shader.PropertyToID("HTOD_MoonCloudColor");

		ID_FogColor     = Shader.PropertyToID("HTOD_FogColor");
		ID_GroundColor  = Shader.PropertyToID("HTOD_GroundColor");
		ID_AmbientColor = Shader.PropertyToID("HTOD_AmbientColor");

		ID_SunDirection   = Shader.PropertyToID("HTOD_SunDirection");
		ID_MoonDirection  = Shader.PropertyToID("HTOD_MoonDirection");
		ID_LightDirection = Shader.PropertyToID("HTOD_LightDirection");

		ID_LocalSunDirection   = Shader.PropertyToID("HTOD_LocalSunDirection");
		ID_LocalMoonDirection  = Shader.PropertyToID("HTOD_LocalMoonDirection");
		ID_LocalLightDirection = Shader.PropertyToID("HTOD_LocalLightDirection");

		ID_Contrast       = Shader.PropertyToID("HTOD_Contrast");
		ID_Brightness     = Shader.PropertyToID("HTOD_Brightness");
		ID_Fogginess      = Shader.PropertyToID("HTOD_Fogginess");
		ID_Directionality = Shader.PropertyToID("HTOD_Directionality");

		ID_MoonHaloPower = Shader.PropertyToID("HTOD_MoonHaloPower");
		ID_MoonHaloColor = Shader.PropertyToID("HTOD_MoonHaloColor");

		ID_CloudSize        = Shader.PropertyToID("HTOD_CloudSize");
		ID_CloudOpacity     = Shader.PropertyToID("HTOD_CloudOpacity");
		ID_CloudCoverage    = Shader.PropertyToID("HTOD_CloudCoverage");
		ID_CloudSharpness   = Shader.PropertyToID("HTOD_CloudSharpness");
		ID_CloudDensity     = Shader.PropertyToID("HTOD_CloudDensity");
		ID_CloudColoring    = Shader.PropertyToID("HTOD_CloudColoring");
		ID_CloudAttenuation = Shader.PropertyToID("HTOD_CloudAttenuation");
		ID_CloudSaturation  = Shader.PropertyToID("HTOD_CloudSaturation");
		ID_CloudScattering  = Shader.PropertyToID("HTOD_CloudScattering");
		ID_CloudBrightness  = Shader.PropertyToID("HTOD_CloudBrightness");
		ID_CloudOffset      = Shader.PropertyToID("HTOD_CloudOffset");
		ID_CloudWind        = Shader.PropertyToID("HTOD_CloudWind");

		ID_StarSize       = Shader.PropertyToID("HTOD_StarSize");
		ID_StarBrightness = Shader.PropertyToID("HTOD_StarBrightness");
		ID_StarVisibility = Shader.PropertyToID("HTOD_StarVisibility");

		ID_SunMeshContrast   = Shader.PropertyToID("HTOD_SunMeshContrast");
		ID_SunMeshBrightness = Shader.PropertyToID("HTOD_SunMeshBrightness");

		ID_MoonMeshContrast   = Shader.PropertyToID("HTOD_MoonMeshContrast");
		ID_MoonMeshBrightness = Shader.PropertyToID("HTOD_MoonMeshBrightness");

		ID_kBetaMie = Shader.PropertyToID("HTOD_kBetaMie");
		ID_kSun     = Shader.PropertyToID("HTOD_kSun");
		ID_k4PI     = Shader.PropertyToID("HTOD_k4PI");
		ID_kRadius  = Shader.PropertyToID("HTOD_kRadius");
		ID_kScale   = Shader.PropertyToID("HTOD_kScale");

		ID_World2Sky = Shader.PropertyToID("HTOD_World2Sky");
		ID_Sky2World = Shader.PropertyToID("HTOD_Sky2World");
	}
}
