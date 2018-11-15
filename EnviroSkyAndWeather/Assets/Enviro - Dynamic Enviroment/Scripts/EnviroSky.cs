////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////        EnviroSky- Renders sky with sun, moon, clouds and weather.          ////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public class EnviroSeasons
{
	public enum Seasons
	{
		Spring,
		Summer,
		Autumn,
		Winter,
	}
	[Tooltip("When enabled the system will change seasons automaticly when enough days passed.")]
	public bool calcSeasons; // if unticked you can manually overwrite current seas. Ticked = automaticly updates seasons
	[Tooltip("The current season.")]
	public Seasons currentSeasons;
	[HideInInspector]
	public Seasons lastSeason;
}

[Serializable]
public class EnviroAudio // AudioSetup variables
{
	[Tooltip("The prefab with AudioSources used by Enviro. Will be instantiated at runtime.")]
	public GameObject SFXHolderPrefab;

	[Header("Volume Settings:")]
	[Range(0f,1f)][Tooltip("The volume of ambient sounds played by enviro.")]
	public float ambientSFXVolume = 0.5f;
	[Range(0f,1f)][Tooltip("The volume of weather sounds played by enviro.")]
	public float weatherSFXVolume = 1.0f;

	[HideInInspector]public EnviroAudioSource currentAmbientSource;
	[HideInInspector]public float ambientSFXVolumeMod = 0f;
	[HideInInspector]public float weatherSFXVolumeMod = 0f;
}



[Serializable]
public class EnviroComponents // References - setup these in inspector! Or use the provided prefab.
{
	[Tooltip("The Enviro sun object.")]
	public GameObject Sun = null;
	[Tooltip("The Enviro moon object.")]
	public GameObject Moon = null;
	[Tooltip("The Enviro Clouds Holder object.")]
	public GameObject Clouds = null;
	[Tooltip("The directional light for direct sun and moon lighting.")]
	public Transform DirectLight;
	[Tooltip("The Enviro global reflection probe for dynamic reflections.")]
	public ReflectionProbe GlobalReflectionProbe;
	[Tooltip("Your WindZone that reflect our weather wind settings.")]
	public WindZone windZone;
	[Tooltip("The Enviro Lighting Flash Component.")]
	public EnviroLightning LightningGenerator; // Creates lightning Flashes
	[Tooltip("Link to the object that hold all additional satellites as childs.")]
	public Transform satellites;
	[Tooltip("Just a transform for stars rotation calculations. ")]
	public Transform starsRotation = null;
}

[Serializable]
public class EnviroSatellite 
{
	[Tooltip("Name of this satellite")]
	public string name;
	[Tooltip("Prefab with model that get instantiated.")]
	public GameObject prefab = null;
	[Tooltip("Orbit distance.")]
	public float orbit;
	[Tooltip("Orbit modification on x axis.")]
	public float xRot;
	[Tooltip("Orbit modification on y axis.")]
	public float yRot;
}

[Serializable]
public class EnviroWeather 
{
	[Tooltip("If disabled the weather will never change.")]
	public bool updateWeather = true;
	[HideInInspector]public List<EnviroWeatherPreset> weatherPresets = new List<EnviroWeatherPreset>();
	[HideInInspector]public List<EnviroWeatherPrefab> WeatherPrefabs = new List<EnviroWeatherPrefab>();
	[Tooltip("List of additional zones. Will be updated on startup!")]
	public List<EnviroZone> zones = new List<EnviroZone>();
	public EnviroWeatherPreset startWeatherPreset;
	[Tooltip("The current active zone.")]
	public EnviroZone currentActiveZone;
	[Tooltip("The current active weather conditions.")]
	public EnviroWeatherPrefab currentActiveWeatherPrefab;
	public EnviroWeatherPreset currentActiveWeatherPreset;

	[HideInInspector]public EnviroWeatherPrefab lastActiveWeatherPrefab;
	[HideInInspector]public EnviroWeatherPreset lastActiveWeatherPreset;

	[HideInInspector]public GameObject VFXHolder;
	[HideInInspector]public float wetness;
	[HideInInspector]public float curWetness;
	[HideInInspector]public float snowStrength;
	[HideInInspector]public float curSnowStrength;
	[HideInInspector]public int thundersfx;
	[HideInInspector]public EnviroAudioSource currentAudioSource;
}
	
[Serializable]
public class EnviroTime // GameTime variables
{
	public enum TimeProgressMode
	{
		None,
		Simulated,
		SystemTime
	}
		
	[Tooltip("None = No time auto time progressing, Simulated = Time calculated with DayLenghtInMinutes, SystemTime = uses your systemTime.")]
	public TimeProgressMode ProgressTime = TimeProgressMode.Simulated;
	[Tooltip("Current Time: minutes")][Range(0,60)]
	public int Seconds  = 0; 
	[Tooltip("Current Time: minutes")][Range(0,60)]
	public int Minutes  = 0; 
	[Tooltip("Current Time: hours")][Range(0,24)]
	public int Hours  = 12; 
	[Tooltip("Current Time: Days")]
	public int Days = 1; 
	[Tooltip("Current Time: Years")]
	public int Years = 1;
	[Space(20)]
	[Tooltip("Day lenght in realtime minutes.")]
	public float DayLengthInMinutes = 5f; // DayLength in realtime minutes
	[Tooltip("Night lenght in realtime minutes.")]
	public float NightLengthInMinutes = 5f; // DayLength in realtime minutes

	[Range(-13,13)][Tooltip("Time offset for timezones")]
	public int utcOffset = 0;
	[Range(-90,90)] [Tooltip("-90,  90   Horizontal earth lines")]
	public float Latitude   = 0f; 
	[Range(-180,180)] [Tooltip("-180, 180  Vertical earth line")]
	public float Longitude  = 0f; 
	[HideInInspector]public float solarTime; 
	[HideInInspector]public float lunarTime;
}
	


[Serializable]
public class EnviroFogging
{
	[Tooltip("Use the enviro fog image effect?")]
	public bool AdvancedFog = true;
	[HideInInspector]
	public float skyFogHeight = 1f;
	[HideInInspector]
	public float skyFogStrength = 0.1f;
	[HideInInspector]
	public float scatteringStrenght = 0.5f;
	[HideInInspector]
	public float sunBlocking = 0.5f;
}

[Serializable]
public class EnviroLightshafts 
{
	[Tooltip("Use light shafts?")]
	public bool sunLightShafts = true;
	public bool moonLightShafts = true;
}

[Serializable]
public class EnviroCloudsLayer 
{
	[HideInInspector]
	public GameObject myObj;
	[HideInInspector]
	public Material myMaterial;
	[HideInInspector]
	public Material myShadowMaterial;
	[HideInInspector]
	public float DirectLightIntensity = 10f;
	[HideInInspector][Tooltip("Base color of clouds.")]
	public Color FirstColor = Color.white;
	[HideInInspector][Tooltip("Coverage rate of clouds generated.")]
	public float Coverage = 0f; // 
	[HideInInspector][Tooltip("Density of clouds generated.")]
	public float Density = 0f; 
	[HideInInspector][Tooltip("Clouds alpha modificator.")]
	public float Alpha = 0f;
}
	

[ExecuteInEditMode]
public class EnviroSky : MonoBehaviour
{
	private static EnviroSky _instance; // Creat a static instance for easy access!

	public static EnviroSky instance
	{
		get
		{
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<EnviroSky>();
			return _instance;
		}
	}

	[HideInInspector]public string prefabVersion = "1.9.0";

	[Tooltip("Assign your player gameObject here. Required Field! or enable AssignInRuntime!")]
	public GameObject Player;
	[Tooltip("Assign your main camera here. Required Field! or enable AssignInRuntime!")]
	public Camera PlayerCamera;
	[Tooltip("If enabled Enviro will search for your Player and Camera by Tag!")]
	public bool AssignInRuntime;
	[Tooltip("Your Player Tag")]
	public string PlayerTag = "";
	[Tooltip("Your CameraTag")]
	public string CameraTag = "MainCamera";
	[Header("Camera Settings")]
	[Tooltip("Enable HDR Rendering. You want to use a third party tonemapping effect for best results!")]
	public bool HDR = true;
	[Header("Layer Setup")]
	[Tooltip("This is the layer id for your clouds and satellites to get projected directly into skybox.!")]
	public int skyRenderingLayer = 30;
	[Tooltip("This is the layer id for all satellites like moons, planets.")]
	public int satelliteRenderingLayer = 31;
	[Tooltip("Activate to set recommended maincamera clear flag.")]
	public bool setCameraClearFlags = true;
	[Header("Virtual Reality")]
	[Tooltip("Enable this when using singlepass rendering.")]
	public bool singlePassVR = false;

	[Header("Profile")]
	public EnviroProfile profile = null;
	// Parameters
	[Header("Control")]
	public EnviroTime GameTime = null;
	public EnviroAudio Audio = null;
	public EnviroWeather Weather = null;
	public EnviroSeasons Seasons = null;
	public EnviroFogging Fog = null;
	public EnviroLightshafts LightShafts = null;

	[Header("Components")]
	public EnviroComponents Components = null;

	//Runtime Settings
	[HideInInspector]public bool started;
	[HideInInspector]public bool isNight = true;
	// Runtime profile
	[HideInInspector]public EnviroLightSettings lightSettings = new EnviroLightSettings();
	[HideInInspector]public EnviroSkySettings skySettings = new EnviroSkySettings();
	[HideInInspector]public EnviroCloudSettings cloudsSettings = new EnviroCloudSettings();
	[HideInInspector]public EnviroWeatherSettings weatherSettings = new EnviroWeatherSettings();
	[HideInInspector]public EnviroFogSettings fogSettings = new EnviroFogSettings();
	[HideInInspector]public EnviroLightShaftsSettings lightshaftsSettings = new EnviroLightShaftsSettings();
	[HideInInspector]public EnviroSeasonSettings seasonsSettings = new EnviroSeasonSettings();
	[HideInInspector]public EnviroAudioSettings audioSettings = new EnviroAudioSettings();
	[HideInInspector]public EnviroSatellitesSettings satelliteSettings = new EnviroSatellitesSettings();
	[HideInInspector]public BackgroundRenderingSettings backgroundSettings = new BackgroundRenderingSettings();
	[HideInInspector]public EnviroQualitySettings qualitySettings = new EnviroQualitySettings();
	// Camera Components
	[HideInInspector]public Camera cloudsCamera;
	[HideInInspector]public Camera skyCamera;
	[HideInInspector]public Camera bgCamera;
	[HideInInspector]public GameObject renderCameraHolder;
	[HideInInspector]public EnviroFog atmosphericFog;
	[HideInInspector]public EnviroLightShafts lightShaftsScriptSun;
	[HideInInspector]public EnviroLightShafts lightShaftsScriptMoon;
	[HideInInspector]public EnviroSkyRendering EnviroCloudsRender;
	// Weather SFX
	[HideInInspector]public GameObject EffectsHolder;
	[HideInInspector]public EnviroAudioSource AudioSourceWeather;
	[HideInInspector]public EnviroAudioSource AudioSourceWeather2;
	[HideInInspector]public EnviroAudioSource AudioSourceAmbient;
	[HideInInspector]public EnviroAudioSource AudioSourceAmbient2;
	[HideInInspector]public AudioSource AudioSourceThunder;
	// Vegeation Growth
	[HideInInspector]public List<EnviroVegetationInstance> EnviroVegetationInstances = new List<EnviroVegetationInstance>(); // All EnviroInstance that getting updated at the moment.
	//Sky runtime
	[HideInInspector]public Color currentWeatherSkyMod;
	[HideInInspector]public Color currentWeatherLightMod;
	[HideInInspector]public Color currentWeatherFogMod;
	//clouds runtime
	[HideInInspector]public List<EnviroCloudsLayer> cloudsLayers = new List<EnviroCloudsLayer>();
	[HideInInspector]public float thunder = 1f;
	// Satellites
	[HideInInspector]public List<GameObject> satellites = new List<GameObject>();
	[HideInInspector]public List<GameObject> satellitesRotation = new List<GameObject>();
	// Used from other Enviro componets
	[HideInInspector]public DateTime dateTime;
	[HideInInspector]public float internalHour;
	[HideInInspector]public float currentHour;
	[HideInInspector]public float currentDay;
	[HideInInspector]public float currentYear;
	[HideInInspector]public float currentTimeInHours;
	// Render Textures
	[HideInInspector]public RenderTexture cloudsRenderTarget;
	[HideInInspector]public RenderTexture satRenderTarget;
	[HideInInspector]public RenderTexture bgRenderTex;
	// Moon Phase
	[HideInInspector]public float customMoonPhase = 0.0f;

	//private
	[HideInInspector]public bool profileLoaded = false;
	private Transform DomeTransform;
	private Material VolumeCloudShader1;
	private Material VolumeCloudShadowShader1;    
	private Material VolumeCloudShader2;
	private Material VolumeCloudShadowShader2;
	private Transform SunTransform;
	private Light MainLight;
	private Transform MoonTransform;
	private Renderer MoonRenderer;
	private Material MoonShader;
	private float lastHourUpdate;
	private float starsRot;
	private float lastHour;
	private float lastRelfectionUpdate;
	private float OrbitRadius
	{
		get { return DomeTransform.localScale.x; }
	}
	private bool serverMode = false;

	// Scattering constants
	const float pi = Mathf.PI;
	private Vector3 K =  new Vector3(686.0f, 678.0f, 666.0f);
	private const float n =  1.0003f;   
	private const float N =  2.545E25f;
	private const float pn =  0.035f;    
	private Vector2 cloudAnim;
	private float hourTime;
	private float E0 = 0f;
	private float E1 = 0f;
	private float LST;


	//menu
	[HideInInspector]public bool showSettings = false;
	[HideInInspector]public List<bool> showCloudLayer = new List<bool>();

	// Events
	public delegate void HourPassed();
	public delegate void DayPassed();
	public delegate void YearPassed();
	public delegate void WeatherChanged(EnviroWeatherPreset weatherType);
	public delegate void ZoneWeatherChanged(EnviroWeatherPreset weatherType,EnviroZone zone);
	public delegate void SeasonChanged(EnviroSeasons.Seasons season);
	public delegate void isNightE();
	public delegate void isDay();
	public delegate void ZoneChanged(EnviroZone zone);
	public event HourPassed OnHourPassed;
	public event DayPassed OnDayPassed;
	public event YearPassed OnYearPassed;
	public event WeatherChanged OnWeatherChanged;
	public event ZoneWeatherChanged OnZoneWeatherChanged;
	public event SeasonChanged OnSeasonChanged;
	public event isNightE OnNightTime;
	public event isDay OnDayTime;
	public event ZoneChanged OnZoneChanged;
	///

