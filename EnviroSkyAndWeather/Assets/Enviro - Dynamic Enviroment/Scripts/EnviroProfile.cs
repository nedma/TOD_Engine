using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[Serializable]
public class EnviroQualitySettings
{

	[Range(0,1)][Tooltip("Modifies the amount of particles used in weather effects.")]
	public float GlobalParticleEmissionRates = 1f;
	[Tooltip("How often Enviro Growth Instances should be updated. Lower value = smoother growth and more frequent updates but more perfomance hungry!")]
	public float UpdateInterval = 0.5f; //Attention: lower value = smoother growth and more frequent updates but more perfomance hungry!
}
	
[Serializable]
public class BackgroundRenderingSettings
{
	public bool backgroundRendering = false;
	public int backgroundLayer = 29;
	public float backgroundViewDistance = 5000f;
}

[Serializable]
public class EnviroSatelliteVariables
{
	[Tooltip("Name of this satellite")]
	public string name;
	[Tooltip("Prefab with model that get instantiated.")]
	public GameObject prefab = null;
	[Tooltip("This value will influence the satellite orbitpositions.")]
	public float orbit_X;
	[Tooltip("This value will influence the satellite orbitpositions.")]
	public float orbit_Y;
	[Tooltip("The speed of the satellites orbit.")]
	public float speed;
}

[Serializable]
public class EnviroSeasonSettings
{
	[Tooltip("How many days in spring?")]
	public float SpringInDays = 90f;
	[Tooltip("How many days in summer?")]
	public float SummerInDays = 93f;
	[Tooltip("How many days in autumn?")]
	public float AutumnInDays = 92f;
	[Tooltip("How many days in winter?")]
	public float WinterInDays = 90f;
}



[Serializable]
public class EnviroWeatherSettings 
{
	[Header("Zones Setup:")]
	[Tooltip("Tag for zone triggers. Create and assign a tag to this gameObject")]
	public bool useTag;

	[Header("Weather Transition Settings:")]
	[Tooltip("Defines the speed of wetness will raise when it is raining.")]
	public float wetnessAccumulationSpeed = 0.05f;
	[Tooltip("Defines the speed of wetness will dry when it is not raining.")]
	public float wetnessDryingSpeed = 0.05f;
	[Tooltip("Defines the speed of snow will raise when it is snowing.")]
	public float snowAccumulationSpeed = 0.05f;
	[Tooltip("Defines the speed of snow will meld when it is not snowing.")]
	public float snowMeltingSpeed = 0.05f;
	[Tooltip("Defines the speed of clouds will change when weather conditions changed.")]
	public float cloudTransitionSpeed = 1f;
	[Tooltip("Defines the speed of fog will change when weather conditions changed.")]
	public float fogTransitionSpeed = 1f;
	[Tooltip("Defines the speed of particle effects will change when weather conditions changed.")]
	public float effectTransitionSpeed = 1f;
}

[Serializable]
public class EnviroSkySettings 
{
	public enum SunAndMoonCalc
	{
		Simple,
		Realistic
	}

	public enum MoonPhases
	{
		Custom,
		Realistic
	}

	public enum SkyboxModi
	{
		Default,
		CustomSkybox,
		CustomColor
	}
	[Header("Sky Mode:")]
	[Tooltip("Select if you want to use enviro skybox your custom material.")]
	public SkyboxModi skyboxMode;
	[Tooltip("If SkyboxMode == CustomSkybox : Assign your skybox material here!")]
	public Material customSkyboxMaterial;
	[Tooltip("If SkyboxMode == CustomColor : Select your sky color here!")]
	public Color customSkyboxColor;

	[Header("Scattering")]
	[Tooltip("Light Wavelength used for atmospheric scattering. Keep it near defaults for earthlike atmospheres, or change for alien or fantasy atmospheres for example.")]
	public Vector3 waveLength = new Vector3(540f,496f,437f);
	[Tooltip("Influence atmospheric scattering.")]
	public float rayleigh = 5.15f;
	[Tooltip("Sky turbidity. Particle in air. Influence atmospheric scattering.")]
	public float turbidity = 1f;
	[Tooltip("Influence scattering near sun.")]
	public float mie = 5f;
	[Tooltip("Influence scattering near sun.")]
	public float g = 0.8f;
	[Tooltip("Intensity gradient for atmospheric scattering. Influence atmospheric scattering based on current sun altitude.")]
	public AnimationCurve scatteringCurve = new AnimationCurve();
	[Tooltip("Color gradient for atmospheric scattering. Influence atmospheric scattering based on current sun altitude.")]
	public Gradient scatteringColor;

	[Header("Sun")]
	public SunAndMoonCalc sunAndMoonPosition = SunAndMoonCalc.Realistic;
	[Tooltip("Intensity of Sun Influence Scale and Dropoff of sundisk.")]
	public float sunIntensity = 100f;
	[Tooltip("Scale of rendered sundisk.")]
	public float sunDiskScale = 20f;
	[Tooltip("Intenisty of rendered sundisk.")]
	public float sunDiskIntensity = 3f;
	[Tooltip("Color gradient for sundisk. Influence sundisk color based on current sun altitude")]
	public Gradient sunDiskColor;


	[Header("Moon")]
	public MoonPhases moonPhaseMode = MoonPhases.Realistic;
	[Tooltip("The Moon texture.")]
	public Texture moonTexture;
	[Tooltip("Brightness of the moon.")]
	public float moonBrightness = 4f;
	[Tooltip("Glow around moon.")]
	public AnimationCurve moonGlow = new AnimationCurve();
	[Tooltip("Start moon phase when using custom phase mode.(-1f - 1f)")]
	[Range(-1f,1f)]
	public float startMoonPhase = 0.0f;

	[Header("Sky Color Corrections")]
	[Tooltip("Higher values = brighter sky.")]
	public AnimationCurve skyLuminence = new AnimationCurve();
	[Tooltip("Higher values = stronger colors applied BEFORE clouds rendered!")]
	public AnimationCurve skyColorPower = new AnimationCurve();

	[Header("Tonemapping - LDR")]
	[Tooltip("Tonemapping when using LDR")]
	public float skyExposure = 1.5f;

	[Header("Stars")]
	[Tooltip("A cubemap for night sky.")]
	public Cubemap starsCubeMap;
	[Tooltip("Intensity of stars based on time of day.")]
	public AnimationCurve starsIntensity = new AnimationCurve();
}

[Serializable]
public class EnviroSatellitesSettings 
{
	[Tooltip("List of satellites.")]
	public List<EnviroSatellite> additionalSatellites = new List<EnviroSatellite>();
}

[Serializable] 
public class EnviroLightSettings // All Lightning Variables
{
	[Header("Direct")]
	[Tooltip("Color gradient for sun and moon light based on sun position in sky.")]
	public Gradient LightColor;
	[Tooltip("Direct light (sun/moon) intensity based on sun position in sky")]
	public AnimationCurve directLightIntensity = new AnimationCurve();
	[Tooltip("Realtime shadow strength of the directional light.")][Range(0f,1f)]
	public float shadowStrength = 1f;
	[Header("Ambient")]
	[Tooltip("Ambient Rendering Mode.")]
	public UnityEngine.Rendering.AmbientMode ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
	[Tooltip("Ambientlight intensity based on sun position in sky.")]
	public AnimationCurve ambientIntensity = new AnimationCurve();
	[Tooltip("Ambientlight sky color based on sun position in sky.")]
	public Gradient ambientSkyColor;
	[Tooltip("Ambientlight Equator color based on sun position in sky.")]
	public Gradient ambientEquatorColor;
	[Tooltip("Ambientlight Ground color based on sun position in sky.")]
	public Gradient ambientGroundColor;

	[Header("Global Reflections")]
	public bool globalReflections = true;
	public float globalReflectionsIntensity = 0.5f;
	public float globalReflectionsUpdate = 0.025f;
}