	// Events:
	public virtual void NotifyHourPassed()
	{
		if(OnHourPassed != null)
			OnHourPassed();
	}
	public virtual void NotifyDayPassed()
	{
		if(OnDayPassed != null)
			OnDayPassed();
	}
	public virtual void NotifyYearPassed()
	{
		if(OnYearPassed != null)
			OnYearPassed();
	}
	public virtual void NotifyWeatherChanged(EnviroWeatherPreset type)
	{
		if(OnWeatherChanged != null)
			OnWeatherChanged (type);
	}
	public virtual void NotifyZoneWeatherChanged(EnviroWeatherPreset type, EnviroZone zone)
	{
		if(OnZoneWeatherChanged != null)
			OnZoneWeatherChanged (type,zone);
	}
	public virtual void NotifySeasonChanged(EnviroSeasons.Seasons season)
	{
		if(OnSeasonChanged != null)
			OnSeasonChanged (season);
	}
	public virtual void NotifyIsNight()
	{
		if(OnNightTime != null)
			OnNightTime ();
	}
	public virtual void NotifyIsDay()
	{
		if(OnDayTime != null)
			OnDayTime ();
	}
	public virtual void NotifyZoneChanged(EnviroZone zone)
	{
		if(OnZoneChanged != null)
			OnZoneChanged (zone);
	}

	void Start()
	{
		started = false;
		//Time
		SetTime (GameTime.Years, GameTime.Days, GameTime.Hours, GameTime.Minutes, GameTime.Seconds);
		lastHourUpdate = Mathf.RoundToInt(internalHour);
		currentTimeInHours = GetInHours (internalHour, GameTime.Days, GameTime.Years);
		// Check for Profile
		if (profileLoaded) {
			InvokeRepeating ("UpdateEnviroment", 0, qualitySettings.UpdateInterval);// Vegetation Updates
			CreateEffects ();  //Create Weather Effects Holder
			if (PlayerCamera != null && Player != null && AssignInRuntime == false && profile != null) {
				Init ();
			}
		}
	}

	void OnEnable()
	{
		DomeTransform = transform;

		//Set Weather
		Weather.currentActiveWeatherPreset = Weather.zones[0].currentActiveZoneWeatherPreset;
		Weather.lastActiveWeatherPreset = Weather.currentActiveWeatherPreset;

		if (profile == null) {
			Debug.LogError ("No profile assigned!");
			return;
		}

		// Auto Load profile
		if (profileLoaded == false)
			ApplyProfile (profile);

		PreInit ();

		if (AssignInRuntime) {
			started = false;	//Wait for assignment
		} else if (PlayerCamera != null && Player != null){
			Init ();
		}

		showCloudLayer.Clear ();
		for (int i = 0; i < cloudsSettings.cloudsLayers.Count; i++) {
			showCloudLayer.Add (false);
		}
	}

	/// <summary>
	/// Loads a profile into system.
	/// </summary>
	public void ApplyProfile(EnviroProfile p)
	{
		profile = p;
		lightSettings = JsonUtility.FromJson<EnviroLightSettings> (JsonUtility.ToJson(p.lightSettings));
		skySettings = JsonUtility.FromJson<EnviroSkySettings> (JsonUtility.ToJson(p.skySettings));
		cloudsSettings = JsonUtility.FromJson<EnviroCloudSettings> (JsonUtility.ToJson(p.cloudsSettings));
		weatherSettings = JsonUtility.FromJson<EnviroWeatherSettings> (JsonUtility.ToJson(p.weatherSettings));
		fogSettings = JsonUtility.FromJson<EnviroFogSettings> (JsonUtility.ToJson(p.fogSettings));
		lightshaftsSettings = JsonUtility.FromJson<EnviroLightShaftsSettings> (JsonUtility.ToJson(p.lightshaftsSettings));
		audioSettings = JsonUtility.FromJson<EnviroAudioSettings> (JsonUtility.ToJson(p.audioSettings));
		satelliteSettings = JsonUtility.FromJson<EnviroSatellitesSettings> (JsonUtility.ToJson(p.satelliteSettings));
		backgroundSettings = JsonUtility.FromJson<BackgroundRenderingSettings> (JsonUtility.ToJson(p.backgroundSettings));
		qualitySettings = JsonUtility.FromJson<EnviroQualitySettings> (JsonUtility.ToJson(p.qualitySettings));
		seasonsSettings = JsonUtility.FromJson<EnviroSeasonSettings> (JsonUtility.ToJson(p.seasonsSettings));
		profileLoaded = true;
	}

	/// <summary>
	/// Saves current settings in assigned profile.
	/// </summary>
	public void SaveProfile()
	{
		profile.lightSettings = JsonUtility.FromJson<EnviroLightSettings> (JsonUtility.ToJson(lightSettings));
		profile.skySettings = JsonUtility.FromJson<EnviroSkySettings> (JsonUtility.ToJson(skySettings));
		profile.cloudsSettings = JsonUtility.FromJson<EnviroCloudSettings> (JsonUtility.ToJson(cloudsSettings));
		profile.weatherSettings = JsonUtility.FromJson<EnviroWeatherSettings> (JsonUtility.ToJson(weatherSettings));
		profile.fogSettings = JsonUtility.FromJson<EnviroFogSettings> (JsonUtility.ToJson(fogSettings));
		profile.lightshaftsSettings = JsonUtility.FromJson<EnviroLightShaftsSettings> (JsonUtility.ToJson(lightshaftsSettings));
		profile.audioSettings = JsonUtility.FromJson<EnviroAudioSettings> (JsonUtility.ToJson(audioSettings));
		profile.satelliteSettings = JsonUtility.FromJson<EnviroSatellitesSettings> (JsonUtility.ToJson(satelliteSettings));
		profile.backgroundSettings = JsonUtility.FromJson<BackgroundRenderingSettings> (JsonUtility.ToJson(backgroundSettings));
		profile.qualitySettings = JsonUtility.FromJson<EnviroQualitySettings> (JsonUtility.ToJson(qualitySettings));
		profile.seasonsSettings = JsonUtility.FromJson<EnviroSeasonSettings> (JsonUtility.ToJson(seasonsSettings));
	}

	/// <summary>
	/// Re-Initilize the system.
	/// </summary>
	public void ReInit ()
	{
		OnEnable ();
	}

	/// <summary>
	/// Pee-Initilize the system.
	/// </summary>
	private void PreInit ()
	{
		// Check time
		if (GameTime.solarTime < 0.5f)
			isNight = true;
		else
			isNight = false;

		//return when in server mode!
		if (serverMode)
			return;

		CheckSatellites ();
	
		// Setup Fog Mode
		RenderSettings.fogMode = fogSettings.Fogmode;

		// Setup Skybox Material
		if (skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Default) {
			Material sky = new Material (Shader.Find ("Enviro/Skybox"));
			sky.SetTexture ("_Stars", skySettings.starsCubeMap);
			RenderSettings.skybox = sky;
		} else if (skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.CustomSkybox) {
			if(skySettings.customSkyboxMaterial != null)
				RenderSettings.skybox = skySettings.customSkyboxMaterial;
		}

		// Set ambient mode
		RenderSettings.ambientMode = lightSettings.ambientMode;
		RenderSettings.fogDensity = 0f;

		// Setup ReflectionProbe
		Components.GlobalReflectionProbe.size = transform.localScale;
		Components.GlobalReflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;

		if (Components.Sun) { 
			SunTransform = Components.Sun.transform; } 
		else { Debug.LogError("Please set Sun object in inspector!"); }

		if (Components.Moon){
			MoonTransform = Components.Moon.transform;
			MoonRenderer = Components.Moon.GetComponent<Renderer>();

			if (MoonRenderer == null)
				MoonRenderer = Components.Moon.AddComponent<MeshRenderer> ();

			MoonRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			MoonRenderer.receiveShadows = false;

			if (MoonRenderer.sharedMaterial != null)
				DestroyImmediate (MoonRenderer.sharedMaterial);

			if(skySettings.moonPhaseMode == EnviroSkySettings.MoonPhases.Realistic)
				MoonShader = new Material (Shader.Find ("Enviro/MoonShader"));
			else
				MoonShader = new Material (Shader.Find ("Enviro/MoonShaderPhased"));

			MoonShader.SetTexture ("_MainTex", skySettings.moonTexture);

			MoonRenderer.sharedMaterial = MoonShader;
			// Set start moon phase
			customMoonPhase = skySettings.startMoonPhase;
		}
		else { Debug.LogError("Please set moon object in inspector!"); }

		if (Components.DirectLight) { MainLight = Components.DirectLight.GetComponent<Light>(); 
		} else { Debug.LogError ("Please set direct light object in inspector!"); }
	}


	/// <summary>
	/// Final Initilization and startup.
	/// </summary>
	private void Init ()
	{
		if (profile == null)
			return;

		InitImageEffects ();

		// Setup Camera
		if (PlayerCamera != null) 
		{

			if (setCameraClearFlags) {
				// Workaround for deffered. Still looking for proper fix
				if (UnityEngine.VR.VRSettings.enabled && singlePassVR && PlayerCamera.actualRenderingPath == RenderingPath.DeferredShading) {
					#if UNITY_5_6_OR_NEWER
				PlayerCamera.clearFlags = CameraClearFlags.Depth;
					#else
					PlayerCamera.clearFlags = CameraClearFlags.SolidColor;
					PlayerCamera.backgroundColor = Color.black;
					#endif
				} else {
					PlayerCamera.clearFlags = CameraClearFlags.SolidColor;
					PlayerCamera.backgroundColor = Color.black;
				}
			}
			// Workaround for deffered. Still looking for proper fix
			if (PlayerCamera.actualRenderingPath == RenderingPath.DeferredShading)
				PlayerCamera.hdr = true;
			else
				PlayerCamera.hdr = HDR;
			
			Components.GlobalReflectionProbe.farClipPlane = PlayerCamera.farClipPlane;
		}

		CreateCameraHolder ();
		InitSatCamera ();
		InitClouds ();

		started = true;
	}

	private void InitImageEffects ()
	{
		atmosphericFog = PlayerCamera.gameObject.GetComponent<EnviroFog> ();

		if (atmosphericFog != null) 
		{
			DestroyImmediate (atmosphericFog.fogMaterial);
			atmosphericFog.fogMaterial = new Material (Shader.Find ("Enviro/Fog"));
			atmosphericFog.fogShader = atmosphericFog.fogMaterial.shader;
		}
		else
		{
			atmosphericFog = PlayerCamera.gameObject.AddComponent<EnviroFog> ();
			atmosphericFog.fogMaterial = new Material (Shader.Find ("Enviro/Fog"));
			atmosphericFog.fogShader = atmosphericFog.fogMaterial.shader;
		}

		EnviroLightShafts[] shaftScripts = PlayerCamera.gameObject.GetComponents<EnviroLightShafts>();

		if(shaftScripts.Length > 0)
			lightShaftsScriptSun = shaftScripts [0];

		if (lightShaftsScriptSun != null) 
		{
			DestroyImmediate (lightShaftsScriptSun.sunShaftsMaterial);
			DestroyImmediate (lightShaftsScriptSun.simpleClearMaterial);
			lightShaftsScriptSun.sunShaftsMaterial = new Material (Shader.Find ("Enviro/Effects/LightShafts"));
			lightShaftsScriptSun.sunShaftsShader = lightShaftsScriptSun.sunShaftsMaterial.shader;
			lightShaftsScriptSun.simpleClearMaterial = new Material (Shader.Find ("Enviro/Effects/ClearLightShafts"));
			lightShaftsScriptSun.simpleClearShader = lightShaftsScriptSun.simpleClearMaterial.shader;
		}
		else
		{
			lightShaftsScriptSun = PlayerCamera.gameObject.AddComponent<EnviroLightShafts> ();
			lightShaftsScriptSun.sunShaftsMaterial = new Material (Shader.Find ("Enviro/Effects/LightShafts"));
			lightShaftsScriptSun.sunShaftsShader = lightShaftsScriptSun.sunShaftsMaterial.shader;
			lightShaftsScriptSun.simpleClearMaterial = new Material (Shader.Find ("Enviro/Effects/ClearLightShafts"));
			lightShaftsScriptSun.simpleClearShader = lightShaftsScriptSun.simpleClearMaterial.shader;
		}

		if(shaftScripts.Length > 1)
			lightShaftsScriptMoon = shaftScripts [1];

		if (lightShaftsScriptMoon != null) 
		{
			DestroyImmediate (lightShaftsScriptMoon.sunShaftsMaterial);
			DestroyImmediate (lightShaftsScriptMoon.simpleClearMaterial);
			lightShaftsScriptMoon.sunShaftsMaterial = new Material (Shader.Find ("Enviro/Effects/LightShafts"));
			lightShaftsScriptMoon.sunShaftsShader = lightShaftsScriptMoon.sunShaftsMaterial.shader;
			lightShaftsScriptMoon.simpleClearMaterial = new Material (Shader.Find ("Enviro/Effects/ClearLightShafts"));
			lightShaftsScriptMoon.simpleClearShader = lightShaftsScriptMoon.simpleClearMaterial.shader;
		}
		else
		{
			lightShaftsScriptMoon = PlayerCamera.gameObject.AddComponent<EnviroLightShafts> ();
			lightShaftsScriptMoon.sunShaftsMaterial = new Material (Shader.Find ("Enviro/Effects/LightShafts"));
			lightShaftsScriptMoon.sunShaftsShader = lightShaftsScriptMoon.sunShaftsMaterial.shader;
			lightShaftsScriptMoon.simpleClearMaterial = new Material (Shader.Find ("Enviro/Effects/ClearLightShafts"));
			lightShaftsScriptMoon.simpleClearShader = lightShaftsScriptMoon.simpleClearMaterial.shader;
		}
	}
		

	private void CreateCameraHolder ()
	{
		//Destroy old rendering component!
		DestroyImmediate(GameObject.Find ("Enviro Rendering"));

		if (renderCameraHolder == null) {
			
			// Create new Holder
			GameObject obj = new GameObject ();
			obj.name = "Enviro Cameras";

			renderCameraHolder = obj;
		}
	}


	/// <summary>
	/// Recreates applies all settings.////
	/// </summary>
	public void InitClouds ()
	{
		int childs = Components.Clouds.transform.childCount;
		for (int i = childs - 1; i >= 0; i--)
		{
			GameObject.DestroyImmediate(Components.Clouds.transform.GetChild(i).gameObject);
		}

		cloudsLayers = new List<EnviroCloudsLayer> ();

		for (int i = 0; i <  cloudsSettings.cloudsLayers.Count; i++) 
		{
			// Create Cloud layer Object, Material, Renderer and Mesh
			GameObject layer = new GameObject();
			layer.name = "Clouds Layer: " + i.ToString ();
			layer.transform.SetParent (Components.Clouds.transform);
			layer.transform.localEulerAngles = new Vector3(0f,0f,-180f);
			layer.transform.localScale = new Vector3(1f *  cloudsSettings.worldScale, 1f,1f *  cloudsSettings.worldScale);
			layer.transform.localPosition = new Vector3 (0f,  cloudsSettings.cloudsLayers [i].layerAltitude, 0f);
			layer.layer = skyRenderingLayer;
			EnviroCloudsLayer newLayer = new EnviroCloudsLayer();
			newLayer.myObj = layer;
			MeshFilter layerMeshFilter = layer.AddComponent<MeshFilter> ();
			MeshRenderer layerMeshRenderer = layer.AddComponent<MeshRenderer> ();
			newLayer.myMaterial = new Material (Shader.Find ("Enviro/Clouds"));
			layerMeshRenderer.sharedMaterial = newLayer.myMaterial;
			layerMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			layerMeshFilter.sharedMesh = CreateCloudsLayer (i, false);
			newLayer.myMaterial.SetTexture ("_CloudsMap", cloudsSettings.cloudsLayers[i].myCloudsTexture);
			layerMeshRenderer.sharedMaterial.renderQueue += cloudsSettings.cloudsLayers.Count - i;
			// Create CloudShadows layer Object, Material, Renderer and Mesh
			if (cloudsSettings.cloudsLayers[i].canCastShadows) {
				GameObject layerShadows = new GameObject ();
				layerShadows.name = "CloudsShadows Layer: " + i.ToString ();
				layerShadows.transform.SetParent (Components.Clouds.transform);
				layerShadows.transform.localEulerAngles = new Vector3 (0f, 0f, -180f);
				layerShadows.transform.localScale = new Vector3(1f * cloudsSettings.worldScale, 1f,1f * cloudsSettings.worldScale);
				layerShadows.transform.localPosition = new Vector3 (0f, cloudsSettings.cloudsLayers [i].layerAltitude, 0f);
				MeshFilter layerShadowsMeshFilter = layerShadows.AddComponent<MeshFilter> ();
				MeshRenderer layerShadowsMeshRenderer = layerShadows.AddComponent<MeshRenderer> ();
				layerShadowsMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
				newLayer.myShadowMaterial = new Material (Shader.Find ("Enviro/CloudsShadows"));
				layerShadowsMeshRenderer.sharedMaterial = newLayer.myShadowMaterial;
				layerShadowsMeshFilter.sharedMesh = CreateCloudsLayer (i, true);
				newLayer.myShadowMaterial.SetTexture ("_CloudsMap", cloudsSettings.cloudsLayers [i].myCloudsTexture);
			}
			// add to layer list to drive
			cloudsLayers.Add (newLayer);
		}
			
			// Search all cams and remove sky layer from cullingmask!
			Camera[] cams = GameObject.FindObjectsOfType<Camera> ();
			for (int i = 0; i < cams.Length; i++) 
			{
				cams[i].cullingMask &= ~(1 << skyRenderingLayer);
			}

			DestroyImmediate(GameObject.Find ("Enviro Clouds Camera"));

			GameObject camObj = new GameObject ();	

			camObj.name = "Enviro Clouds Camera";
			camObj.transform.SetParent (renderCameraHolder.transform);
			camObj.transform.localPosition = Vector3.zero;
			camObj.transform.localRotation = Quaternion.identity;
			cloudsCamera = camObj.AddComponent<Camera> ();
			cloudsCamera.farClipPlane = PlayerCamera.farClipPlane * cloudsSettings.worldScale;
			cloudsCamera.nearClipPlane = PlayerCamera.nearClipPlane;
			cloudsCamera.aspect = PlayerCamera.aspect;
			cloudsCamera.hdr = HDR;
			cloudsCamera.useOcclusionCulling = false;
			cloudsCamera.renderingPath = RenderingPath.Forward;
			cloudsCamera.fieldOfView = PlayerCamera.fieldOfView;
			if (skySettings.skyboxMode != EnviroSkySettings.SkyboxModi.CustomColor)
				cloudsCamera.clearFlags = CameraClearFlags.Skybox;
			else {
				cloudsCamera.clearFlags = CameraClearFlags.SolidColor;
				cloudsCamera.backgroundColor = skySettings.customSkyboxColor;
			}
			cloudsCamera.cullingMask = (1 << skyRenderingLayer);
			cloudsCamera.stereoTargetEye = StereoTargetEyeMask.Both;
			cloudsCamera.enabled = false;


		// Create clouds Render Texture
	
			var format = cloudsCamera.hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
			cloudsRenderTarget = new RenderTexture (Screen.currentResolution.width / cloudsSettings.cloudsRenderResolution, Screen.currentResolution.height / cloudsSettings.cloudsRenderResolution, 16, format);
			cloudsCamera.targetTexture = cloudsRenderTarget;
		

		if(backgroundSettings.backgroundRendering)
			InitBGCamera ();

		if (PlayerCamera != null)
			AddEnviroCloudsRendering ();
	}



	/// <summary>
	/// Creates Component for sky finalization.
	/// </summary>
	private void AddEnviroCloudsRendering ()
	{
		EnviroCloudsRender = PlayerCamera.gameObject.GetComponent<EnviroSkyRendering> ();

		if(EnviroCloudsRender == null)
			EnviroCloudsRender = PlayerCamera.gameObject.AddComponent<EnviroSkyRendering> ();

		Material mat = new Material (Shader.Find ("Enviro/SkyRendering"));


		if(backgroundSettings.backgroundRendering && bgCamera != null)
			mat.SetTexture("_Background",bgCamera.targetTexture);

		EnviroCloudsRender.material = mat;
		/// Apply camera command on player camera
		EnviroCloudsRender.Apply ();
	}

	/// <summary>
	/// Re-create the camera and render texture for satellite rendering
	/// </summary>
	public void InitSatCamera ()
	{
		Camera[] cams = GameObject.FindObjectsOfType<Camera> ();
		for (int i = 0; i < cams.Length; i++) 
		{
			cams[i].cullingMask &= ~(1 << satelliteRenderingLayer);
		}

		DestroyImmediate(GameObject.Find ("Enviro Sky Camera"));

		GameObject camObj = new GameObject ();	

		camObj.name = "Enviro Sky Camera";
		camObj.transform.SetParent (renderCameraHolder.transform);
		camObj.transform.localPosition = Vector3.zero;
		camObj.transform.localRotation = Quaternion.identity;

		skyCamera = camObj.AddComponent<Camera> ();
		skyCamera.farClipPlane = PlayerCamera.farClipPlane;
		skyCamera.nearClipPlane = PlayerCamera.nearClipPlane;
		skyCamera.aspect = PlayerCamera.aspect;
		skyCamera.hdr = HDR;
		skyCamera.useOcclusionCulling = false;
		skyCamera.renderingPath = RenderingPath.Forward;
		skyCamera.fieldOfView = PlayerCamera.fieldOfView;
		skyCamera.clearFlags = CameraClearFlags.Skybox;
		skyCamera.cullingMask = (1 << satelliteRenderingLayer);
		skyCamera.depth = PlayerCamera.depth + 1;
		skyCamera.enabled = true;
		PlayerCamera.cullingMask &= ~(1 << satelliteRenderingLayer);
		//camObj.AddComponent <EnviroCamera>();
		Components.Moon.layer = satelliteRenderingLayer;

		var format = skyCamera.hdr ? RenderTextureFormat.DefaultHDR: RenderTextureFormat.Default;

		satRenderTarget = new RenderTexture (Screen.currentResolution.width, Screen.currentResolution.height,16,format);
		skyCamera.targetTexture = satRenderTarget;
		skyCamera.enabled = false;
	}

	/// <summary>
	/// Re-create the camera and render texture for background rendering
	/// </summary>
	public void InitBGCamera ()
	{
		Camera[] cams = GameObject.FindObjectsOfType<Camera> ();
		for (int i = 0; i < cams.Length; i++) 
		{
			cams[i].cullingMask &= ~(1 << backgroundSettings.backgroundLayer);
		}

		DestroyImmediate(GameObject.Find ("Enviro Background Camera"));

		GameObject camObj = new GameObject ();	

		camObj.name = "Enviro Background Camera";
		camObj.transform.SetParent (renderCameraHolder.transform);
		camObj.transform.localPosition = Vector3.zero;
		camObj.transform.localRotation = Quaternion.identity;
		bgCamera = camObj.AddComponent<Camera> ();
		bgCamera.farClipPlane = backgroundSettings.backgroundViewDistance;
		bgCamera.nearClipPlane = PlayerCamera.nearClipPlane;
		bgCamera.aspect = PlayerCamera.aspect;
		bgCamera.hdr = HDR;
		bgCamera.renderingPath = RenderingPath.Forward;
		bgCamera.fieldOfView = PlayerCamera.fieldOfView;
		bgCamera.clearFlags = CameraClearFlags.Skybox;
		bgCamera.cullingMask = (1 << backgroundSettings.backgroundLayer);
		bgCamera.depth = PlayerCamera.depth + 1;
		bgCamera.enabled = true;
		PlayerCamera.cullingMask &= ~(1 << backgroundSettings.backgroundLayer);

		var format = bgCamera.hdr ? RenderTextureFormat.DefaultHDR: RenderTextureFormat.Default;

		bgRenderTex = new RenderTexture (Screen.currentResolution.width, Screen.currentResolution.height,16,format);
		bgCamera.targetTexture = bgRenderTex;
		bgCamera.enabled = false;
	}

	/// <summary>
	/// Create Effect Holder Gmaeobjec and adds audiofeatures
	/// </summary>
	public void CreateEffects ()
	{
			GameObject old = GameObject.Find ("Enviro Effects");

			if (old != null)
				DestroyImmediate (old);

			EffectsHolder = new GameObject ();
			EffectsHolder.name = "Enviro Effects";
			if(Player != null)
				EffectsHolder.transform.position = Player.transform.position;
			else
				EffectsHolder.transform.position = EnviroSky.instance.transform.position;


			CreateWeatherEffectHolder ();

			GameObject SFX = (GameObject)Instantiate (Audio.SFXHolderPrefab, Vector3.zero, Quaternion.identity);

			SFX.transform.parent = EffectsHolder.transform;

			EnviroAudioSource[] srcs = SFX.GetComponentsInChildren<EnviroAudioSource> ();

			for (int i = 0; i < srcs.Length; i++) 
			{
				switch (srcs [i].myFunction) {
				case EnviroAudioSource.AudioSourceFunction.Weather1:
					AudioSourceWeather = srcs [i];
					break;
				case EnviroAudioSource.AudioSourceFunction.Weather2:
					AudioSourceWeather2 = srcs [i];
					break;
				case EnviroAudioSource.AudioSourceFunction.Ambient:
					AudioSourceAmbient = srcs [i];
					break;
				case EnviroAudioSource.AudioSourceFunction.Ambient2:
					AudioSourceAmbient2 = srcs [i];
					break;
				case EnviroAudioSource.AudioSourceFunction.Thunder:
					AudioSourceThunder = srcs [i].audiosrc;
					break;
				}
			}
			
		Weather.currentAudioSource = AudioSourceWeather; 
		Audio.currentAmbientSource = AudioSourceAmbient;
		TryPlayAmbientSFX ();
	}

	/// <summary>
	/// Called internaly from growth objects
	/// </summary>
	/// <param name="season">Season.</param>
	public int RegisterMe (EnviroVegetationInstance me) 
	{
		EnviroVegetationInstances.Add (me);
		return EnviroVegetationInstances.Count - 1;
	}

	/// <summary>
	/// Manual change of Season
	/// </summary>
	/// <param name="season">Season.</param>
	public void ChangeSeason (EnviroSeasons.Seasons season)
	{
		Seasons.currentSeasons = season;
		NotifySeasonChanged (season);
	}

	// Update the Season according gameDays
	private void UpdateSeason ()
	{
		
		if (currentDay >= 0 && currentDay < seasonsSettings.SpringInDays)
		{
			Seasons.currentSeasons = EnviroSeasons.Seasons.Spring;

			if (Seasons.lastSeason != Seasons.currentSeasons)
				NotifySeasonChanged (EnviroSeasons.Seasons.Spring);

			Seasons.lastSeason = Seasons.currentSeasons;
		} 
		else if (currentDay >= seasonsSettings.SpringInDays && currentDay < (seasonsSettings.SpringInDays + seasonsSettings.SummerInDays))
		{
			Seasons.currentSeasons = EnviroSeasons.Seasons.Summer;

			if (Seasons.lastSeason != Seasons.currentSeasons)
				NotifySeasonChanged (EnviroSeasons.Seasons.Summer);

			Seasons.lastSeason = Seasons.currentSeasons;
		} 
		else if (currentDay >= (seasonsSettings.SpringInDays + seasonsSettings.SummerInDays) && currentDay < (seasonsSettings.SpringInDays + seasonsSettings.SummerInDays + seasonsSettings.AutumnInDays)) 
		{
			Seasons.currentSeasons = EnviroSeasons.Seasons.Autumn;

			if (Seasons.lastSeason != Seasons.currentSeasons)
				NotifySeasonChanged (EnviroSeasons.Seasons.Autumn);

			Seasons.lastSeason = Seasons.currentSeasons;
		}
		else if(currentDay >= (seasonsSettings.SpringInDays + seasonsSettings.SummerInDays + seasonsSettings.AutumnInDays) && currentDay <= (seasonsSettings.SpringInDays + seasonsSettings.SummerInDays + seasonsSettings.AutumnInDays + seasonsSettings.WinterInDays))
		{
			Seasons.currentSeasons = EnviroSeasons.Seasons.Winter;

			if (Seasons.lastSeason != Seasons.currentSeasons)
				NotifySeasonChanged (EnviroSeasons.Seasons.Winter);

			Seasons.lastSeason = Seasons.currentSeasons;
		}
	}

	private void PlayAmbient (AudioClip sfx)
	{
		if (sfx == Audio.currentAmbientSource.audiosrc.clip) {
			Audio.currentAmbientSource.FadeIn (sfx);
			return;
		}
			if (Audio.currentAmbientSource == AudioSourceAmbient){
				AudioSourceAmbient.FadeOut();
				AudioSourceAmbient2.FadeIn(sfx);
				Audio.currentAmbientSource = AudioSourceAmbient2;
			}
			else if (Audio.currentAmbientSource == AudioSourceAmbient2){
				AudioSourceAmbient2.FadeOut();
				AudioSourceAmbient.FadeIn(sfx);
				Audio.currentAmbientSource = AudioSourceAmbient;
			}
	}