[Serializable]
public class EnviroFogSettings 
{
	[Header("Mode")]
	[Tooltip("Unity's fog mode.")]
	public FogMode Fogmode = FogMode.Exponential;
	[Header("Distance Fog")]
	[Tooltip("Use distance fog?")]
	public bool distanceFog = true;
	[Tooltip("Use radial distance fog?")]
	public bool useRadialDistance = true;
	[Tooltip("The distance where fog starts.")]
	public float startDistance = 0.0f;
	[Range(0f,10f)][Tooltip("The intensity of distance fog.")]
	public float distanceFogIntensity = 4.0f;
	[Range(0f,1f)][Tooltip("The maximum density of fog.")]
	public float maximumFogDensity = 0.9f;
	[Header("Height Fog")]
	[Tooltip("Use heightbased fog?")]
	public bool heightFog = true;
	[Tooltip("The height of heightbased fog.")]
	public float height = 90.0f;
	[Range(0f,1f)][Tooltip("The intensity of heightbased fog.")]
	public float heightFogIntensity = 1f;
	[HideInInspector]
	public float heightDensity = 0.15f;
	[Range(0f,1f)][Tooltip("The noise intensity of heightbased fog.")]
	public float noiseIntensity = 0.01f;
	[Range(0f,0.1f)][Tooltip("The noise scale of heightbased fog.")]
	public float noiseScale = 0.001f;
	[Tooltip("The speed and direction of heightbased fog.")]
	public Vector2 heightFogVelocity = new Vector2(0.001f,0.015f);
	[Header("Fog Dithering")]
	[Tooltip("Fog dithering settings to reduce color banding.")]
	public float fogDitheringScale = 240f;
	[Tooltip("Fog dithering settings to reduce color banding.")]
	public float fogDitheringIntensity = 0.5f;

	[HideInInspector]
	public float skyFogIntensity = 1f;
}

[Serializable]
public class EnviroLightShaftsSettings 
{
	[Header("Quality Settings")]
	[Tooltip("Lightshafts resolution quality setting.")]
	public EnviroLightShafts.SunShaftsResolution resolution = EnviroLightShafts.SunShaftsResolution.Normal;
	[Tooltip("Lightshafts blur mode.")]
	public EnviroLightShafts.ShaftsScreenBlendMode screenBlendMode = EnviroLightShafts.ShaftsScreenBlendMode.Screen;
	[Tooltip("Use cameras depth to hide lightshafts?")]
	public bool useDepthTexture = true;

	[Header("Intensity Settings")]
	[Tooltip("Color gradient for lightshafts based on sun position.")]
	public Gradient lightShaftsColorSun;
	[Tooltip("Color gradient for lightshafts based on moon position.")]
	public Gradient lightShaftsColorMoon;
	[Tooltip("Treshhold gradient for lightshafts based on sun position. This will influence lightshafts intensity!")]
	public Gradient thresholdColorSun;
	[Tooltip("Treshhold gradient for lightshafts based on moon position. This will influence lightshafts intensity!")]
	public Gradient thresholdColorMoon;
	[Tooltip("Radius of blurring applied.")]
	public float blurRadius = 6f;
	[Tooltip("Global Lightshafts intensity.")]
	public float intensity = 0.6f;
	[Tooltip("Lightshafts maximum radius.")]
	public float maxRadius = 10f;
}

[Serializable]
public class EnviroCloudsLayerVariables
{
	public string Name;
	[Header("Mesh Setup")]
	[Range(1,100)][Tooltip("Clouds Quality. High Performance Impact! Call InitClouds() to apply change in runtime.")]
	public int Quality = 25;
	[Tooltip("Segments of generated clouds mesh. Good for curvate meshes. If curved disabled keep it low.")]
	public int segmentCount = 3;
	[Tooltip("Thickness of generated clouds mesh.")]
	public float thickness = 0.4f;
	[Tooltip("Clouds mesh curved at horizon?")]
	public bool curved;
	[Tooltip("Clouds mesh curve intensity.")]
	public float curvedIntensity = 0.1f;