	private void TryPlayAmbientSFX ()
	{
		if (Weather.currentActiveWeatherPreset == null)
			return;
		
		if (isNight) 
		{
			switch (Seasons.currentSeasons)
			{
			case EnviroSeasons.Seasons.Spring:
				if (Weather.currentActiveWeatherPreset.SpringNightAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherPreset.SpringNightAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
			break;

			case EnviroSeasons.Seasons.Summer:
				if (Weather.currentActiveWeatherPreset.SummerNightAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherPreset.SummerNightAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			case EnviroSeasons.Seasons.Autumn:
				if (Weather.currentActiveWeatherPreset.AutumnNightAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherPreset.AutumnNightAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			case EnviroSeasons.Seasons.Winter:
				if (Weather.currentActiveWeatherPreset.WinterNightAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherPreset.WinterNightAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			}
		} 
		else 
		{
			switch (Seasons.currentSeasons)
			{
			case EnviroSeasons.Seasons.Spring:
				if (Weather.currentActiveWeatherPreset.SpringDayAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherPreset.SpringDayAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			case EnviroSeasons.Seasons.Summer:
				if (Weather.currentActiveWeatherPreset.SummerDayAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherPreset.SummerDayAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			case EnviroSeasons.Seasons.Autumn:
				if (Weather.currentActiveWeatherPreset.AutumnDayAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherPreset.AutumnDayAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			case EnviroSeasons.Seasons.Winter:
				if (Weather.currentActiveWeatherPreset.WinterDayAmbient != null)
					PlayAmbient (Weather.currentActiveWeatherPreset.WinterDayAmbient);
				else {
					AudioSourceAmbient.FadeOut ();
					AudioSourceAmbient2.FadeOut ();
				}
				break;
			}
		}
	}

	private void UpdateEnviroment () // Update the all GrowthInstances
	{
		// Set correct Season.
		if(Seasons.calcSeasons)
			UpdateSeason ();

		// Update all EnviroGrowInstancesSeason in scene!
		if (EnviroVegetationInstances.Count > 0) 
		{
			for (int i = 0; i < EnviroVegetationInstances.Count; i++) {
				if (EnviroVegetationInstances [i] != null)
					EnviroVegetationInstances [i].UpdateInstance ();

			}
		}
	}

	/// <summary>
	/// Instantiates a new satellite
	/// </summary>
	/// <param name="id">Identifier.</param>
	private void CreateSatellite (int id)
	{
		if (satelliteSettings.additionalSatellites [id].prefab == null) {
			Debug.Log ("Satellite without prefab! Pleae assign a prefab to all satellites.");
			return;
		}
		GameObject satRot = new GameObject ();
		satRot.name = satelliteSettings.additionalSatellites [id].name;
		satRot.transform.parent = Components.satellites;
		satellitesRotation.Add (satRot);
		GameObject sat = (GameObject)Instantiate (satelliteSettings.additionalSatellites [id].prefab,satRot.transform);
		sat.layer = satelliteRenderingLayer;
		satellites.Add (sat);
	}

	/// <summary>
	/// Destroy and recreate all satellites
	/// </summary>
	public void CheckSatellites ()
	{
		satellites = new List<GameObject> ();

		int childs = Components.satellites.childCount;
		for (int i = childs-1; i >= 0; i--) 
		{
			DestroyImmediate (Components.satellites.GetChild (i).gameObject);
		}

		satellites.Clear ();
		satellitesRotation.Clear ();

		for (int i = 0; i < satelliteSettings.additionalSatellites.Count; i++) 
		{
			CreateSatellite (i);
		}
	}
		

	private void CalculateSatPositions (float siderealTime)
	{
		for (int i = 0; i < satelliteSettings.additionalSatellites.Count; i++)
		{
		Quaternion satRotation = Quaternion.Euler (90 - GameTime.Latitude, GameTime.Longitude, 0);
			satRotation *= Quaternion.Euler(satelliteSettings.additionalSatellites[i].yRot, siderealTime, satelliteSettings.additionalSatellites[i].xRot);
		
		if(satellites.Count >= i)
			satellites [i].transform.localPosition = new Vector3 (0f, satelliteSettings.additionalSatellites[i].orbit, 0f);
		if(satellitesRotation.Count >= i)
		    satellitesRotation[i].transform.localRotation = satRotation;
		}
	}


	private void UpdateCameraComponents()
	{
		//Update Fog
		if (atmosphericFog != null) 
		{
			atmosphericFog.distanceFog = fogSettings.distanceFog;
			atmosphericFog.heightFog = fogSettings.heightFog;
			atmosphericFog.height = fogSettings.height;
			atmosphericFog.heightDensity = fogSettings.heightDensity;
			atmosphericFog.useRadialDistance = fogSettings.useRadialDistance;
			atmosphericFog.startDistance = fogSettings.startDistance;

			if (Fog.AdvancedFog)
				atmosphericFog.enabled = true;
			else
				atmosphericFog.enabled = false;
		}

		//Update LightShafts
		if (lightShaftsScriptSun != null) 
		{
			lightShaftsScriptSun.resolution = lightshaftsSettings.resolution;
			lightShaftsScriptSun.screenBlendMode = lightshaftsSettings.screenBlendMode;
			lightShaftsScriptSun.useDepthTexture = lightshaftsSettings.useDepthTexture;
			lightShaftsScriptSun.sunThreshold = lightshaftsSettings.thresholdColorSun.Evaluate (GameTime.solarTime);

			lightShaftsScriptSun.sunShaftBlurRadius = lightshaftsSettings.blurRadius;
			lightShaftsScriptSun.sunShaftIntensity = lightshaftsSettings.intensity;
			lightShaftsScriptSun.maxRadius = lightshaftsSettings.maxRadius;
			lightShaftsScriptSun.sunColor = lightshaftsSettings.lightShaftsColorSun.Evaluate (GameTime.solarTime);
			lightShaftsScriptSun.sunTransform = Components.Sun.transform;

			if (LightShafts.sunLightShafts) {
				lightShaftsScriptSun.enabled = true;
			} else {
				lightShaftsScriptSun.enabled = false;
			}
		}

		if (lightShaftsScriptMoon != null) 
		{
			lightShaftsScriptMoon.resolution = lightshaftsSettings.resolution;
			lightShaftsScriptMoon.screenBlendMode = lightshaftsSettings.screenBlendMode;
			lightShaftsScriptMoon.useDepthTexture = lightshaftsSettings.useDepthTexture;
			lightShaftsScriptMoon.sunThreshold = lightshaftsSettings.thresholdColorMoon.Evaluate (GameTime.lunarTime);
   

			lightShaftsScriptMoon.sunShaftBlurRadius = lightshaftsSettings.blurRadius;
			lightShaftsScriptMoon.sunShaftIntensity = Mathf.Clamp ((lightshaftsSettings.intensity - GameTime.solarTime),0,100);
			lightShaftsScriptMoon.maxRadius = lightshaftsSettings.maxRadius;
			lightShaftsScriptMoon.sunColor = lightshaftsSettings.lightShaftsColorMoon.Evaluate (GameTime.lunarTime);
			lightShaftsScriptMoon.sunTransform = Components.Moon.transform;

			if (LightShafts.moonLightShafts) {
				lightShaftsScriptMoon.enabled = true;
			} else {
				lightShaftsScriptMoon.enabled = false;
			}
		}
	}

	private Vector3 CalculatePosition ()
	{
		Vector3 newPosition;
		newPosition.x = Player.transform.position.x;
		newPosition.z = Player.transform.position.z;
		newPosition.y = Player.transform.position.y;

		return newPosition;
	}



	void Update()
	{
		if (profile == null) {
			Debug.Log ("No profile applied! Please create and assign a profile.");
			return;
		}

		if (!started) 
		{
			if (AssignInRuntime && PlayerTag != "" && CameraTag != "" && Application.isPlaying) {
				Player = GameObject.FindGameObjectWithTag (PlayerTag);
				PlayerCamera = GameObject.FindGameObjectWithTag (CameraTag).GetComponent<Camera>();

				if (Player != null && PlayerCamera != null) {
					Init ();
					started = true;
				}
				else  {started = false; return;}
			} else {started = false; return;}
		}

		UpdateTime ();
		ValidateParameters();

		if (!serverMode) {
			UpdateCameraComponents ();
			UpdateAmbientLight ();
			UpdateReflections ();
			UpdateWeather ();
			CalculateSatPositions (LST);

			if (EffectsHolder != null)
				EffectsHolder.transform.position = Player.transform.position;

			if (Fog.AdvancedFog)
				UpdateAdvancedFog ();

			// Update sun and fog color according to the new position of the sun
			if (skySettings.sunAndMoonPosition == EnviroSkySettings.SunAndMoonCalc.Realistic)
				UpdateSunAndMoonPosition ();
			else
				UpdateSimpleSunAndMoonPosition ();

			CalculateDirectLight ();
	
			if (PlayerCamera != null) {
				// Set Clouds layers altitude
				if (cloudsSettings.FixedAltitude)
					Components.Clouds.transform.position = new Vector3(Components.Clouds.transform.position.x,cloudsSettings.cloudsAltitude,Components.Clouds.transform.position.z);
				else
					Components.Clouds.transform.localPosition = new Vector3(0f,0.2f,0f);

				transform.position = Player.transform.position;
				transform.localScale = new Vector3 (PlayerCamera.farClipPlane, PlayerCamera.farClipPlane, PlayerCamera.farClipPlane);
			}

			if (!isNight && GameTime.solarTime < 0.5f) {
				isNight = true;
				if (AudioSourceAmbient != null)
					TryPlayAmbientSFX ();
				NotifyIsNight ();
			} else if (isNight && GameTime.solarTime >= 0.5f) {
				isNight = false;
				if (AudioSourceAmbient != null)
					TryPlayAmbientSFX ();
				NotifyIsDay ();
			}
		} 
		else 
		{


		}
	
	}

	private Vector3 BetaRay() {
		Vector3 Br;

		Vector3 realWavelength = skySettings.waveLength * 1.0e-9f;

		Br.x = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)))*(6.0f+3.0f*pn) ) / ((3.0f * N * Mathf.Pow(realWavelength.x, 4.0f))*(6.0f-7.0f*pn) ))* 2000f;
		Br.y = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)))*(6.0f+3.0f*pn) ) / ((3.0f * N * Mathf.Pow(realWavelength.y, 4.0f))*(6.0f-7.0f*pn) ))* 2000f;
		Br.z = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)))*(6.0f+3.0f*pn) ) / ((3.0f * N * Mathf.Pow(realWavelength.z, 4.0f))*(6.0f-7.0f*pn) ))* 2000f;

		return Br;
	}


	private Vector3 BetaMie() {
		Vector3 Bm;

		float c = (0.2f * skySettings.turbidity ) * 10.0f;

		Bm.x = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / skySettings.waveLength.x, 2.0f) * K.x);
		Bm.y = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / skySettings.waveLength.y, 2.0f) * K.y);
		Bm.z = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / skySettings.waveLength.z, 2.0f) * K.z);

		Bm.x=Mathf.Pow(Bm.x,-1.0f);
		Bm.y=Mathf.Pow(Bm.y,-1.0f);
		Bm.z=Mathf.Pow(Bm.z,-1.0f);

		return Bm;
	}

	private Vector3 GetMieG() {
		return new Vector3(1.0f - skySettings.g * skySettings.g, 1.0f + skySettings.g * skySettings.g, 2.0f * skySettings.g);
	}

	// Setup the Shaders with correct information
	private void SetupShader(float setup)
	{
		RenderSettings.skybox.SetVector ("_SunDir", -SunTransform.transform.forward);
		RenderSettings.skybox.SetVector ("_MoonDir", -Components.Moon.transform.forward);
		RenderSettings.skybox.SetMatrix ("_Sun",  SunTransform.worldToLocalMatrix);
		RenderSettings.skybox.SetColor("_scatteringColor",skySettings.scatteringColor.Evaluate(GameTime.solarTime));
		RenderSettings.skybox.SetColor("_sunDiskColor", skySettings.sunDiskColor.Evaluate(GameTime.solarTime));
		RenderSettings.skybox.SetColor("_weatherSkyMod",currentWeatherSkyMod);
		RenderSettings.skybox.SetColor("_weatherFogMod",currentWeatherFogMod);
		RenderSettings.skybox.SetVector ("_Bm", BetaMie () * (skySettings.mie * Fog.scatteringStrenght));
		RenderSettings.skybox.SetVector ("_Br", BetaRay() * skySettings.rayleigh);
		RenderSettings.skybox.SetVector ("_mieG",GetMieG ());
		RenderSettings.skybox.SetFloat ("_SunIntensity",skySettings.sunIntensity);
		RenderSettings.skybox.SetFloat ("_SunDiskSize", skySettings.sunDiskScale);
		RenderSettings.skybox.SetFloat ("_SunDiskIntensity", skySettings.sunDiskIntensity);
		RenderSettings.skybox.SetFloat ("_SunDiskSize",skySettings.sunDiskScale);
		RenderSettings.skybox.SetFloat ("_Exposure", skySettings.skyExposure);
		RenderSettings.skybox.SetFloat ("_SkyLuminance", skySettings.skyLuminence.Evaluate(GameTime.solarTime));
		RenderSettings.skybox.SetFloat ("_scatteringPower", skySettings.scatteringCurve.Evaluate(GameTime.solarTime));
		RenderSettings.skybox.SetFloat ("_SkyColorPower", skySettings.skyColorPower.Evaluate(GameTime.solarTime));
		RenderSettings.skybox.SetFloat ("_StarsIntensity", skySettings.starsIntensity.Evaluate(GameTime.solarTime));
		float hdr = HDR ? 1f : 0f;
		RenderSettings.skybox.SetFloat ("_hdr", hdr);

		RenderSettings.skybox.SetFloat("_moonGlowStrenght", skySettings.moonGlow.Evaluate(GameTime.solarTime));

		// Update SkyFog settingss
		if(PlayerCamera != null)
		RenderSettings.skybox.SetVector ("_CameraWS", PlayerCamera.transform.position);

		//if (Sky.StarsBlinking > 0.0f)
		//{
		//	starsRot += Sky.StarsBlinking * Time.deltaTime;
		//	Quaternion rot = Quaternion.Euler (starsRot, starsRot, starsRot);
		//		Matrix4x4 NoiseRot = Matrix4x4.TRS (Vector3.zero, rot, new Vector3 (1, 1, 1));
		//		RenderSettings.skybox.SetMatrix ("_NoiseMatrix", NoiseRot);
		//}

		float windStrenght = 0;

		if (Weather.currentActiveWeatherPreset != null)
			windStrenght = Weather.currentActiveWeatherPreset.WindStrenght;

		if (cloudsSettings.useWindZoneDirection) {
			cloudsSettings.cloudsWindDirectionX = Components.windZone.transform.forward.x;
			cloudsSettings.cloudsWindDirectionY = Components.windZone.transform.forward.z;
		}
			
		cloudAnim += new Vector2(((cloudsSettings.cloudsTimeScale * (windStrenght * cloudsSettings.cloudsWindDirectionX)) * cloudsSettings.cloudsWindStrengthModificator) * Time.deltaTime,((cloudsSettings.cloudsTimeScale * (windStrenght * cloudsSettings.cloudsWindDirectionY)) * cloudsSettings.cloudsWindStrengthModificator) * Time.deltaTime);
			
		if (cloudAnim.x > 1f)
			cloudAnim.x = -1f;
		else if (cloudAnim.x < -1f)
			cloudAnim.x = 1f;

		if (cloudAnim.y > 1f)
			cloudAnim.y = -1f;
		else if (cloudAnim.y < -1f)
			cloudAnim.y = 1f;

		if (cloudAnim.x == 0)
			cloudAnim.x = 0.1f;
		if (cloudAnim.y == 0)
			cloudAnim.y = 0.1f;

		for (int i = 0; i < cloudsLayers.Count; i++) 
		{
			cloudsLayers[i].myMaterial.SetFloat("_Offset", cloudsSettings.cloudsLayers[i].LayerOffset);
			cloudsLayers[i].myMaterial.SetFloat("_Scale", PlayerCamera.farClipPlane * cloudsSettings.cloudsLayers[i].Scaling);
			cloudsLayers[i].myMaterial.SetColor("_BaseColor", cloudsLayers[i].FirstColor);
			cloudsLayers[i].myMaterial.SetColor("_SkyColor", cloudsSettings.skyColor.Evaluate(GameTime.solarTime));
			cloudsLayers[i].myMaterial.SetColor("_MoonColor", cloudsSettings.moonHighlightColor.Evaluate(GameTime.lunarTime));
			cloudsLayers[i].myMaterial.SetColor("_SunColor", cloudsSettings.sunHighlightColor.Evaluate(GameTime.solarTime));
			cloudsLayers[i].myMaterial.SetFloat("_CloudCover", cloudsLayers[i].Coverage);
			cloudsLayers[i].myMaterial.SetFloat("_Density", cloudsLayers[i].Density);
			cloudsLayers[i].myMaterial.SetFloat("_CloudAlpha", cloudsLayers[i].Alpha);
			cloudsLayers[i].myMaterial.SetVector("_timeScale", cloudAnim);
			cloudsLayers[i].myMaterial.SetFloat ("_lightIntensity", cloudsSettings.lightIntensity.Evaluate(GameTime.solarTime));
			cloudsLayers[i].myMaterial.SetFloat ("_direct", cloudsLayers[i].DirectLightIntensity);
			cloudsLayers[i].myMaterial.SetFloat ("_thunder", thunder);
			cloudsLayers[i].myMaterial.SetFloat ("_solarTime", GameTime.solarTime);
			float hdrClouds = HDR ? 1f : 0f;
			cloudsLayers[i].myMaterial.SetFloat ("_hdr", hdrClouds);

			cloudsLayers[i].myMaterial.SetVector ("_SunDirection", -Components.Sun.transform.forward);
			cloudsLayers[i].myMaterial.SetVector ("_MoonDirection", -Components.Moon.transform.forward);

			if (cloudsSettings.cloudsLayers [i].canCastShadows) {
				cloudsLayers [i].myShadowMaterial.SetFloat("_Offset", cloudsSettings.cloudsLayers[i].LayerOffset);
				cloudsLayers [i].myShadowMaterial.SetFloat ("_Scale", PlayerCamera.farClipPlane * cloudsSettings.cloudsLayers [i].Scaling);
				cloudsLayers [i].myShadowMaterial.SetFloat ("_CloudCover", cloudsLayers[i].Coverage);
				cloudsLayers [i].myShadowMaterial.SetFloat ("_CloudAlpha", cloudsLayers[i].Alpha);
				cloudsLayers [i].myShadowMaterial.SetVector("_timeScale", cloudAnim);
			}
		}

		if (MoonShader != null)
		{
			MoonShader.SetFloat("_Phase", customMoonPhase);
			MoonShader.SetFloat("_Brightness", skySettings.moonBrightness * (1-GameTime.solarTime));
		}
	}

	void UpdateAdvancedFog ()
	{
		if (atmosphericFog == null)
			return;

		if(cloudsCamera != null)
			atmosphericFog.fogMaterial.SetTexture ("_Clouds", cloudsCamera.targetTexture);

		if(backgroundSettings.backgroundRendering && bgCamera != null)
			atmosphericFog.fogMaterial.SetTexture ("_Background", bgCamera.targetTexture);
		
		atmosphericFog.fogMaterial.SetVector ("_SunDir", -SunTransform.transform.forward);
		atmosphericFog.fogMaterial.SetColor("_scatteringColor", skySettings.scatteringColor.Evaluate(GameTime.solarTime));
		atmosphericFog.fogMaterial.SetColor("_sunDiskColor", skySettings.sunDiskColor.Evaluate(GameTime.solarTime));
		atmosphericFog.fogMaterial.SetColor("_weatherSkyMod", currentWeatherSkyMod);
		atmosphericFog.fogMaterial.SetColor("_weatherFogMod", currentWeatherFogMod);

		atmosphericFog.fogMaterial.SetFloat ("_SkyFogHeight", Fog.skyFogHeight);
		atmosphericFog.fogMaterial.SetFloat ("_scatteringStrenght", Fog.scatteringStrenght);
		atmosphericFog.fogMaterial.SetFloat ("_skyFogIntensity", fogSettings.skyFogIntensity);
		atmosphericFog.fogMaterial.SetFloat ("_SkyFogStrenght", Fog.skyFogStrength);
		atmosphericFog.fogMaterial.SetFloat ("_SunBlocking", Fog.sunBlocking);
	
		atmosphericFog.fogMaterial.SetVector ("_Bm", BetaMie () * (skySettings.mie * (Fog.scatteringStrenght * GameTime.solarTime)));
		atmosphericFog.fogMaterial.SetVector ("_Br", BetaRay() * skySettings.rayleigh);
		atmosphericFog.fogMaterial.SetVector ("_mieG", GetMieG ());
		atmosphericFog.fogMaterial.SetFloat ("_SunIntensity",  skySettings.sunIntensity);

		atmosphericFog.fogMaterial.SetFloat ("_SunDiskSize",  skySettings.sunDiskScale);
		atmosphericFog.fogMaterial.SetFloat ("_SunDiskIntensity",  skySettings.sunDiskIntensity);
		atmosphericFog.fogMaterial.SetFloat ("_SunDiskSize",  skySettings.sunDiskScale);

		atmosphericFog.fogMaterial.SetFloat ("_Exposure", skySettings.skyExposure);
		atmosphericFog.fogMaterial.SetFloat ("_SkyLuminance", skySettings.skyLuminence.Evaluate(GameTime.solarTime));
		atmosphericFog.fogMaterial.SetFloat ("_scatteringPower", skySettings.scatteringCurve.Evaluate(GameTime.solarTime));
		atmosphericFog.fogMaterial.SetFloat ("_SkyColorPower", skySettings.skyColorPower.Evaluate(GameTime.solarTime));

		atmosphericFog.fogMaterial.SetFloat ("_heightFogIntensity", fogSettings.heightFogIntensity);
		atmosphericFog.fogMaterial.SetFloat ("_scatteringStrenght", Fog.scatteringStrenght);
		atmosphericFog.fogMaterial.SetFloat ("_distanceFogIntensity", fogSettings.distanceFogIntensity);
		atmosphericFog.fogMaterial.SetFloat ("_maximumFogDensity", 1 - fogSettings.maximumFogDensity);

		atmosphericFog.fogMaterial.SetVector ("_NoiseData", new Vector4 (fogSettings.noiseScale, fogSettings.heightDensity, fogSettings.noiseIntensity, 0f));
		atmosphericFog.fogMaterial.SetVector ("_NoiseVelocity", new Vector4 (fogSettings.heightFogVelocity.x, fogSettings.heightFogVelocity.y,0f, 0f));
		float hdr = HDR ? 1f : 0f;
		atmosphericFog.fogMaterial.SetFloat ("_hdr", hdr);
	}
		
	DateTime CreateSystemDate ()
	{
		DateTime date = new DateTime ();

		date = date.AddYears (GameTime.Years - 1);
		date = date.AddDays (GameTime.Days - 1);

		return date;
	}

	void UpdateSunAndMoonPosition ()
	{
		DateTime date = CreateSystemDate ();
		float d = 367 * date.Year - 7 * ( date.Year + (date.Month / 12 + 9) / 12 ) / 4 + 275 * date.Month/9 + date.Day - 730530;
		d += (GetUniversalTimeOfDay() / 24f);

		float ecl = 23.4393f - 3.563E-7f * d;

		CalculateSunPosition (d, ecl);
		CalculateMoonPosition (d, ecl);
	}


	 private float Remap (float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	void CalculateSunPosition (float d, float ecl)
	{
		/////http://www.stjarnhimlen.se/comp/ppcomp.html#5////
		///////////////////////// SUN ////////////////////////
		float w = 282.9404f + 4.70935E-5f * d;
		float e = 0.016709f - 1.151E-9f * d;
		float M = 356.0470f + 0.9856002585f * d;

		float E = M + e * Mathf.Rad2Deg * Mathf.Sin(Mathf.Deg2Rad * M) * (1 + e * Mathf.Cos(Mathf.Deg2Rad * M));

		float xv = Mathf.Cos(Mathf.Deg2Rad * E) - e;
		float yv = Mathf.Sin(Mathf.Deg2Rad * E) * Mathf.Sqrt(1 - e*e);

		float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
		float r = Mathf.Sqrt(xv*xv + yv*yv);

		float l = v + w;

		float xs = r * Mathf.Cos(Mathf.Deg2Rad * l);
		float ys = r * Mathf.Sin(Mathf.Deg2Rad * l);

		float xe = xs;
		float ye = ys * Mathf.Cos(Mathf.Deg2Rad * ecl);
		float ze = ys * Mathf.Sin(Mathf.Deg2Rad * ecl);

		float decl_rad = Mathf.Atan2(ze, Mathf.Sqrt(xe*xe + ye*ye));
		float decl_sin = Mathf.Sin(decl_rad);
		float decl_cos = Mathf.Cos(decl_rad);

		float GMST0 = (l + 180);
		float GMST  = GMST0 + GetUniversalTimeOfDay() * 15;
		LST = GMST + GameTime.Longitude;

		if (LST > 24)LST -= 24;  
		else if (LST < 0)LST += 24;

		CalculateStarsPosition (LST);

		float HA_deg = LST - Mathf.Rad2Deg * Mathf.Atan2(ye, xe);
		float HA_rad = Mathf.Deg2Rad * HA_deg;
		float HA_sin = Mathf.Sin(HA_rad);
		float HA_cos = Mathf.Cos(HA_rad);

		float x = HA_cos * decl_cos;
		float y = HA_sin * decl_cos;
		float z = decl_sin;

		float sin_Lat = Mathf.Sin(Mathf.Deg2Rad * GameTime.Latitude);
		float cos_Lat = Mathf.Cos(Mathf.Deg2Rad * GameTime.Latitude);

		float xhor = x * sin_Lat - z * cos_Lat;
		float yhor = y;
		float zhor = x * cos_Lat + z * sin_Lat;

		float azimuth  = Mathf.Atan2(yhor, xhor) + Mathf.Deg2Rad * 180;
		float altitude = Mathf.Atan2(zhor, Mathf.Sqrt(xhor*xhor + yhor*yhor));

		float sunTheta = (90 * Mathf.Deg2Rad) - altitude;
		float sunPhi   = azimuth;

		//Set SolarTime: 1 = mid-day (sun directly above you), 0.5 = sunset/dawn, 0 = midnight;
		GameTime.solarTime = Mathf.Clamp01(Remap (sunTheta, -1.5f, 0f, 1.5f, 1f));

		SunTransform.localPosition = OrbitalToLocal(sunTheta, sunPhi);

		// Always Face dome or better face the playerCamera!
		if(PlayerCamera != null)
			SunTransform.LookAt(PlayerCamera.transform.position);
		else
			SunTransform.transform.LookAt(DomeTransform.position);


		SetupShader(sunTheta);
	}

	void CalculateMoonPosition (float d, float ecl)
	{
		float N = 125.1228f - 0.0529538083f * d;
		float i = 5.1454f;
		float w = 318.0634f + 0.1643573223f * d;
		float a = 60.2666f;
		float e = 0.054900f;
		float M = 115.3654f + 13.0649929509f * d;

		float sun_w = 282.9404f + 4.70935E-5f * d;
		float sun_M = 356.0470f + 0.9856002585f * d;

		float sin_M = Mathf.Sin(Mathf.Deg2Rad * M);
		float cos_M = Mathf.Cos(Mathf.Deg2Rad * M);

		float E = M + e * Mathf.Rad2Deg * sin_M * (1 + e * cos_M);
	
		E0 = E;

		for (int eL = 0; eL < 1000; eL++){
			E1 = E0 - (E0 - (180.0f/pi) * e * Mathf.Sin(E0 * Mathf.Deg2Rad) - M) / ( 1.0f - e * Mathf.Cos(Mathf.Deg2Rad * E0));
			if (Mathf.Abs(E1)-Mathf.Abs(E0) < 0.005f){
				break;
			} else {
				E0 = E1;
			}
		}
		E = E1;

		float xv = a * (Mathf.Cos(Mathf.Deg2Rad * E) - e);
		float yv = a * (Mathf.Sin(Mathf.Deg2Rad * E) * Mathf.Sqrt(1 - e*e));

		float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
		float r = Mathf.Sqrt(xv*xv + yv*yv);

		float l = v + w;

		float sin_l = Mathf.Sin(Mathf.Deg2Rad * l);
		float cos_l = Mathf.Cos(Mathf.Deg2Rad * l);
		float cos_i = Mathf.Cos(Mathf.Sin(Mathf.Deg2Rad * i));
		float sin_N = Mathf.Sin(Mathf.Deg2Rad * N);
		float cos_N = Mathf.Cos(Mathf.Deg2Rad * N);

		float xh = r * (cos_N * cos_l - sin_N * sin_l * cos_i);
		float yh = r * (sin_N * cos_l + cos_N * sin_l * cos_i);
		float zh = r * (sin_l * Mathf.Sin(Mathf.Deg2Rad * i));

		float moonLongitude = Mathf.Atan2(yh,xh)*Mathf.Rad2Deg;
		float moonLatitude = Mathf.Atan2(zh,Mathf.Sqrt(xh*xh+yh*yh))*Mathf.Rad2Deg;

		float Ms = sun_M;	// Mean Anomaly of the Sun
		float Mm = M;	// Mean Anomaly of the Moon
		float Nm = N;	// Longitude of the Moon's node
		//float ws = sun_w;	// Argument of perihelion for the Sun
		float wm = w;	// Argument of perihelion for the Moon

		float Ls = sun_w + sun_M;										// Mean Longitude of the Sun  (Ns=0)
		float Lm = Mm + wm + Nm;								// Mean longitude of the Moon
		float Dm = Lm - Ls;									// Mean elongation of the Moon
		float F = Lm - Nm;									// Argument of latitude for the Moon

		//Add these terms to the Moon's longitude (degrees):
		moonLongitude -= 1.274f * Mathf.Sin((Mm - (2.0f*Dm))* Mathf.Deg2Rad );          		// (the Evection)
		moonLongitude += 0.658f * Mathf.Sin((2.0f*Dm) * Mathf.Deg2Rad);               		// (the Variation)
		moonLongitude -= 0.186f * Mathf.Sin(Ms* Mathf.Deg2Rad);                 		// (the Yearly Equation)
		moonLongitude -= 0.059f * Mathf.Sin(((2.0f*Mm) - (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLongitude -= 0.057f * Mathf.Sin((Mm - (2.0f*Dm) + Ms) * Mathf.Deg2Rad);
		moonLongitude += 0.053f * Mathf.Sin((Mm + (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLongitude += 0.046f * Mathf.Sin(((2.0f*Dm) - Ms) * Mathf.Deg2Rad);
		moonLongitude += 0.041f * Mathf.Sin((Mm - Ms) * Mathf.Deg2Rad);
		moonLongitude -= 0.035f * Mathf.Sin(Dm * Mathf.Deg2Rad);                 		// (the Parallactic Equation)
		moonLongitude -= 0.031f * Mathf.Sin((Mm + Ms) * Mathf.Deg2Rad);
		moonLongitude -= 0.015f * Mathf.Sin(((2.0f*F) - (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLongitude += 0.011f * Mathf.Sin((Mm - (4.0f*Dm)) * Mathf.Deg2Rad);

		//Add these terms to the Moon's latitude (degrees):
		moonLatitude -= 0.173f * Mathf.Sin((F - (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLatitude -= 0.055f * Mathf.Sin(((Mm) - F - (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLatitude -= 0.046f * Mathf.Sin(((Mm) + F - (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLatitude += 0.033f * Mathf.Sin((F + (2.0f*Dm)) * Mathf.Deg2Rad);
		moonLatitude += 0.017f * Mathf.Sin(((2.0f*Mm) + F) * Mathf.Deg2Rad);

		xh = 1f * Mathf.Cos(moonLongitude * Mathf.Deg2Rad) * Mathf.Cos(moonLatitude * Mathf.Deg2Rad);
		yh = 1f * Mathf.Sin(moonLongitude* Mathf.Deg2Rad) * Mathf.Cos(moonLatitude* Mathf.Deg2Rad);
		zh = 1f * Mathf.Sin(moonLatitude* Mathf.Deg2Rad);

		float xe = xh;
		float ye = yh * Mathf.Cos(Mathf.Deg2Rad * ecl) - zh * Mathf.Sin(Mathf.Deg2Rad * ecl);
		float ze = zh * Mathf.Sin(Mathf.Deg2Rad * ecl) + zh * Mathf.Cos(Mathf.Deg2Rad * ecl);

		float HA = Mathf.Deg2Rad * ( LST - Mathf.Rad2Deg * Mathf.Atan2(ye , xe));
		float cos_decl = Mathf.Cos(Mathf.Atan2( ze, Mathf.Sqrt(xe * xe + ye * ye)));

		float x = Mathf.Cos(HA) * cos_decl;
		float y = Mathf.Sin(HA) * cos_decl;
		float z = Mathf.Sin(Mathf.Atan2(ze, Mathf.Sqrt(xe*xe + ye*ye)));

		float sin_Lat = Mathf.Sin(Mathf.Deg2Rad * GameTime.Latitude);
		float cos_Lat = Mathf.Cos(Mathf.Deg2Rad * GameTime.Latitude);

		float xhor = x * sin_Lat - z * cos_Lat;
		float yhor = y;
		float zhor = x * cos_Lat + z * sin_Lat;

		float azimuth = Mathf.Atan2(yhor, xhor) + Mathf.Deg2Rad * 180;
		float altitude = Mathf.Atan2(zhor, Mathf.Sqrt(xhor*xhor + yhor*yhor));

		float MoonTheta = (90 * Mathf.Deg2Rad) - altitude;
		float MoonPhi = azimuth;

		MoonTransform.localPosition = OrbitalToLocal(MoonTheta, MoonPhi);
		GameTime.lunarTime = Mathf.Clamp01(Remap (MoonTheta, -1.5f, 0f, 1.5f, 1f));

		// Always Face dome or better face the playerCamera!
		if(PlayerCamera != null)
			MoonTransform.LookAt(PlayerCamera.transform.position);
		else
			MoonTransform.transform.LookAt(DomeTransform.position);
	}

	void CalculateStarsPosition (float siderealTime)
	{
		Quaternion starsRotation = Quaternion.Euler (90 - GameTime.Latitude, GameTime.Longitude, 0); 
		starsRotation *= Quaternion.Euler(0, siderealTime, 0);

		Components.starsRotation.localRotation = starsRotation;
		RenderSettings.skybox.SetMatrix ("_StarsMatrix", Components.starsRotation.worldToLocalMatrix);
		//Matrix4x4 starsMatrix = Matrix4x4.TRS (DomeTransform.localPosition, starsRotation, new Vector3 (1f, 1f, 1f));
		//RenderSettings.skybox.SetMatrix ("_StarsMatrix", starsMatrix);
	}




	void UpdateSimpleSunAndMoonPosition ()
	{
		// Calculates the Solar latitude
		float latitudeRadians = Mathf.Deg2Rad * GameTime.Latitude;
		float latitudeRadiansSin = Mathf.Sin(latitudeRadians);
		float latitudeRadiansCos = Mathf.Cos(latitudeRadians);

		// Calculates the Solar longitude
		float longitudeRadians = Mathf.Deg2Rad * GameTime.Longitude;

		// Solar declination - constant for the whole globe at any given day
		float solarDeclination = 0.4093f * Mathf.Sin(2f * pi / 368f * (GameTime.Days - 81f));
		float solarDeclinationSin = Mathf.Sin(solarDeclination);
		float solarDeclinationCos = Mathf.Cos(solarDeclination);

		// Calculate Solar time
		float timeZone = (int)(GameTime.Longitude / 15f);
		float meridian = Mathf.Deg2Rad * 15f * timeZone;
		float solarTime = GetUniversalTimeOfDay() + 0.170f * Mathf.Sin(4f * pi / 373f * (GameTime.Days - 80f)) - 0.129f * Mathf.Sin(2f * pi / 355f * (GameTime.Days - 8f))  + 12f / pi * (meridian - longitudeRadians);
		float solarTimeRadians = pi / 12f * solarTime;
		float solarTimeSin = Mathf.Sin(solarTimeRadians);
		float solarTimeCos = Mathf.Cos(solarTimeRadians);

		// Solar altitude angle between the sun and the horizon
		float solarAltitudeSin = latitudeRadiansSin * solarDeclinationSin - latitudeRadiansCos * solarDeclinationCos * solarTimeCos;
		float solarAltitude = Mathf.Asin(solarAltitudeSin);

		// Solar azimuth angle of the sun around the horizon
		float solarAzimuthY = -solarDeclinationCos * solarTimeSin;
		float solarAzimuthX = latitudeRadiansCos * solarDeclinationSin - latitudeRadiansSin * solarDeclinationCos * solarTimeCos;
		float solarAzimuth = Mathf.Atan2(solarAzimuthY, solarAzimuthX);

		// Convert to spherical coords
		float theta = pi / 2 - solarAltitude;
		float phi = solarAzimuth;

		GameTime.solarTime = Mathf.Clamp01(Remap (theta, -1.5f, 0f, 1.5f, 1f));
		GameTime.lunarTime = Mathf.Clamp01(Remap (theta - pi, -1.5f, 0f, 1.5f, 1f));

		// Update sun position
		SunTransform.localPosition = OrbitalToLocal(theta, phi);
		SunTransform.LookAt(DomeTransform.position);
		// Update moon position
		MoonTransform.localPosition = OrbitalToLocal(theta - pi, phi);
		MoonTransform.LookAt(DomeTransform.position);

		SetupShader(theta);
		RenderSettings.skybox.SetMatrix ("_StarsMatrix", SunTransform.worldToLocalMatrix);
	}

	Vector3 UpdateSatellitePosition (float orbit,float orbit2,float speed)
	{
		// Calculates the Solar latitude
		float latitudeRadians = Mathf.Deg2Rad * GameTime.Latitude;
		float latitudeRadiansSin = Mathf.Sin(latitudeRadians);
		float latitudeRadiansCos = Mathf.Cos(latitudeRadians);

		// Calculates the Solar longitude
		float longitudeRadians = Mathf.Deg2Rad * GameTime.Longitude;

		// Solar declination - constant for the whole globe at any given day
		float solarDeclination = orbit2 * Mathf.Sin(2f * pi / 368f * (GameTime.Days - 81f));
		float solarDeclinationSin = Mathf.Sin(solarDeclination);
		float solarDeclinationCos = Mathf.Cos(solarDeclination);

		// Calculate Solar time
		float timeZone = (int)(GameTime.Longitude / 15f);
		float meridian = Mathf.Deg2Rad * 15f * timeZone;

		float solarTime = GetUniversalTimeOfDay() + orbit * Mathf.Sin(4f * pi / 377f * (GameTime.Days - 80f)) - speed * Mathf.Sin(1f * pi / 355f * (GameTime.Days - 8f))  + 12f / pi * (meridian - longitudeRadians);

		float solarTimeRadians = pi / 12f * solarTime;
		float solarTimeSin = Mathf.Sin(solarTimeRadians);
		float solarTimeCos = Mathf.Cos(solarTimeRadians);

		// Solar altitude angle between the sun and the horizon
		float solarAltitudeSin = latitudeRadiansSin * solarDeclinationSin - latitudeRadiansCos * solarDeclinationCos * solarTimeCos;
		float solarAltitude = Mathf.Asin(solarAltitudeSin);

		// Solar azimuth angle of the sun around the horizon
		float solarAzimuthY = -solarDeclinationCos * solarTimeSin;
		float solarAzimuthX = latitudeRadiansCos * solarDeclinationSin - latitudeRadiansSin * solarDeclinationCos * solarTimeCos;
		float solarAzimuth = Mathf.Atan2(solarAzimuthY, solarAzimuthX);

		// Convert to spherical coords
		float theta = pi / 2 - solarAltitude;
		float phi = solarAzimuth;

		// Send local position
		return OrbitalToLocal(theta, phi);
	}
		
	Vector3 OrbitalToLocal(float theta, float phi)
	{
		Vector3 res;

		float sinTheta = Mathf.Sin(theta);
		float cosTheta = Mathf.Cos(theta);
		float sinPhi   = Mathf.Sin(phi);
		float cosPhi   = Mathf.Cos(phi);

		res.z = sinTheta * cosPhi;
		res.y = cosTheta;
		res.x = sinTheta * sinPhi;

		return res;
	}



	void UpdateReflections ()
	{
		Components.GlobalReflectionProbe.intensity = lightSettings.globalReflectionsIntensity;

		if ((currentTimeInHours > lastRelfectionUpdate + lightSettings.globalReflectionsUpdate || currentTimeInHours < lastRelfectionUpdate - lightSettings.globalReflectionsUpdate) && lightSettings.globalReflections) {
			Components.GlobalReflectionProbe.enabled = true;
			lastRelfectionUpdate = currentTimeInHours;
			Components.GlobalReflectionProbe.RenderProbe ();
		} else if (!lightSettings.globalReflections) {
			Components.GlobalReflectionProbe.enabled = false;
		}
	}

	// Update the GameTime
	void UpdateTime()
	{
		if (Application.isPlaying) {

			float t = 0f;

			if(!isNight)
				t = (24.0f / 60.0f) / GameTime.DayLengthInMinutes;
			else
				t = (24.0f / 60.0f) / GameTime.NightLengthInMinutes;
			
			hourTime = t * Time.deltaTime;

			switch (GameTime.ProgressTime) {
			case EnviroTime.TimeProgressMode.None://Set Time over editor or other scripts.
				SetTime (GameTime.Years, GameTime.Days, GameTime.Hours, GameTime.Minutes, GameTime.Seconds);
			break;

			case EnviroTime.TimeProgressMode.Simulated:
				internalHour += hourTime;
				SetGameTime ();
				customMoonPhase += Time.deltaTime / (30f * (GameTime.DayLengthInMinutes * 60f)) * 2f;
			break;

			case EnviroTime.TimeProgressMode.SystemTime:
				SetTime (System.DateTime.Now);
				customMoonPhase += Time.deltaTime / (30f * (1440f * 60f)) * 2f;
			break;
			}
		} 
		else 
		{
			SetTime (GameTime.Years, GameTime.Days, GameTime.Hours, GameTime.Minutes, GameTime.Seconds);
		}

		if (customMoonPhase < -1) customMoonPhase += 2;
		else if (customMoonPhase > 1) customMoonPhase -= 2;

		//Fire OnHour Event
		if (internalHour > (lastHourUpdate + 1f)) {
			lastHourUpdate = internalHour;
			NotifyHourPassed ();
		}

		// Check Days
		if(GameTime.Days >= (seasonsSettings.SpringInDays + seasonsSettings.SummerInDays + seasonsSettings.AutumnInDays + seasonsSettings.WinterInDays)){
			GameTime.Years = GameTime.Years + 1;
			GameTime.Days = 0;
			NotifyYearPassed ();
		}

		currentHour = internalHour;
		currentDay = GameTime.Days;
		currentYear = GameTime.Years;

		currentTimeInHours = GetInHours (internalHour, currentDay, currentYear);
	}

	private void SetInternalTime(int year, int dayOfYear, int hour, int minute, int seconds)
	{
		GameTime.Years = year;
		GameTime.Days = dayOfYear;
		GameTime.Minutes = minute;
		GameTime.Hours = hour;
		internalHour = hour + (minute * 0.0166667f) + (seconds * 0.000277778f);
	}

	/// <summary>
	/// Set the time of day in hours. (12.5 = 12:30)
	/// </summary>
	private void SetGameTime()
	{ 
		if (internalHour >= 24f) {
			internalHour = internalHour - 24f;
			NotifyHourPassed ();
			lastHourUpdate = internalHour;
			GameTime.Days = GameTime.Days + 1;
			NotifyDayPassed ();
		} else if (internalHour < 0f) {
			internalHour = 24f + internalHour;
			GameTime.Days = GameTime.Days - 1;
			lastHourUpdate = internalHour;
			NotifyDayPassed ();
		}

		float inHours = internalHour;
		GameTime.Hours = (int)(inHours);
		inHours -= GameTime.Hours;
		GameTime.Minutes = (int)(inHours * 60f);
		inHours -= GameTime.Minutes * 0.0166667f;
		GameTime.Seconds = (int)(inHours * 3600f);
	}


	void UpdateAmbientLight ()
	{
		switch (lightSettings.ambientMode) {
		case UnityEngine.Rendering.AmbientMode.Flat:
			RenderSettings.ambientSkyColor = Color.Lerp(lightSettings.ambientSkyColor.Evaluate (GameTime.solarTime),currentWeatherLightMod,currentWeatherLightMod.a) * lightSettings.ambientIntensity.Evaluate(GameTime.solarTime);
			break;
			 
		case UnityEngine.Rendering.AmbientMode.Trilight:
			RenderSettings.ambientSkyColor = Color.Lerp(lightSettings.ambientSkyColor.Evaluate (GameTime.solarTime),currentWeatherLightMod,currentWeatherLightMod.a) * lightSettings.ambientIntensity.Evaluate(GameTime.solarTime);
			RenderSettings.ambientEquatorColor = Color.Lerp(lightSettings.ambientEquatorColor.Evaluate (GameTime.solarTime),currentWeatherLightMod,currentWeatherLightMod.a) * lightSettings.ambientIntensity.Evaluate(GameTime.solarTime);
			RenderSettings.ambientGroundColor = Color.Lerp(lightSettings.ambientGroundColor.Evaluate (GameTime.solarTime),currentWeatherLightMod,currentWeatherLightMod.a) * lightSettings.ambientIntensity.Evaluate(GameTime.solarTime);
			break;

		case UnityEngine.Rendering.AmbientMode.Skybox:
			DynamicGI.UpdateEnvironment ();
			break;

		}
	}

	// Calculate sun and moon light intensity and color
	private void CalculateDirectLight()
	{ 
		MainLight.color = Color.Lerp(lightSettings.LightColor.Evaluate (GameTime.solarTime),currentWeatherLightMod,currentWeatherLightMod.a);

		Shader.SetGlobalColor ("_EnviroLighting", lightSettings.LightColor.Evaluate (GameTime.solarTime));
		Shader.SetGlobalVector ("_SunDirection", -Components.Sun.transform.forward);

		Shader.SetGlobalVector ("_SunPosition", Components.Sun.transform.localPosition + (-Components.Sun.transform.forward * 10000f));
		Shader.SetGlobalVector ("_MoonPosition", Components.Moon.transform.localPosition);
			
		float lightIntensity;

		// Set sun and moon intensity
		if (!isNight)
		{
			lightIntensity = lightSettings.directLightIntensity.Evaluate (GameTime.solarTime);
			Components.DirectLight.position = Components.Sun.transform.position;
			Components.DirectLight.rotation = Components.Sun.transform.rotation;
		}
		else
		{
			lightIntensity = lightSettings.directLightIntensity.Evaluate (GameTime.solarTime) * Mathf.Clamp01(2f - Mathf.Abs(customMoonPhase));
			Components.DirectLight.position = Components.Moon.transform.position;
			Components.DirectLight.rotation = Components.Moon.transform.rotation;
		}
			
		// Set the light and shadow intensity
		MainLight.intensity = Mathf.Lerp (MainLight.intensity, lightIntensity, 5f * Time.deltaTime);
		MainLight.shadowStrength = lightSettings.shadowStrength;
	}

	// Make the parameters stay in reasonable range
	private void ValidateParameters()
	{
		// Keep GameTime Parameters right!
		internalHour = Mathf.Repeat(internalHour, 24f);
		GameTime.Longitude = Mathf.Clamp(GameTime.Longitude, -180, 180);
		GameTime.Latitude = Mathf.Clamp(GameTime.Latitude, -90, 90);

		#if UNITY_EDITOR
		if (GameTime.DayLengthInMinutes == 0)
		{
			internalHour = 12f;
			customMoonPhase = 0f;
		}
		// Moon
		customMoonPhase = Mathf.Clamp(customMoonPhase, -1f, 1f);
		#endif
	}
		
	///////////////////////////////////////////////////////////////////cloud meshes/////////////////////////////////////////////////////////////////////////
	private Mesh CreateCloudsLayer (int layerID, bool isShadowMesh)
	{
		int sliceQuality = 1;

		if (!isShadowMesh)
			sliceQuality = cloudsSettings.cloudsLayers [layerID].Quality;

		//Setting arrays up
		Vector3[] vertices = new Vector3[(cloudsSettings.cloudsLayers[layerID].segmentCount * cloudsSettings.cloudsLayers[layerID].segmentCount) * sliceQuality];
		Vector2[] uvMap = new Vector2[vertices.Length];
		int[] triangleConstructor = new int[(cloudsSettings.cloudsLayers[layerID].segmentCount-1) * (cloudsSettings.cloudsLayers[layerID].segmentCount-1) * sliceQuality * 2 * 3];
		Color[] vertexColor = new Color[vertices.Length];
		float tempRatio = 1.0f / ((float)cloudsSettings.cloudsLayers[layerID].segmentCount - 1);
		Vector3 posGainPerVertices = new Vector3(tempRatio * 2f, 1.0f/(Mathf.Clamp(sliceQuality - 1, 1, 999999)) * cloudsSettings.cloudsLayers[layerID].thickness, tempRatio * 2f); 
		float posGainPerUV = tempRatio;

		// Lets Create our mesh yea!
		int iteration = 0; 
		int vIncrement = 0;
		int increment = 0;
		float curvature = 0.0f;

		float depthColor = -1.0f;
		float mirrorColor = 0.0f;
		//computes slices by vertices row, each time the row ends, do the next one.
		for(int s = 0; s < sliceQuality; s++){
			depthColor = -1 + (s*(2/(float)sliceQuality));

			if(s < sliceQuality * 0.5f)
				mirrorColor = 0 + (1.0f / ((float)sliceQuality * 0.5f)) * s;
			else 				 
				mirrorColor = 2 - (1.0f / ((float)sliceQuality * 0.5f)) * (s + 1);

			if(sliceQuality == 1 || isShadowMesh)
				mirrorColor = 1;
			//horizontal vertices
			for(int h = 0; h < cloudsSettings.cloudsLayers[layerID].segmentCount; h++){
				int incrementV = cloudsSettings.cloudsLayers[layerID].segmentCount * iteration;
				//vertical vertices
				for(int v = 0; v < cloudsSettings.cloudsLayers[layerID].segmentCount; v++){

					if(cloudsSettings.cloudsLayers[layerID].curved)
						curvature = Vector3.Distance(new Vector3(posGainPerVertices.x*v - 1f, 0.0f, posGainPerVertices.z * h - 1f), Vector3.zero);

					if(sliceQuality == 1 || isShadowMesh)					
						vertices[v+incrementV] = new Vector3(posGainPerVertices.x*v- 1f, 0f + (Mathf.Pow(curvature, 2f) * cloudsSettings.cloudsLayers[layerID].curvedIntensity), posGainPerVertices.z*h-1f);
					else 
						vertices[v+incrementV] = new Vector3(posGainPerVertices.x*v- 1f, posGainPerVertices.y*s-(cloudsSettings.cloudsLayers[layerID].thickness / 2f)+(Mathf.Pow(curvature, 2f) * cloudsSettings.cloudsLayers[layerID].curvedIntensity), posGainPerVertices.z * h - 1f);

					uvMap[v+incrementV] = new Vector2(posGainPerUV*v, posGainPerUV*h);
					vertexColor[v+incrementV] = new Vector4(depthColor, depthColor, depthColor, mirrorColor);
				}
				iteration += 1;

				//Triangle construction
				if(h >= 1){
					for(int tri = 0; tri < cloudsSettings.cloudsLayers[layerID].segmentCount-1; tri++){
						triangleConstructor[0+increment] = (0+tri)+vIncrement+(s*cloudsSettings.cloudsLayers[layerID].segmentCount);//
						triangleConstructor[1+increment] = (cloudsSettings.cloudsLayers[layerID].segmentCount+tri)+vIncrement+(s*cloudsSettings.cloudsLayers[layerID].segmentCount);
						triangleConstructor[2+increment] = (1+tri)+vIncrement+(s*cloudsSettings.cloudsLayers[layerID].segmentCount);//
						triangleConstructor[3+increment] = ((cloudsSettings.cloudsLayers[layerID].segmentCount+1)+tri)+vIncrement+(s*cloudsSettings.cloudsLayers[layerID].segmentCount);
						triangleConstructor[4+increment] = (1+tri)+vIncrement+(s*cloudsSettings.cloudsLayers[layerID].segmentCount);
						triangleConstructor[5+increment] = (cloudsSettings.cloudsLayers[layerID].segmentCount+tri)+vIncrement+(s*cloudsSettings.cloudsLayers[layerID].segmentCount);
						increment +=6;
					}
					vIncrement += cloudsSettings.cloudsLayers[layerID].segmentCount;
				}
			}
		}
		if (!isShadowMesh) 
		{
			Mesh slicedCloudMesh = new Mesh ();
			slicedCloudMesh.Clear ();
			slicedCloudMesh.name = "Clouds";
			slicedCloudMesh.vertices = vertices;
			slicedCloudMesh.triangles = triangleConstructor;
			slicedCloudMesh.uv = uvMap;
			slicedCloudMesh.colors = vertexColor;
			slicedCloudMesh.RecalculateNormals ();
			slicedCloudMesh.RecalculateBounds ();
			CalcMeshTangents (slicedCloudMesh);

			return slicedCloudMesh;
		} 
		else
		{
			Mesh shadowMesh = new Mesh ();
			shadowMesh.Clear ();
			shadowMesh.name = "CloudsShadows";
			shadowMesh.vertices = vertices;
			shadowMesh.triangles = triangleConstructor;
			shadowMesh.uv = uvMap;
			shadowMesh.colors = vertexColor;
			shadowMesh.RecalculateNormals ();
			shadowMesh.RecalculateBounds ();
			CalcMeshTangents (shadowMesh);

			return shadowMesh;
		}
	}

	public static void CalcMeshTangents(Mesh mesh)
	{
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;

		int triangleCount = triangles.Length;
		int vertexCount = vertices.Length;

		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];

		Vector4[] tangents = new Vector4[vertexCount];

		for (long a = 0; a < triangleCount; a += 3)
		{
			long i1 = triangles[a + 0];
			long i2 = triangles[a + 1];
			long i3 = triangles[a + 2];

			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];

			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];

			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;

			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;

			float r = 1.0f / (s1 * t2 - s2 * t1);

			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;

			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}


		for (long a = 0; a < vertexCount; ++a)
		{
			Vector3 n = normals[a];
			Vector3 t = tan1[a];
			Vector3.OrthoNormalize(ref n, ref t);

			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;

			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}
		mesh.tangents = tangents;
	}

	///////////////////////////////////////////////////////////////////WEATHER SYSTEM /////////////////////////////////////////////////////////////////////////
	public void RegisterZone (EnviroZone zoneToAdd)
	{
		Weather.zones.Add (zoneToAdd);
	}


	public void EnterZone (EnviroZone zone)
	{
		Weather.currentActiveZone = zone;
	}

	public void ExitZone ()
	{

	}

	public void CreateWeatherEffectHolder()
	{
		if (Weather.VFXHolder == null) {
			GameObject VFX = new GameObject ();
			VFX.name = "VFX";
			VFX.transform.parent = EffectsHolder.transform;
			VFX.transform.localPosition = Vector3.zero;
			Weather.VFXHolder = VFX;
		}
	}

	private void UpdateAudioSource (EnviroWeatherPreset i)
	{
		if (i != null && i.weatherSFX != null)
		{
			if (i.weatherSFX == Weather.currentAudioSource.audiosrc.clip)
			{
				if(Weather.currentAudioSource.audiosrc.volume < 0.1f)
					Weather.currentAudioSource.FadeIn(i.weatherSFX);
				
				return;
			}

			if (Weather.currentAudioSource == AudioSourceWeather)
			{
				AudioSourceWeather.FadeOut();
				AudioSourceWeather2.FadeIn(i.weatherSFX);
				Weather.currentAudioSource = AudioSourceWeather2;
			}
			else if (Weather.currentAudioSource == AudioSourceWeather2)
			{
				AudioSourceWeather2.FadeOut();
				AudioSourceWeather.FadeIn(i.weatherSFX);
				Weather.currentAudioSource = AudioSourceWeather;
			}
		} 
		else
		{
			AudioSourceWeather.FadeOut();
			AudioSourceWeather2.FadeOut();
		}
	}

	private void UpdateClouds (EnviroWeatherPreset i, bool withTransition)
	{
		if (i == null)
			return;

		float speed = 500f * Time.deltaTime;

		if (withTransition)
			speed = weatherSettings.cloudTransitionSpeed * Time.deltaTime;

		for(int q = 0; q < cloudsLayers.Count; q++)
		{
			if (i.cloudConfig.Count > q) {
				cloudsLayers [q].FirstColor = Color.Lerp (cloudsLayers [q].FirstColor, i.cloudConfig [q].BaseColor, speed);
				cloudsLayers [q].DirectLightIntensity = Mathf.Lerp (cloudsLayers [q].DirectLightIntensity, i.cloudConfig [q].DirectLightInfluence, speed);
				cloudsLayers [q].Coverage = Mathf.Lerp (cloudsLayers [q].Coverage, i.cloudConfig [q].Coverage, speed);
				cloudsLayers [q].Density = Mathf.Lerp (cloudsLayers [q].Density, i.cloudConfig [q].Density, speed);
				cloudsLayers [q].Alpha = Mathf.Lerp (cloudsLayers [q].Alpha, i.cloudConfig [q].Alpha, speed);
			} 
			else 
			{
				cloudsLayers [q].Density = Mathf.Lerp (cloudsLayers [q].Density, 0f, speed);
				cloudsLayers [q].Coverage = Mathf.Lerp (cloudsLayers [q].Coverage, -1f, speed);
				cloudsLayers [q].Alpha = Mathf.Lerp (cloudsLayers [q].Alpha, 0.5f, speed);
			}
		}
		currentWeatherSkyMod = Color.Lerp (currentWeatherSkyMod, i.weatherSkyMod.Evaluate(GameTime.solarTime), speed);
		currentWeatherFogMod = Color.Lerp (currentWeatherFogMod, i.weatherFogMod.Evaluate(GameTime.solarTime), speed * 10);
		currentWeatherLightMod = Color.Lerp (currentWeatherLightMod, i.weatherLightMod.Evaluate(GameTime.solarTime), speed);
	}


	void UpdateFog (EnviroWeatherPreset i, bool withTransition)
	{
		if (i != null) {

			float speed = 500f * Time.deltaTime;

			if (withTransition)
				speed = weatherSettings.fogTransitionSpeed * Time.deltaTime;

			if (fogSettings.Fogmode == FogMode.Linear) {
				RenderSettings.fogEndDistance = Mathf.Lerp (RenderSettings.fogEndDistance, i.fogDistance, speed);
				RenderSettings.fogStartDistance = Mathf.Lerp (RenderSettings.fogStartDistance, i.fogStartDistance, speed);
			} else
				RenderSettings.fogDensity = Mathf.Lerp (RenderSettings.fogDensity, i.fogDensity, speed);


			// Set the Fog color to light color to match Day-Night cycle and weather
			RenderSettings.fogColor = Color.Lerp(lightSettings.ambientSkyColor.Evaluate(GameTime.solarTime),currentWeatherFogMod,currentWeatherFogMod.a);
			fogSettings.heightDensity = Mathf.Lerp (fogSettings.heightDensity, i.heightFogDensity, speed);
			Fog.skyFogHeight = Mathf.Lerp (Fog.skyFogHeight, i.SkyFogHeight, speed);
			Fog.skyFogStrength = Mathf.Lerp (Fog.skyFogStrength, i.SkyFogIntensity, speed);
			fogSettings.skyFogIntensity = Mathf.Lerp (fogSettings.skyFogIntensity, i.SkyFogIntensity, speed);
			Fog.scatteringStrenght = Mathf.Lerp (Fog.scatteringStrenght, i.FogScatteringIntensity, speed);
			Fog.sunBlocking = Mathf.Lerp (Fog.sunBlocking, i.fogSunBlocking, speed);
		}
	}

	void UpdateEffectSystems (EnviroWeatherPrefab id, bool withTransition)
	{
		if (id != null) {

			float speed = 500f * Time.deltaTime;

			if (withTransition)
				speed = weatherSettings.effectTransitionSpeed * Time.deltaTime;

			for (int i = 0; i < id.effectSystems.Count; i++) {
				// Set EmissionRate
				float val = Mathf.Lerp (GetEmissionRate (id.effectSystems [i]), id.effectEmmisionRates [i] * qualitySettings.GlobalParticleEmissionRates, speed );
				SetEmissionRate (id.effectSystems [i], val);
			}

			for (int i = 0; i < Weather.WeatherPrefabs.Count; i++) {
				if (Weather.WeatherPrefabs [i].gameObject != id.gameObject) {
					for (int i2 = 0; i2 < Weather.WeatherPrefabs [i].effectSystems.Count; i2++) {
						float val2 = Mathf.Lerp (GetEmissionRate (Weather.WeatherPrefabs [i].effectSystems [i2]), 0f, speed);

						if (val2 < 1f)
							val2 = 0f;

						SetEmissionRate (Weather.WeatherPrefabs [i].effectSystems [i2], val2);
					}
				}
			}

			Components.windZone.windMain = id.weatherPreset.WindStrenght; // Set Wind Strenght

			// The wetness raise
			if (Weather.wetness < id.weatherPreset.wetnessLevel) {
				Weather.wetness = Mathf.Lerp (Weather.curWetness, id.weatherPreset.wetnessLevel, weatherSettings.wetnessAccumulationSpeed * Time.deltaTime);
			} else { // Drying
				Weather.wetness = Mathf.Lerp (Weather.curWetness, id.weatherPreset.wetnessLevel, weatherSettings.wetnessDryingSpeed * Time.deltaTime);
			}

			Weather.wetness = Mathf.Clamp (Weather.wetness, 0f, 1f);
			Weather.curWetness = Weather.wetness;

			//Snowing
			if (Weather.snowStrength < id.weatherPreset.snowLevel)
				Weather.snowStrength = Mathf.Lerp (Weather.curSnowStrength, id.weatherPreset.snowLevel, weatherSettings.snowAccumulationSpeed * Time.deltaTime);
			else //Melting
				Weather.snowStrength = Mathf.Lerp (Weather.curSnowStrength, id.weatherPreset.snowLevel, weatherSettings.snowMeltingSpeed * Time.deltaTime);
		
			Weather.snowStrength = Mathf.Clamp (Weather.snowStrength, 0f, 1f);
			Weather.curSnowStrength = Weather.snowStrength;

			Shader.SetGlobalFloat ("_EnviroGrassSnow", Weather.curSnowStrength);
		}
	}

	public static float GetEmissionRate (ParticleSystem system)
	{
		return system.emission.rate.constantMax;
	}


	public static void SetEmissionRate (ParticleSystem sys, float emissionRate)
	{
		var emission = sys.emission;
		var rate = emission.rate;
		rate.constantMax = emissionRate;
		emission.rate = rate;
	}

	IEnumerator PlayThunderRandom()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(10,20));

		if (Weather.currentActiveWeatherPrefab.weatherPreset.isLightningStorm) 
		{
			Thunder ();
			StartCoroutine (PlayThunderRandom ());
		}
		else 
		{
			StopCoroutine (PlayThunderRandom ());
			Components.LightningGenerator.StopLightning ();
		}
	}

	private void Thunder ()
	{
		int i = UnityEngine.Random.Range(0,audioSettings.ThunderSFX.Count);
		AudioSourceThunder.clip = audioSettings.ThunderSFX[i];
		AudioSourceThunder.loop = false;
		AudioSourceThunder.Play ();
		Components.LightningGenerator.Lightning ();
	}

	void UpdateWeather ()
	{	
		//Current active weather not matching current zones weather
		if(Weather.currentActiveWeatherPreset != Weather.currentActiveZone.currentActiveZoneWeatherPreset)
		{
			Weather.lastActiveWeatherPreset = Weather.currentActiveWeatherPreset;
			Weather.lastActiveWeatherPrefab = Weather.currentActiveWeatherPrefab;
			Weather.currentActiveWeatherPreset = Weather.currentActiveZone.currentActiveZoneWeatherPreset;
			Weather.currentActiveWeatherPrefab = Weather.currentActiveZone.currentActiveZoneWeatherPrefab;
			if (Weather.currentActiveWeatherPreset != null) {
				NotifyWeatherChanged (Weather.currentActiveWeatherPreset);
				TryPlayAmbientSFX ();
				UpdateAudioSource (Weather.currentActiveWeatherPreset);

				if (Weather.currentActiveWeatherPrefab.weatherPreset.isLightningStorm)
					StartCoroutine (PlayThunderRandom ());
				else {
					StopCoroutine (PlayThunderRandom ());
					Components.LightningGenerator.StopLightning ();
				}
			}
		}

		if (Weather.currentActiveWeatherPrefab != null) 
		{
			UpdateClouds (Weather.currentActiveWeatherPreset, true);
			UpdateFog (Weather.currentActiveWeatherPreset, true);
			UpdateEffectSystems (Weather.currentActiveWeatherPrefab, true);
		}
	}


	/// <summary>
	/// Set weather directly with list id of Weather.WeatherTemplates. No transtions!
	/// </summary>
	public void SetWeatherOverwrite (int weatherId)
	{
		if (weatherId < 0 || weatherId > Weather.WeatherPrefabs.Count)
			return;

		if (Weather.WeatherPrefabs[weatherId] != Weather.currentActiveWeatherPrefab)
		{
			Weather.currentActiveZone.currentActiveZoneWeatherPrefab = Weather.WeatherPrefabs[weatherId];
			Weather.currentActiveZone.currentActiveZoneWeatherPreset = Weather.WeatherPrefabs[weatherId].weatherPreset;
			EnviroSky.instance.NotifyZoneWeatherChanged (Weather.WeatherPrefabs[weatherId].weatherPreset, Weather.currentActiveZone);
		}

		UpdateClouds (Weather.currentActiveZone.currentActiveZoneWeatherPreset, false);
		UpdateFog (Weather.currentActiveZone.currentActiveZoneWeatherPreset, false);
		UpdateEffectSystems (Weather.currentActiveZone.currentActiveZoneWeatherPrefab, false);
	}
	/// <summary>
	/// Set weather directly with preset of Weather.WeatherTemplates. No transtions!
	/// </summary>
	public void SetWeatherOverwrite (EnviroWeatherPreset preset)
	{
		if (preset == null)
			return;

		if (preset != Weather.currentActiveWeatherPreset)
		{
			for (int i = 0; i < Weather.WeatherPrefabs.Count; i++) {
				if (preset == Weather.WeatherPrefabs [i].weatherPreset) {
					Weather.currentActiveZone.currentActiveZoneWeatherPrefab = Weather.WeatherPrefabs[i];
					Weather.currentActiveZone.currentActiveZoneWeatherPreset = preset;
					EnviroSky.instance.NotifyZoneWeatherChanged (preset, Weather.currentActiveZone);
				}
			}
		}

		UpdateClouds (Weather.currentActiveZone.currentActiveZoneWeatherPreset, false);
		UpdateFog (Weather.currentActiveZone.currentActiveZoneWeatherPreset, false);
		UpdateEffectSystems (Weather.currentActiveZone.currentActiveZoneWeatherPrefab, false);
	}

	/// <summary>
	/// Set weather over id with smooth transtion.
	/// </summary>
	public void ChangeWeather (int weatherId)
	{
		if (weatherId < 0 || weatherId > Weather.WeatherPrefabs.Count)
			return;

		if (Weather.WeatherPrefabs[weatherId] != Weather.currentActiveWeatherPrefab)
		{
			Weather.currentActiveZone.currentActiveZoneWeatherPrefab = Weather.WeatherPrefabs[weatherId];
			Weather.currentActiveZone.currentActiveZoneWeatherPreset = Weather.WeatherPrefabs[weatherId].weatherPreset;
			EnviroSky.instance.NotifyZoneWeatherChanged (Weather.WeatherPrefabs[weatherId].weatherPreset, Weather.currentActiveZone);
		}
	}

	/// <summary>
	/// Set weather over name.
	/// </summary>
	public void ChangeWeather (string weatherName)
	{
		for (int i = 0; i < Weather.WeatherPrefabs.Count; i++) {
			if (Weather.WeatherPrefabs [i].weatherPreset.Name == weatherName && Weather.WeatherPrefabs [i] != Weather.currentActiveWeatherPrefab) {
				ChangeWeather (i);
				EnviroSky.instance.NotifyZoneWeatherChanged (Weather.WeatherPrefabs [i].weatherPreset, Weather.currentActiveZone);
			}
		}
	}
		
	/// <summary>
	/// Get Active Weather ID
	/// </summary>
	public int GetActiveWeatherID ()
	{
		for (int i = 0; i < Weather.WeatherPrefabs.Count; i++) 
			{
				if (Weather.WeatherPrefabs [i].weatherPreset == Weather.currentActiveWeatherPreset)
				return i;
			}
		return -1;
	}
		
	/// <summary>
	/// Saves the current time and weather in Playerprefs.
	/// </summary>
	public void Save ()
	{
		PlayerPrefs.SetFloat("Time_Hours",internalHour);
		PlayerPrefs.SetInt("Time_Days",GameTime.Days);
		PlayerPrefs.SetInt("Time_Years",GameTime.Years);
		for (int i = 0; i < Weather.WeatherPrefabs.Count; i++) {
			if(Weather.WeatherPrefabs[i] == Weather.currentActiveWeatherPrefab)
				PlayerPrefs.SetInt("currentWeather",i);
		}
	}

	/// <summary>
	/// Loads the saved time and weather from Playerprefs.
	/// </summary>
	public void Load ()
	{
		if (PlayerPrefs.HasKey ("Time_Hours"))
			internalHour = PlayerPrefs.GetFloat ("Time_Hours");
		if (PlayerPrefs.HasKey ("Time_Days"))
			GameTime.Days = PlayerPrefs.GetInt ("Time_Days");
		if (PlayerPrefs.HasKey ("Time_Years"))
			GameTime.Years = PlayerPrefs.GetInt ("Time_Years");
		if (PlayerPrefs.HasKey ("currentWeather"))
			SetWeatherOverwrite(PlayerPrefs.GetInt("currentWeather"));
	}

	/// <summary>
	/// Set the exact date. by DateTime
	/// </summary>
	public void SetTime(DateTime date)
	{
		GameTime.Years = date.Year;
		GameTime.Days = date.DayOfYear;
		GameTime.Minutes = date.Minute;
		GameTime.Seconds = date.Second;
		GameTime.Hours = date.Hour;
		internalHour = date.Hour + (date.Minute * 0.0166667f) + (date.Second * 0.000277778f);
	}

	/// <summary>
	/// Set the exact date.
	/// </summary>
	public void SetTime(int year, int dayOfYear, int hour, int minute, int seconds)
	{
		GameTime.Years = year;
		GameTime.Days = dayOfYear;
		GameTime.Minutes = minute;
		GameTime.Hours = hour;
		internalHour = hour + (minute * 0.0166667f) + (seconds * 0.000277778f);
	}

	/// <summary>
	/// Set the time of day in hours. (12.5 = 12:30)
	/// </summary>
	public void SetInternalTimeOfDay(float inHours)
	{ 
			internalHour = inHours;
			GameTime.Hours = (int)(inHours);
			inHours -= GameTime.Hours;
			GameTime.Minutes = (int)(inHours * 60f);
			inHours -= GameTime.Minutes * 0.0166667f;
			GameTime.Seconds = (int)(inHours * 3600f);
	}

	/// <summary>
	/// Get current time in a nicely formatted string with seconds!
	/// </summary>
	/// <returns>The time string.</returns>
	public string GetTimeStringWithSeconds ()
	{
		return string.Format ("{0:00}:{1:00}:{2:00}", GameTime.Hours, GameTime.Minutes, GameTime.Seconds);
	}

	/// <summary>
	/// Get current time in a nicely formatted string!
	/// </summary>
	/// <returns>The time string.</returns>
	public string GetTimeString ()
	{
		return string.Format ("{0:00}:{1:00}", GameTime.Hours, GameTime.Minutes);
	}

	/// <summary>
	/// Get current time in hours. UTC0 (12.5 = 12:30)
	/// </summary>
	/// <returns>The the current time of day in hours.</returns>
	public float GetUniversalTimeOfDay()
	{
		return internalHour - GameTime.utcOffset;
	}

	/// <summary>
	/// Calculate total time in hours.
	/// </summary>
	/// <returns>The the current date in hours.</returns>
	public float GetInHours (float hours,float days, float years)
	{
		float inHours  = hours + (days*24f) + ((years * (seasonsSettings.SpringInDays + seasonsSettings.SummerInDays + seasonsSettings.AutumnInDays + seasonsSettings.WinterInDays)) * 24f);
		return inHours;
	}
		
	/// <summary>
	/// Assign your Player and Camera and Initilize.////
	/// </summary>
	public void AssignAndStart (GameObject player, Camera Camera)
	{
		this.Player = player;
		PlayerCamera = Camera;
		Init ();
		started = true;
	}

	/// <summary>
	/// Assign your Player and Camera and Initilize.////
	/// </summary>
	public void StartAsServer ()
	{
		Player = gameObject;
		serverMode = true;
		Init ();
		started = true;
	}

	/// <summary>
	/// Changes focus on other Player or Camera on runtime.////
	/// </summary>
	/// <param name="Player">Player.</param>
	/// <param name="Camera">Camera.</param>
	public void ChangeFocus (GameObject player, Camera Camera)
	{
		this.Player = player;
		RemoveEnviroCameraComponents (PlayerCamera);
		PlayerCamera = Camera;
		InitImageEffects ();
		AddEnviroCloudsRendering ();
	}
	/// <summary>
	/// Destroy all enviro related camera components on this camera.
	/// </summary>
	/// <param name="cam">Cam.</param>
	private void RemoveEnviroCameraComponents (Camera cam)
	{
		EnviroFog oldFog;
		EnviroLightShafts oldSunShafts;
		EnviroLightShafts oldMoonShafts;
		EnviroSkyRendering renderComponent;

		oldFog = cam.GetComponent<EnviroFog> ();
		if (oldFog != null)
			Destroy (oldFog);
		
		oldSunShafts = cam.GetComponent<EnviroLightShafts> (); 
		if(oldSunShafts != null)
			Destroy (oldSunShafts);
		
		oldMoonShafts = cam.GetComponent<EnviroLightShafts> (); 
		if(oldMoonShafts != null)
			Destroy (oldMoonShafts);

		renderComponent = cam.GetComponent<EnviroSkyRendering> (); 
		if(renderComponent != null)
			Destroy (renderComponent);
		
		if (cam.actualRenderingPath == RenderingPath.DeferredShading) {
			UnityEngine.Rendering.CommandBuffer[] cbs;
			cbs = cam.GetCommandBuffers (UnityEngine.Rendering.CameraEvent.BeforeGBuffer);

			for (int i = 0; i < cbs.Length; i++) {

				if (cbs [i].name == "Enviro Sky Rendering")
					cam.RemoveCommandBuffer (UnityEngine.Rendering.CameraEvent.BeforeGBuffer, cbs [i]);
			}

		} else {
			UnityEngine.Rendering.CommandBuffer[] cbs;
			cbs = cam.GetCommandBuffers (UnityEngine.Rendering.CameraEvent.BeforeForwardOpaque);

			for (int i = 0; i < cbs.Length; i++) {

				if (cbs [i].name == "Enviro Sky Rendering")
					cam.RemoveCommandBuffer (UnityEngine.Rendering.CameraEvent.BeforeForwardOpaque, cbs [i]);
			}
		}
	}
}