	[Header("Material Setup")]
	[Tooltip("The texture used for this cloud layer.")]
	public Texture myCloudsTexture;
	[Range(0.5f,2f)][Tooltip("Clouds tiling/scale modificator.")]
	public float Scaling = 1f; 
	[Tooltip("Enable to let cat this cloudlayer shadows.")]
	public bool canCastShadows = false;
	[Tooltip("Altitude of this cloud layer")]
	public float layerAltitude = 0.1f;
	[Tooltip("Offset used for multi layer clouds rendering.")]
	public float LayerOffset = 0.5f;
}
[Serializable]
public class EnviroAudioSettings // Default cloud settings, will be changed on runtime if Weather is enabled!
{
[Tooltip("A list of all possible thunder audio effects.")]
public List<AudioClip> ThunderSFX = new List<AudioClip> ();
}

[Serializable]
public class EnviroCloudSettings // Default cloud settings, will be changed on runtime if Weather is enabled!
{
	/*public enum CloudRenderResolution
	{
		Legacy,
		sixteenths,
		eighth,
		quarter,
		half,
		full
	}*/

	[Header("Clouds layers")]
	public List<EnviroCloudsLayerVariables> cloudsLayers = new List<EnviroCloudsLayerVariables> ();

	[Range(1f,3f)][Header("Global Clouds Position")]
	public float worldScale = 1.25f;
	[Tooltip("When enabled, clouds will stay at this height. When disabled clouds height will be calculated by player position.")]
	public bool FixedAltitude = false;
	[Tooltip("The altitude of the clouds when 'FixedAltitude' is enabled.")]
	public float cloudsAltitude = 0f;

	[Header("Clouds Wind Animation")]
	[Range(-1f,1f)][Tooltip("Time scale / wind animation speed of clouds.")]
	public float cloudsTimeScale = 1f;
	[Range(0f,0.1f)][Tooltip("Global clouds wind speed modificator.")]
	public float cloudsWindStrengthModificator = 0.001f;

	public bool useWindZoneDirection;
	[Range(-1f,1f)][Tooltip("Global clouds wind direction X axes.")]
	public float cloudsWindDirectionX = 1f;
	[Range(-1f,1f)][Tooltip("Global clouds wind direction Y axes.")]
	public float cloudsWindDirectionY = 1f;

	[Header("Cloud Rendering")]
	[Range(1,16)]
	public int cloudsRenderResolution = 5;

	[Header("Cloud Lighting")]
	[Tooltip("Global Color for clouds based sun positon.")]
	public Gradient skyColor;
	[Tooltip("Global Color for clouds based sun positon.")]
	public Gradient sunHighlightColor;
	[Tooltip("Global Color for clouds based moon positon.")]
	public Gradient moonHighlightColor;
	[Tooltip("Direct Light intensity for clouds based on time of day.")]
	public AnimationCurve lightIntensity = new AnimationCurve();
}


[System.Serializable]
//[CreateAssetMenu(fileName = "EnviroProfile", menuName = "EnviroProfile",order =1)]
public class EnviroProfile : ScriptableObject 
{
	[HideInInspector]public string version;
	public EnviroLightSettings lightSettings = new EnviroLightSettings();
	public EnviroSkySettings skySettings = new EnviroSkySettings();
	public EnviroCloudSettings cloudsSettings = new EnviroCloudSettings();
	public EnviroWeatherSettings weatherSettings = new EnviroWeatherSettings();
	public EnviroFogSettings fogSettings = new EnviroFogSettings();
	public EnviroLightShaftsSettings lightshaftsSettings = new EnviroLightShaftsSettings();
	public EnviroSeasonSettings seasonsSettings = new EnviroSeasonSettings();
	public EnviroAudioSettings audioSettings = new EnviroAudioSettings();
	public EnviroSatellitesSettings satelliteSettings = new EnviroSatellitesSettings();
	public BackgroundRenderingSettings backgroundSettings = new BackgroundRenderingSettings();
	public EnviroQualitySettings qualitySettings = new EnviroQualitySettings();

	// Inspector categories
	public enum settingsMode
	{
		Lighting,
		Sky,
		Weather,
		Clouds,
		Fog,
		Lightshafts,
		Season,
		Satellites,
		Background,
		Audio,
		Quality
	}
	[HideInInspector]public settingsMode viewMode;
	[HideInInspector]public bool showPlayerSetup = true;
	[HideInInspector]public bool showRenderingSetup = false;
	[HideInInspector]public bool showComponentsSetup = false;
	[HideInInspector]public bool showTimeUI = false;
	[HideInInspector]public bool showWeatherUI = false;
	[HideInInspector]public bool showAudioUI = false;
	[HideInInspector]public bool showEffectsUI = false;
	[HideInInspector]public bool modified;
}

public static class EnviroProfileCreation {
	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Enviro/Profile")]
	public static EnviroProfile CreateNewEnviroProfile()
	{
		EnviroProfile profile = ScriptableObject.CreateInstance<EnviroProfile>();

		profile.version = "1.9.0";
		// Setup new profile with default settings
		SetupDefaults (profile);

		// Create and save the new profile with unique name
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets/Enviro - Dynamic Enviroment/Profiles";
		} 
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + "Enviro Profile" + ".asset");
		AssetDatabase.CreateAsset (profile, assetPathAndName);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
		//EditorUtility.FocusProjectWindow ();
		//Selection.activeObject = profile;
		return profile;
	}
#endif
	
	public static void SetupDefaults (EnviroProfile profile)
	{
		List<Color> gradientColors = new List<Color> ();
		List<float> gradientTimes = new List<float> ();

		//Light Color
		gradientColors.Add (GetColor ("#4C5570"));gradientTimes.Add (0f);
		gradientColors.Add (GetColor ("#4C5570"));gradientTimes.Add (0.46f);
		gradientColors.Add (GetColor ("#C98842"));gradientTimes.Add (0.51f);
		gradientColors.Add (GetColor ("#EAC8A4"));gradientTimes.Add (0.56f);
		gradientColors.Add (GetColor ("#EADCCE"));gradientTimes.Add (1f);
		profile.lightSettings.LightColor = CreateGradient (gradientColors, gradientTimes);
		gradientColors = new List<Color> ();gradientTimes = new List<float> ();
		//Light Intensity
		profile.lightSettings.directLightIntensity.AddKey(CreateKey(1f,0f));
		profile.lightSettings.directLightIntensity.AddKey(CreateKey(1f,0.44f));
		profile.lightSettings.directLightIntensity.AddKey(CreateKey(0.25f,0.48f));
		profile.lightSettings.directLightIntensity.AddKey(CreateKey(1f,0.52f,20f,20f));
		profile.lightSettings.directLightIntensity.AddKey(CreateKey(1.5f,0.6f));
		profile.lightSettings.directLightIntensity.AddKey(CreateKey(1.5f,1f));
		//Ambient Intensity
		profile.lightSettings.ambientIntensity.AddKey(CreateKey(0.75f,0f));
		profile.lightSettings.ambientIntensity.AddKey(CreateKey(0.75f,1f));
		//Ambient SkyColor
		gradientColors.Add (GetColor ("#4C5570"));gradientTimes.Add (0f);
		gradientColors.Add (GetColor ("#4C5570"));gradientTimes.Add (0.46f);
		gradientColors.Add (GetColor ("#C98842"));gradientTimes.Add (0.51f);
		gradientColors.Add (GetColor ("#99B2C3"));gradientTimes.Add (0.57f);
		gradientColors.Add (GetColor ("#99B2C3"));gradientTimes.Add (1f);
		profile.lightSettings.ambientSkyColor = CreateGradient (gradientColors, gradientTimes);
		gradientColors = new List<Color> ();gradientTimes = new List<float> ();
		//Ambient EquatorColor
		profile.lightSettings.ambientEquatorColor = CreateGradient(GetColor("#2E3344"),0f,GetColor("#414852"),1f);
		//Ambient GroundColor
		profile.lightSettings.ambientGroundColor = CreateGradient(GetColor("#272B39"),0f,GetColor("#3E3631"),1f);
		//ScatteringIntensity
		profile.skySettings.scatteringCurve.AddKey(CreateKey(-25f,0f));
		profile.skySettings.scatteringCurve.AddKey(CreateKey (-10f, 0.5f,55f,55f));
		profile.skySettings.scatteringCurve.AddKey(CreateKey (6.5f, 0.52f,35f,35f));
		profile.skySettings.scatteringCurve.AddKey(CreateKey(11f,1f));
		// Scattering Color Tint
		gradientColors.Add (GetColor ("#8492C8"));gradientTimes.Add (0f);
		gradientColors.Add (GetColor ("#8492C8"));gradientTimes.Add (0.45f);
		gradientColors.Add (GetColor ("#FFB69C"));gradientTimes.Add (0.527f);
		gradientColors.Add (GetColor ("#D2D2D2"));gradientTimes.Add (0.75f);
		gradientColors.Add (GetColor ("#D2D2D2"));gradientTimes.Add (1f);
		profile.skySettings.scatteringColor = CreateGradient (gradientColors, gradientTimes);
		gradientColors = new List<Color> ();gradientTimes = new List<float> ();
		//Sun Disk Color
		gradientColors.Add (GetColor ("#0A0300"));gradientTimes.Add (0f);
		gradientColors.Add (GetColor ("#FF6211"));gradientTimes.Add (0.45f);
		gradientColors.Add (GetColor ("#FF6917"));gradientTimes.Add (0.55f);
		gradientColors.Add (GetColor ("#FFE2CB"));gradientTimes.Add (0.75f);
		gradientColors.Add (GetColor ("#FFFFFF"));gradientTimes.Add (1f);
		profile.skySettings.sunDiskColor = CreateGradient (gradientColors, gradientTimes);
		gradientColors = new List<Color> ();gradientTimes = new List<float> ();
		//Moon Glow
		profile.skySettings.moonGlow.AddKey(CreateKey (1f, 0f));
		profile.skySettings.moonGlow.AddKey(CreateKey (0f, 0.65f));
		profile.skySettings.moonGlow.AddKey(CreateKey (0f, 1f));
		//Sky Luminance
		profile.skySettings.skyLuminence.AddKey(CreateKey(0f,0f));
		profile.skySettings.skyLuminence.AddKey(CreateKey(0.15f,0.5f));
		profile.skySettings.skyLuminence.AddKey(CreateKey(0.105f,0.62f));
		profile.skySettings.skyLuminence.AddKey(CreateKey(0.1f,1f));
		//Sky Color Power
		profile.skySettings.skyColorPower.AddKey(CreateKey(1.5f,0f));
		profile.skySettings.skyColorPower.AddKey(CreateKey(1.25f,1f));
		//Stars Intensity
		profile.skySettings.starsIntensity.AddKey(CreateKey(0.3f,0f));
		profile.skySettings.starsIntensity.AddKey(CreateKey(0.015f,0.5f));
		profile.skySettings.starsIntensity.AddKey(CreateKey(0.0f,0.6f));
		profile.skySettings.starsIntensity.AddKey(CreateKey(0.0f,1f));
		// Get Texture
		profile.skySettings.moonTexture = GetAssetTexture ("tex_enviro_moon");
		profile.skySettings.starsCubeMap = GetAssetCubemap ("cube_enviro_stars");
		//Create default cloud layers:
		Texture clouds1 = GetAssetTexture("tex_enviro_clouds_1");
		Texture clouds2 = GetAssetTexture("tex_enviro_clouds_2");
		if (clouds1 == null || clouds2 == null) {
			Debug.Log ("Cannot find cloud textures");
			return;
		}
		profile.cloudsSettings.cloudsLayers.Add (CreateCloudLayer ("First Layer", 35, 8, 0.04f, true, 0.15f, clouds1, 1.25f, false, 0f, 0f));
		profile.cloudsSettings.cloudsLayers.Add (CreateCloudLayer ("Second Layer", 20, 8, 0.04f, true, 0.15f, clouds1, 1.75f, false, 0.05f, 0.5f));
		profile.cloudsSettings.cloudsLayers.Add (CreateCloudLayer ("High Altitude Clouds", 7, 8, 0.015f, true, 0.25f, clouds2, 1.75f, false, 0.15f, 0.25f));
		//Clouds Moon Highlight
		profile.cloudsSettings.moonHighlightColor = CreateGradient(GetColor("#232228"),0f,GetColor("#B6BCDC"),1f);
		//Clouds Sky Color
		gradientColors.Add (GetColor ("#17171A"));gradientTimes.Add (0f);
		gradientColors.Add (GetColor ("#17171A"));gradientTimes.Add (0.455f);
		gradientColors.Add (GetColor ("#3D3D3B"));gradientTimes.Add (0.48f);
		gradientColors.Add (GetColor ("#EEB279"));gradientTimes.Add (0.53f);
		gradientColors.Add (GetColor ("#EEF0FF"));gradientTimes.Add (0.6f);
		gradientColors.Add (GetColor ("#ECEEFF"));gradientTimes.Add (1f);
		profile.cloudsSettings.skyColor = CreateGradient (gradientColors, gradientTimes);
		gradientColors = new List<Color> ();gradientTimes = new List<float> ();
		//Clouds Sun Color
		gradientColors.Add (GetColor ("#17171A"));gradientTimes.Add (0f);
		gradientColors.Add (GetColor ("#17171A"));gradientTimes.Add (0.455f);
		gradientColors.Add (GetColor ("#3D3D3B"));gradientTimes.Add (0.48f);
		gradientColors.Add (GetColor ("#EEB279"));gradientTimes.Add (0.53f);
		gradientColors.Add (GetColor ("#CECECE"));gradientTimes.Add (0.58f);
		gradientColors.Add (GetColor ("#CECECE"));gradientTimes.Add (1f);
		profile.cloudsSettings.sunHighlightColor = CreateGradient (gradientColors, gradientTimes);
		gradientColors = new List<Color> ();gradientTimes = new List<float> ();
		//clouds light intensity
		profile.cloudsSettings.lightIntensity.AddKey(CreateKey(5f,0f));
		profile.cloudsSettings.lightIntensity.AddKey(CreateKey(4.75f,0.4333f));
		profile.cloudsSettings.lightIntensity.AddKey(CreateKey(0.6f,0.51f));
		profile.cloudsSettings.lightIntensity.AddKey(CreateKey(0.5f,0.75f));
		profile.cloudsSettings.lightIntensity.AddKey(CreateKey(0.48f,1f));
		//LightShafts
		gradientColors.Add (GetColor ("#FF703C"));gradientTimes.Add (0f);
		gradientColors.Add (GetColor ("#FF5D00"));gradientTimes.Add (0.47f);
		gradientColors.Add (GetColor ("#FFF4DF"));gradientTimes.Add (0.65f);
		gradientColors.Add (GetColor ("#FFFFFF"));gradientTimes.Add (1f);
		profile.lightshaftsSettings.lightShaftsColorSun = CreateGradient (gradientColors, gradientTimes);
		gradientColors = new List<Color> ();gradientTimes = new List<float> ();
		profile.lightshaftsSettings.lightShaftsColorMoon = CreateGradient(GetColor("#94A8E5"),0f,GetColor("#94A8E5"),1f);
		gradientColors.Add (GetColor ("#1D1D1D"));gradientTimes.Add (0f);
		gradientColors.Add (GetColor ("#1D1D1D"));gradientTimes.Add (0.43f);
		gradientColors.Add (GetColor ("#A6A6A6"));gradientTimes.Add (0.54f);
		gradientColors.Add (GetColor ("#D0D0D0"));gradientTimes.Add (0.65f);
		gradientColors.Add (GetColor ("#C3C3C3"));gradientTimes.Add (1f);
		profile.lightshaftsSettings.thresholdColorSun = CreateGradient (gradientColors, gradientTimes);
		gradientColors = new List<Color> ();gradientTimes = new List<float> ();
		profile.lightshaftsSettings.thresholdColorMoon =  CreateGradient(GetColor("#0B0B0B"),0f,GetColor("#000000"),1f);
		//Audio
		for (int i = 0; i < 8; i++) {
			profile.audioSettings.ThunderSFX.Add (GetAudioClip ("SFX_Thunder_" + (i+1)));
		}
	}



	public static GameObject GetAssetPrefab(string name)
	{
		#if UNITY_EDITOR
		string[] assets = AssetDatabase.FindAssets(name, null);
		for (int idx = 0; idx < assets.Length; idx++)
		{
			string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
			if (path.Contains(".prefab"))
			{
				return AssetDatabase.LoadAssetAtPath<GameObject>(path);
			}
		}
		#endif
		return null;
	}

	public static AudioClip GetAudioClip(string name)
	{
		#if UNITY_EDITOR
		string[] assets = AssetDatabase.FindAssets(name, null);
		for (int idx = 0; idx < assets.Length; idx++)
		{
			string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
			if (path.Contains(".wav"))
			{
				return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
			}
		}
		#endif
		return null;
	}

	public static Cubemap GetAssetCubemap(string name)
	{
		#if UNITY_EDITOR
		string[] assets = AssetDatabase.FindAssets(name, null);
		for (int idx = 0; idx < assets.Length; idx++)
		{
			string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
			if (path.Contains(".png"))
			{
				return AssetDatabase.LoadAssetAtPath<Cubemap>(path);
			}
		}
		#endif
		return null;
	}

	public static Texture GetAssetTexture(string name)
	{
		#if UNITY_EDITOR
		string[] assets = AssetDatabase.FindAssets(name, null);
		for (int idx = 0; idx < assets.Length; idx++)
		{
			string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
			if (path.Length > 0)
			{
				return AssetDatabase.LoadAssetAtPath<Texture>(path);
			}
		}
		#endif
		return null;
	}
		
	public static Gradient CreateGradient(Color clr1, float time1, Color clr2, float time2)
	{
		Gradient nG = new Gradient ();
		GradientColorKey[] gClr = new GradientColorKey[2];
		GradientAlphaKey[] gAlpha = new GradientAlphaKey[2];

		gClr [0].color = clr1;
		gClr [0].time = time1;
		gClr [1].color = clr2;
		gClr [1].time = time2;

		gAlpha [0].alpha = 1f;
		gAlpha [0].time = 0f;
		gAlpha [1].alpha = 1f;
		gAlpha [1].time = 1f;

		nG.SetKeys (gClr, gAlpha);

		return nG;
	}

	public static Gradient CreateGradient(List<Color> clrs,List<float>times)
	{
		Gradient nG = new Gradient ();

		GradientColorKey[] gClr = new GradientColorKey[clrs.Count];
		GradientAlphaKey[] gAlpha = new GradientAlphaKey[2];

		for (int i = 0; i < clrs.Count; i++) {
			gClr [i].color = clrs [i];
			gClr [i].time = times[i];
		}

		gAlpha [0].alpha = 1f;
		gAlpha [0].time = 0f;
		gAlpha [1].alpha = 1f;
		gAlpha [1].time = 1f;

		nG.SetKeys (gClr, gAlpha);
		return nG;
	}

	public static Color GetColor (string hex)
	{
		Color clr = new Color ();	
		ColorUtility.TryParseHtmlString (hex, out clr);
		return clr;
	}
	
	public static Keyframe CreateKey (float value, float time)
	{
		Keyframe k = new Keyframe();
		k.value = value;
		k.time = time;
		return k;
	}

	public static Keyframe CreateKey (float value, float time, float inTangent, float outTangent)
	{
		Keyframe k = new Keyframe();
		k.value = value;
		k.time = time;
		k.inTangent = inTangent;
		k.outTangent = outTangent;
		return k;
	}

	public static EnviroCloudsLayerVariables CreateCloudLayer (string name, int quality,int segments,float thick, bool curved, float curveIntensity, Texture tex, float scale, bool shadows, float altitude, float offset)
	{
		EnviroCloudsLayerVariables newLayer = new EnviroCloudsLayerVariables ();
		newLayer.Name = name;
		newLayer.Quality = quality;
		newLayer.segmentCount = segments;
		newLayer.thickness = thick;
		newLayer.curved = curved;
		newLayer.curvedIntensity = curveIntensity;
		newLayer.myCloudsTexture = tex;
		newLayer.Scaling = scale;
		newLayer.canCastShadows = shadows;
		newLayer.layerAltitude = altitude;
		newLayer.LayerOffset = offset;
		return newLayer;
	}		
}
