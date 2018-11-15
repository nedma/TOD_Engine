using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(EnviroSky))]
public class EnviroSkyEditor : Editor {


	// GUI Styles
	private GUIStyle boxStyle;
	private GUIStyle boxStyleModified;
	private GUIStyle wrapStyle;
	private GUIStyle headerStyle;
	//Target
	private EnviroSky myTarget;
	private Color modifiedColor;

	//settings
	private bool useWindZone = false;
	private int daysInyear = 365;

	//Profile Properties
	SerializedObject serializedObj;
	SerializedProperty Sun,Moon,Clouds,DirectLight,GlobalReflectionProbe, windZone, LightningGenerator, satellites, starsRotation; 
	SerializedProperty Player,Camera,PlayerTag,CameraTag, AssignOnRuntime, HDR, SkyTag, SatTag;
	SerializedProperty ProgressMode,Years,Days,Hours,Minutes,Seconds,Longitude,Latitude, DayLength,NightLength,UTC, UpdateSeason, CurrentSeason;
	SerializedProperty UpdateWeather,StartWeather, EnableFog,EnableSunShafts,EnableMoonShafts,AmbientVolume,WeatherVolume;
	SerializedProperty lightColorGradient, lightIntensityCurve, shadowStrength;
	SerializedProperty ambientMode, ambientIntensityCurve, ambientSkyGradient, ambientEquatorGradient, ambientGroundGradient;
	SerializedProperty reflectionBool, reflectionIntensity, reflectionUpdate;
	SerializedProperty skyboxMode, customSkyboxMaterial, customSkyboxColor, rayleigh, g, mie, scatteringCurve, scatteringColor, sunMoonPos, sunIntensity, sunDiskScale, sunDiskIntensity, sunDiskColor, moonPhaseMode, moonTexture, moonBrightness,moonGlow, startMoonPhase, currentMoonPhase, skyLuminance, skyColorPower, skyExposure, starsCubemap, starsIntensity;
	SerializedProperty worldScale, fixedAltitude, cloudsAltitude, cloudsRenderQuality, skyColor, sunHighlightColor, moonHighlightColor, lightIntensity;
	SerializedProperty useTag, wetnessAccumulationSpeed,wetnessDryingSpeed, snowAccumulationSpeed,snowMeltingSpeed,cloudTransitionSpeed,fogTransitionSpeed,
					   effectTransitionSpeed, useWindZoneDirection,windTimeScale,windIntensity,windDirectionX,windDirectionY;

	SerializedProperty fogmode, distanceFog, useRadialFog, startDistance, distanceFogIntensity,maximumFogIntensity, heightFog, height, heightFogIntensity, noiseIntensity, noiseScale,fogDitheringScale,fogDitheringIntensity;
	SerializedProperty resolution, screenBlendMode, useDepthTexture, lightShaftsColorSun, lightShaftsColorMoon, treshholdColorSun, treshholdColorMoon, blurRadius, shaftsIntensity, maxRadius;
	SerializedProperty daysInSpring, daysInSummer, daysInAutumn,daysInWinter;
	SerializedProperty enableBGRendering, bgRenderingLayer,bgRenderingDistance, effectQuality, updateInterval;

	SerializedProperty singlePassVR, setCameraClearFlags;
	ReorderableList thunderSFX;

	void OnEnable()
	{
		myTarget = (EnviroSky)target;
		serializedObj = new SerializedObject (myTarget);
		//Components
		Sun = serializedObj.FindProperty ("Components.Sun");
		Moon = serializedObj.FindProperty ("Components.Moon");
		Clouds = serializedObj.FindProperty ("Components.Clouds");
		DirectLight = serializedObj.FindProperty ("Components.DirectLight");
		GlobalReflectionProbe = serializedObj.FindProperty ("Components.GlobalReflectionProbe");
		windZone = serializedObj.FindProperty ("Components.windZone");
		LightningGenerator = serializedObj.FindProperty ("Components.LightningGenerator");
		satellites = serializedObj.FindProperty ("Components.satellites");
		starsRotation = serializedObj.FindProperty ("Components.starsRotation");
		/// Setup
		Player = serializedObj.FindProperty ("Player");
		Camera = serializedObj.FindProperty ("PlayerCamera");
		PlayerTag = serializedObj.FindProperty ("PlayerTag");
		CameraTag = serializedObj.FindProperty ("CameraTag");
		AssignOnRuntime = serializedObj.FindProperty ("AssignInRuntime"); 
		HDR = serializedObj.FindProperty ("HDR"); 
		SkyTag = serializedObj.FindProperty ("skyRenderingLayer"); 
		SatTag = serializedObj.FindProperty ("satelliteRenderingLayer"); 
		singlePassVR = serializedObj.FindProperty ("singlePassVR"); 
		setCameraClearFlags = serializedObj.FindProperty ("setCameraClearFlags"); 
		// Weather Controls
		UpdateWeather = serializedObj.FindProperty ("Weather.updateWeather");
		StartWeather = serializedObj.FindProperty ("Weather.startWeatherPreset");
		//Feature Controls:
		EnableFog = serializedObj.FindProperty ("Fog.AdvancedFog");
		EnableSunShafts = serializedObj.FindProperty ("LightShafts.sunLightShafts");
		EnableMoonShafts = serializedObj.FindProperty ("LightShafts.moonLightShafts");
		// Audio Controls
		AmbientVolume = serializedObj.FindProperty ("Audio.ambientSFXVolume");
		WeatherVolume = serializedObj.FindProperty ("Audio.weatherSFXVolume");
		// Time Controls
		ProgressMode = serializedObj.FindProperty ("GameTime.ProgressTime");
		DayLength = serializedObj.FindProperty ("GameTime.DayLengthInMinutes");
		NightLength = serializedObj.FindProperty ("GameTime.NightLengthInMinutes");
		UpdateSeason = serializedObj.FindProperty ("Seasons.calcSeasons");
		CurrentSeason = serializedObj.FindProperty ("Seasons.currentSeasons");
		Years = serializedObj.FindProperty ("GameTime.Years");
		Days = serializedObj.FindProperty ("GameTime.Days");
		Hours = serializedObj.FindProperty ("GameTime.Hours");
		Minutes = serializedObj.FindProperty ("GameTime.Minutes");
		Seconds = serializedObj.FindProperty ("GameTime.Seconds");
		Longitude = serializedObj.FindProperty ("GameTime.Longitude");
		Latitude = serializedObj.FindProperty ("GameTime.Latitude");
		UTC = serializedObj.FindProperty ("GameTime.utcOffset");
		//Lighting Category
		lightColorGradient = serializedObj.FindProperty ("lightSettings.LightColor");
		lightIntensityCurve = serializedObj.FindProperty ("lightSettings.directLightIntensity");
		shadowStrength = serializedObj.FindProperty ("lightSettings.shadowStrength");
		ambientMode = serializedObj.FindProperty ("lightSettings.ambientMode");
		ambientIntensityCurve = serializedObj.FindProperty ("lightSettings.ambientIntensity");
		ambientSkyGradient = serializedObj.FindProperty ("lightSettings.ambientSkyColor");
		ambientEquatorGradient = serializedObj.FindProperty ("lightSettings.ambientEquatorColor");
		ambientGroundGradient = serializedObj.FindProperty ("lightSettings.ambientGroundColor");
		reflectionBool = serializedObj.FindProperty ("lightSettings.globalReflections");
		reflectionIntensity = serializedObj.FindProperty ("lightSettings.globalReflectionsIntensity");
		reflectionUpdate = serializedObj.FindProperty ("lightSettings.globalReflectionsUpdate");
		//Sky Category
		skyboxMode = serializedObj.FindProperty ("skySettings.skyboxMode");
		customSkyboxMaterial = serializedObj.FindProperty ("skySettings.customSkyboxMaterial");
		customSkyboxColor = serializedObj.FindProperty ("skySettings.customSkyboxColor");
		rayleigh = serializedObj.FindProperty ("skySettings.rayleigh");
		g = serializedObj.FindProperty ("skySettings.g");
		mie = serializedObj.FindProperty ("skySettings.mie");
		scatteringCurve = serializedObj.FindProperty ("skySettings.scatteringCurve");
		scatteringColor = serializedObj.FindProperty ("skySettings.scatteringColor");
		sunMoonPos = serializedObj.FindProperty ("skySettings.sunAndMoonPosition");
		sunIntensity = serializedObj.FindProperty ("skySettings.sunIntensity");
		sunDiskScale = serializedObj.FindProperty ("skySettings.sunDiskScale");
		sunDiskIntensity = serializedObj.FindProperty ("skySettings.sunDiskIntensity");
		sunDiskColor = serializedObj.FindProperty ("skySettings.sunDiskColor");
		moonPhaseMode = serializedObj.FindProperty ("skySettings.moonPhaseMode");
		moonTexture = serializedObj.FindProperty ("skySettings.moonTexture");
		moonBrightness = serializedObj.FindProperty ("skySettings.moonBrightness");
		moonGlow = serializedObj.FindProperty ("skySettings.moonGlow");
		startMoonPhase = serializedObj.FindProperty ("skySettings.startMoonPhase");
		currentMoonPhase = serializedObj.FindProperty ("customMoonPhase");
		skyLuminance = serializedObj.FindProperty ("skySettings.skyLuminence");
		skyColorPower = serializedObj.FindProperty ("skySettings.skyColorPower");
		skyExposure = serializedObj.FindProperty ("skySettings.skyExposure");
		starsCubemap = serializedObj.FindProperty ("skySettings.starsCubeMap");
		starsIntensity = serializedObj.FindProperty ("skySettings.starsIntensity");
		//Clouds Category
		worldScale = serializedObj.FindProperty ("cloudsSettings.worldScale");
		fixedAltitude = serializedObj.FindProperty ("cloudsSettings.FixedAltitude");
		cloudsAltitude = serializedObj.FindProperty ("cloudsSettings.cloudsAltitude");
		cloudsRenderQuality = serializedObj.FindProperty ("cloudsSettings.cloudsRenderResolution");
		skyColor = serializedObj.FindProperty ("cloudsSettings.skyColor");
		sunHighlightColor = serializedObj.FindProperty ("cloudsSettings.sunHighlightColor");
		moonHighlightColor = serializedObj.FindProperty ("cloudsSettings.moonHighlightColor");
		lightIntensity = serializedObj.FindProperty ("cloudsSettings.lightIntensity");
		// Weather Category
		useTag = serializedObj.FindProperty ("weatherSettings.useTag");
		wetnessAccumulationSpeed = serializedObj.FindProperty ("weatherSettings.wetnessAccumulationSpeed");
		wetnessDryingSpeed = serializedObj.FindProperty ("weatherSettings.wetnessDryingSpeed");
		snowAccumulationSpeed = serializedObj.FindProperty ("weatherSettings.snowAccumulationSpeed");
		snowMeltingSpeed = serializedObj.FindProperty ("weatherSettings.snowMeltingSpeed");
		cloudTransitionSpeed = serializedObj.FindProperty ("weatherSettings.cloudTransitionSpeed");
		fogTransitionSpeed = serializedObj.FindProperty ("weatherSettings.fogTransitionSpeed");
		effectTransitionSpeed = serializedObj.FindProperty ("weatherSettings.effectTransitionSpeed");
		useWindZoneDirection = serializedObj.FindProperty ("cloudsSettings.useWindZoneDirection");
		windTimeScale = serializedObj.FindProperty ("cloudsSettings.cloudsTimeScale");
		windIntensity = serializedObj.FindProperty ("cloudsSettings.cloudsWindStrengthModificator");
		windDirectionX = serializedObj.FindProperty ("cloudsSettings.cloudsWindDirectionX");
		windDirectionY = serializedObj.FindProperty ("cloudsSettings.cloudsWindDirectionY");
		fogmode = serializedObj.FindProperty ("fogSettings.Fogmode");
		distanceFog = serializedObj.FindProperty ("fogSettings.distanceFog");
		useRadialFog = serializedObj.FindProperty ("fogSettings.useRadialDistance");
		startDistance = serializedObj.FindProperty ("fogSettings.startDistance");
		distanceFogIntensity = serializedObj.FindProperty ("fogSettings.distanceFogIntensity");
		maximumFogIntensity = serializedObj.FindProperty ("fogSettings.maximumFogDensity");
		heightFog = serializedObj.FindProperty ("fogSettings.heightFog");
		height = serializedObj.FindProperty ("fogSettings.height");
		heightFogIntensity = serializedObj.FindProperty ("fogSettings.heightFogIntensity");
		noiseIntensity = serializedObj.FindProperty ("fogSettings.noiseIntensity");
		noiseScale = serializedObj.FindProperty ("fogSettings.noiseScale");
		fogDitheringScale = serializedObj.FindProperty ("fogSettings.fogDitheringScale");
		fogDitheringIntensity = serializedObj.FindProperty ("fogSettings.fogDitheringIntensity");
		//LightShafts
		resolution = serializedObj.FindProperty ("lightshaftsSettings.resolution");
		screenBlendMode = serializedObj.FindProperty ("lightshaftsSettings.screenBlendMode");
		useDepthTexture = serializedObj.FindProperty ("lightshaftsSettings.useDepthTexture");
		lightShaftsColorSun = serializedObj.FindProperty ("lightshaftsSettings.lightShaftsColorSun");
		lightShaftsColorMoon = serializedObj.FindProperty ("lightshaftsSettings.lightShaftsColorMoon");
		treshholdColorSun = serializedObj.FindProperty ("lightshaftsSettings.thresholdColorSun");
		treshholdColorMoon = serializedObj.FindProperty ("lightshaftsSettings.thresholdColorMoon");
		blurRadius = serializedObj.FindProperty ("lightshaftsSettings.blurRadius");
		shaftsIntensity = serializedObj.FindProperty ("lightshaftsSettings.intensity");
		maxRadius = serializedObj.FindProperty ("lightshaftsSettings.maxRadius");
		//Season
		daysInSpring = serializedObj.FindProperty ("seasonsSettings.SpringInDays");
		daysInSummer = serializedObj.FindProperty ("seasonsSettings.SummerInDays");
		daysInAutumn = serializedObj.FindProperty ("seasonsSettings.AutumnInDays");
		daysInWinter = serializedObj.FindProperty ("seasonsSettings.WinterInDays");
		//Background Rendering
		enableBGRendering= serializedObj.FindProperty ("backgroundSettings.backgroundRendering");
		bgRenderingLayer= serializedObj.FindProperty ("backgroundSettings.backgroundLayer");
		bgRenderingDistance= serializedObj.FindProperty ("backgroundSettings.backgroundViewDistance");
		//Quality
		effectQuality= serializedObj.FindProperty ("qualitySettings.GlobalParticleEmissionRates");
		updateInterval= serializedObj.FindProperty ("qualitySettings.UpdateInterval");
		//Audio
		thunderSFX = new ReorderableList(serializedObject, 
			serializedObject.FindProperty("audioSettings.ThunderSFX"), 
			true, true, true, true);

		thunderSFX.drawHeaderCallback = (Rect rect) =>
		{
			EditorGUI.LabelField(rect, "Thunder SFX");
		};
		thunderSFX.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) =>
		{
			var element = thunderSFX.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += 2;
			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y, Screen.width*.8f, EditorGUIUtility.singleLineHeight),
				element, GUIContent.none);
		};
		thunderSFX.onAddCallback = (ReorderableList l) =>
		{
			var index = l.serializedProperty.arraySize;
			l.serializedProperty.arraySize++;
			l.index = index;
			var element = l.serializedProperty.GetArrayElementAtIndex(index);
		};

		modifiedColor = Color.red;
		modifiedColor.a = 0.5f;
		////
	}
	/// <summary>
	/// Applies the changes and set profile to modifed but not saved.
	/// </summary>
	private void ApplyChanges ()
	{
		if (EditorGUI.EndChangeCheck ()) {
			serializedObj.ApplyModifiedProperties ();
			myTarget.profile.modified = true;
		}
	}

	public override void OnInspectorGUI ()
	{
		myTarget = (EnviroSky)target;
		daysInyear = (int)(myTarget.seasonsSettings.SpringInDays + myTarget.seasonsSettings.SummerInDays + myTarget.seasonsSettings.AutumnInDays + myTarget.seasonsSettings.WinterInDays);
		//Set up the box style
		if (boxStyle == null)
		{
			boxStyle = new GUIStyle(GUI.skin.box);
			boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
			boxStyle.fontStyle = FontStyle.Bold;
			boxStyle.alignment = TextAnchor.UpperLeft;
		}

		if (boxStyleModified == null)
		{
			boxStyleModified = new GUIStyle(GUI.skin.box);
			boxStyleModified.normal.textColor = GUI.skin.label.normal.textColor;
			boxStyleModified.fontStyle = FontStyle.Bold;
			boxStyleModified.alignment = TextAnchor.UpperLeft;
		}
			
		//Setup the wrap style
		if (wrapStyle == null)
		{
			wrapStyle = new GUIStyle(GUI.skin.label);
			wrapStyle.fontStyle = FontStyle.Bold;
			wrapStyle.wordWrap = true;
		}

		if (headerStyle == null) {
			headerStyle = new GUIStyle(GUI.skin.label);
			headerStyle.fontStyle = FontStyle.Bold;
			headerStyle.alignment = TextAnchor.UpperLeft;
		}
			
		GUILayout.BeginVertical("Enviro - Sky and Weather 1.9.0", boxStyle);
		GUILayout.Space(20);
		GUILayout.BeginVertical("Profile", boxStyle);
		GUILayout.Space(20);
		myTarget.profile = (EnviroProfile)EditorGUILayout.ObjectField (myTarget.profile, typeof(EnviroProfile), false);
		GUILayout.Space(10);
		if (myTarget.profile != null)
			EditorGUILayout.LabelField ("Profile Version:", myTarget.profile.version);
		else
			EditorGUILayout.LabelField ("No Profile Assigned!");
		EditorGUILayout.LabelField ("Prefab Version:", myTarget.prefabVersion);
		GUILayout.Space(10);
		// Runtime Settings
		if (GUILayout.Button ("Apply all Settings")) {
			myTarget.enabled = false;
			myTarget.enabled = true;
		}
		GUILayout.EndHorizontal ();
		if (myTarget.profile != null) {
		if (myTarget.profile.modified) // Change color when modified
			GUI.backgroundColor = modifiedColor;
		GUILayout.BeginVertical ("", boxStyle);
		if(myTarget.profile.modified)
			GUI.backgroundColor = Color.white;
			
			serializedObj.UpdateIfDirtyOrScript ();
			myTarget.showSettings = EditorGUILayout.BeginToggleGroup (" Edit Profile", myTarget.showSettings);
			if (myTarget.showSettings) {
				GUILayout.BeginVertical ("", boxStyle);
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Save to Profile")) {
					myTarget.SaveProfile ();
					myTarget.profile.modified = false;
				}
				if (GUILayout.Button ("Load from Profile")) {
					myTarget.ApplyProfile (myTarget.profile);
					myTarget.profile.modified = false;
					serializedObj.UpdateIfDirtyOrScript ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.Space (10);
				EditorGUILayout.LabelField ("Category", headerStyle);
				myTarget.profile.viewMode =	(EnviroProfile.settingsMode)EditorGUILayout.EnumPopup (myTarget.profile.viewMode);
				GUILayout.EndVertical ();
			
				switch (myTarget.profile.viewMode) {
				case EnviroProfile.settingsMode.Lighting:
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (lightColorGradient, true, null);
					EditorGUILayout.PropertyField (lightIntensityCurve, true, null);
					EditorGUILayout.PropertyField (shadowStrength, true, null);
					EditorGUILayout.PropertyField (ambientMode, true, null);
					EditorGUILayout.PropertyField (ambientIntensityCurve, true, null);
					EditorGUILayout.PropertyField (ambientSkyGradient, true, null);
					EditorGUILayout.PropertyField (ambientEquatorGradient, true, null);
					EditorGUILayout.PropertyField (ambientGroundGradient, true, null);
					EditorGUILayout.PropertyField (reflectionBool, true, null);
					EditorGUILayout.PropertyField (reflectionIntensity, true, null);
					EditorGUILayout.PropertyField (reflectionUpdate, true, null);
					ApplyChanges ();
					break;
				case EnviroProfile.settingsMode.Sky:
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (skyboxMode, true, null);
					EditorGUILayout.PropertyField (customSkyboxMaterial, true, null);
					EditorGUILayout.PropertyField (customSkyboxColor, true, null);
					GUILayout.Space (10);
					EditorGUILayout.LabelField ("Scattering", headerStyle);
					myTarget.skySettings.waveLength = EditorGUILayout.Vector3Field ("Wave Length", myTarget.skySettings.waveLength);
					EditorGUILayout.PropertyField (rayleigh, true, null);
					EditorGUILayout.PropertyField (g, true, null);
					EditorGUILayout.PropertyField (mie, true, null);
					EditorGUILayout.PropertyField (scatteringCurve, true, null);
					EditorGUILayout.PropertyField (scatteringColor, true, null);
					EditorGUILayout.PropertyField (sunMoonPos, true, null);
					EditorGUILayout.PropertyField (sunIntensity, true, null);
					EditorGUILayout.PropertyField (sunDiskScale, true, null);
					EditorGUILayout.PropertyField (sunDiskIntensity, true, null);
					EditorGUILayout.PropertyField (sunDiskColor, true, null);
					EditorGUILayout.PropertyField (moonPhaseMode, true, null);
					EditorGUILayout.PropertyField (moonTexture, true, null);
					EditorGUILayout.PropertyField (moonBrightness, true, null);
					EditorGUILayout.PropertyField (moonGlow, true, null);
					if (myTarget.skySettings.moonPhaseMode == EnviroSkySettings.MoonPhases.Custom) {
						EditorGUILayout.PropertyField (startMoonPhase, true, null);
						EditorGUILayout.PropertyField (currentMoonPhase, true, null);
					}
					EditorGUILayout.PropertyField (skyLuminance, true, null);
					EditorGUILayout.PropertyField (skyColorPower, true, null);
					EditorGUILayout.PropertyField (skyExposure, true, null);
					EditorGUILayout.PropertyField (starsCubemap, true, null);
					EditorGUILayout.PropertyField (starsIntensity, true, null);
					ApplyChanges ();
					break;
				// CLouds Category
				case EnviroProfile.settingsMode.Clouds:	
					GUILayout.BeginVertical (" Layer Setup", boxStyle);
					GUILayout.Space (20);
					if (GUILayout.Button ("Add Layer")) {
						myTarget.cloudsSettings.cloudsLayers.Add (new EnviroCloudsLayerVariables ());
						myTarget.showCloudLayer.Add (true);
					}
			
					if (GUILayout.Button ("Apply Changes")) {
						myTarget.InitClouds ();
					}
					for (int i = 0; i < myTarget.cloudsSettings.cloudsLayers.Count; i++) {
						GUILayout.BeginVertical ("", boxStyle);
						myTarget.showCloudLayer [i] = EditorGUILayout.BeginToggleGroup (myTarget.cloudsSettings.cloudsLayers [i].Name, myTarget.showCloudLayer [i]);
						if (myTarget.showCloudLayer [i]) {
							GUILayout.Space (10);
							myTarget.cloudsSettings.cloudsLayers [i].Name = EditorGUILayout.TextField ("Layer Name", myTarget.cloudsSettings.cloudsLayers [i].Name);
							GUILayout.Space (10);
							myTarget.cloudsSettings.cloudsLayers [i].Quality = EditorGUILayout.IntSlider ("Quality", myTarget.cloudsSettings.cloudsLayers [i].Quality, 5, 100);
							myTarget.cloudsSettings.cloudsLayers [i].segmentCount = EditorGUILayout.IntSlider ("Segments", myTarget.cloudsSettings.cloudsLayers [i].segmentCount, 4, 16);
							myTarget.cloudsSettings.cloudsLayers [i].thickness = EditorGUILayout.Slider ("Thickness", myTarget.cloudsSettings.cloudsLayers [i].thickness, 0.001f, 0.1f);
							myTarget.cloudsSettings.cloudsLayers [i].curved = EditorGUILayout.Toggle ("Curved", myTarget.cloudsSettings.cloudsLayers [i].curved);
							myTarget.cloudsSettings.cloudsLayers [i].curvedIntensity = EditorGUILayout.Slider ("Curved Intensity", myTarget.cloudsSettings.cloudsLayers [i].curvedIntensity, 0.001f, 0.5f);
							myTarget.cloudsSettings.cloudsLayers [i].Scaling = EditorGUILayout.Slider ("Scaling", myTarget.cloudsSettings.cloudsLayers [i].Scaling, 0.5f, 2f);
							GUILayout.Space (10);
							myTarget.cloudsSettings.cloudsLayers [i].myCloudsTexture = (Texture)EditorGUILayout.ObjectField ("Texture", myTarget.cloudsSettings.cloudsLayers [i].myCloudsTexture, typeof(Texture), true);	
							myTarget.cloudsSettings.cloudsLayers [i].canCastShadows = EditorGUILayout.Toggle ("Cast Shadows", myTarget.cloudsSettings.cloudsLayers [i].canCastShadows);
							GUILayout.Space (10);
							myTarget.cloudsSettings.cloudsLayers [i].layerAltitude = EditorGUILayout.FloatField ("Layer Altitude", myTarget.cloudsSettings.cloudsLayers [i].layerAltitude);
							myTarget.cloudsSettings.cloudsLayers [i].LayerOffset = EditorGUILayout.FloatField ("Layer Offset", myTarget.cloudsSettings.cloudsLayers [i].LayerOffset);
							if (GUILayout.Button ("Remove")) {
								myTarget.cloudsSettings.cloudsLayers.Remove (myTarget.cloudsSettings.cloudsLayers [i]);
								if (myTarget.cloudsLayers.Count > i)
									myTarget.cloudsLayers.RemoveAt (i);
							
								myTarget.showCloudLayer.RemoveAt (i);

							}
						}
						EditorGUILayout.EndToggleGroup ();
						GUILayout.EndVertical ();
					}
					serializedObj.Update ();
					GUILayout.EndVertical ();
					GUILayout.Space (10);
					GUILayout.BeginVertical ("Clouds Wind Animation",headerStyle);
					GUILayout.Space (15);
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (useWindZoneDirection, true, null);
					EditorGUILayout.PropertyField (windTimeScale, true, null);
					EditorGUILayout.PropertyField (windIntensity, true, null);
					if (useWindZoneDirection.boolValue == false) {
						EditorGUILayout.PropertyField (windDirectionX, true, null);
						EditorGUILayout.PropertyField (windDirectionY, true, null);
					}
					GUILayout.EndVertical ();


					EditorGUILayout.PropertyField (worldScale, true, null);
					EditorGUILayout.PropertyField (fixedAltitude, true, null);
					EditorGUILayout.PropertyField (cloudsAltitude, true, null);
					EditorGUILayout.PropertyField (cloudsRenderQuality, true, null);
					EditorGUILayout.PropertyField (skyColor, true, null);
					EditorGUILayout.PropertyField (sunHighlightColor, true, null);
					EditorGUILayout.PropertyField (moonHighlightColor, true, null);
					EditorGUILayout.PropertyField (lightIntensity, true, null);
					ApplyChanges ();
					break;

				case EnviroProfile.settingsMode.Weather:
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (useTag, true, null);
					EditorGUILayout.PropertyField (wetnessAccumulationSpeed, true, null);
					EditorGUILayout.PropertyField (wetnessDryingSpeed, true, null);
					EditorGUILayout.PropertyField (snowAccumulationSpeed, true, null);
					EditorGUILayout.PropertyField (snowMeltingSpeed, true, null);
					EditorGUILayout.PropertyField (cloudTransitionSpeed, true, null);
					EditorGUILayout.PropertyField (fogTransitionSpeed, true, null);
					EditorGUILayout.PropertyField (effectTransitionSpeed, true, null);
					ApplyChanges ();
					break;

				case EnviroProfile.settingsMode.Season:
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (daysInSpring, true, null);
					EditorGUILayout.PropertyField (daysInSummer, true, null);
					EditorGUILayout.PropertyField (daysInAutumn, true, null);
					EditorGUILayout.PropertyField (daysInWinter, true, null);
					ApplyChanges ();
					break;

				case EnviroProfile.settingsMode.Fog:
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (fogmode, true, null);
					EditorGUILayout.PropertyField (distanceFog, true, null);
					EditorGUILayout.PropertyField (useRadialFog, true, null);
					EditorGUILayout.PropertyField (startDistance, true, null);
					EditorGUILayout.PropertyField (distanceFogIntensity, true, null);
					EditorGUILayout.PropertyField (maximumFogIntensity, true, null);
					EditorGUILayout.PropertyField (heightFog, true, null);
					EditorGUILayout.PropertyField (height, true, null);
					EditorGUILayout.PropertyField (heightFogIntensity, true, null);
					EditorGUILayout.PropertyField (noiseIntensity, true, null);
					EditorGUILayout.PropertyField (noiseScale, true, null);
					myTarget.fogSettings.heightFogVelocity = EditorGUILayout.Vector2Field ("Height Fog Velocity", myTarget.fogSettings.heightFogVelocity);
					EditorGUILayout.PropertyField (fogDitheringScale, true, null);
					EditorGUILayout.PropertyField (fogDitheringIntensity, true, null);
					ApplyChanges ();
					break;



				case EnviroProfile.settingsMode.Lightshafts:
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (resolution, true, null);
					EditorGUILayout.PropertyField (screenBlendMode, true, null);
					EditorGUILayout.PropertyField (useDepthTexture, true, null);
					EditorGUILayout.PropertyField (lightShaftsColorSun, true, null);
					EditorGUILayout.PropertyField (lightShaftsColorMoon, true, null);
					EditorGUILayout.PropertyField (treshholdColorSun, true, null);
					EditorGUILayout.PropertyField (treshholdColorMoon, true, null);
					EditorGUILayout.PropertyField (blurRadius, true, null);
					EditorGUILayout.PropertyField (shaftsIntensity, true, null);
					EditorGUILayout.PropertyField (maxRadius, true, null);
					ApplyChanges ();
					break;

				case EnviroProfile.settingsMode.Audio:
					myTarget.Audio.SFXHolderPrefab = (GameObject)EditorGUILayout.ObjectField ("SFX Prefab:", myTarget.Audio.SFXHolderPrefab, typeof(GameObject), false);
					serializedObject.Update ();
					thunderSFX.DoLayoutList ();
					serializedObject.ApplyModifiedProperties ();
					break;

				case EnviroProfile.settingsMode.Satellites:
					GUILayout.BeginVertical (" Layer Setup", boxStyle);
					GUILayout.Space (20);
					if (GUILayout.Button ("Add Satellite")) {
						myTarget.satelliteSettings.additionalSatellites.Add (new EnviroSatellite ());
					}

					if (GUILayout.Button ("Apply Changes")) {
							myTarget.CheckSatellites ();
					}
					for (int i = 0; i < myTarget.satelliteSettings.additionalSatellites.Count; i++) {
						GUILayout.BeginVertical ("", boxStyle);
							GUILayout.Space (10);
							myTarget.satelliteSettings.additionalSatellites [i].name = EditorGUILayout.TextField ("Name", myTarget.satelliteSettings.additionalSatellites [i].name);
							GUILayout.Space (10);
							myTarget.satelliteSettings.additionalSatellites [i].prefab = (GameObject)EditorGUILayout.ObjectField ("Prefab", myTarget.satelliteSettings.additionalSatellites [i].prefab, typeof(GameObject), false);
							myTarget.satelliteSettings.additionalSatellites [i].orbit = EditorGUILayout.Slider ("OrbitDistance", myTarget.satelliteSettings.additionalSatellites [i].orbit,0f,myTarget.transform.localScale.y);
							myTarget.satelliteSettings.additionalSatellites [i].xRot = EditorGUILayout.Slider ("XRot", myTarget.satelliteSettings.additionalSatellites [i].xRot,0f,360f);
						myTarget.satelliteSettings.additionalSatellites [i].yRot = EditorGUILayout.Slider ("YRot", myTarget.satelliteSettings.additionalSatellites [i].yRot,0f,360f);
						if (GUILayout.Button ("Remove")) 
						{
							myTarget.satelliteSettings.additionalSatellites.Remove (myTarget.satelliteSettings.additionalSatellites [i]);
							myTarget.CheckSatellites ();
						}
						GUILayout.EndVertical ();
					}
					serializedObj.Update ();
					GUILayout.EndVertical ();
				break;

				case EnviroProfile.settingsMode.Background:
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (enableBGRendering, true, null);
					bgRenderingLayer.intValue = EditorGUILayout.LayerField ("Background Layer", bgRenderingLayer.intValue);
					EditorGUILayout.PropertyField (bgRenderingDistance, true, null);
					if (EditorGUI.EndChangeCheck ()) {
						myTarget.ReInit ();
						serializedObj.ApplyModifiedProperties ();
						myTarget.profile.modified = true;
					}
					break;

				case EnviroProfile.settingsMode.Quality:
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (effectQuality, true, null);
					EditorGUILayout.PropertyField (updateInterval, true, null);
					ApplyChanges ();
					break;
				}
			}
			GUILayout.EndVertical ();
			EditorGUILayout.EndToggleGroup ();
		}
		GUILayout.EndVertical ();

		if (myTarget.profile != null) {
			EditorGUI.BeginChangeCheck ();
			// Begin Setup
			GUILayout.BeginVertical ("", boxStyle);
			// Player Setup
			GUILayout.BeginVertical ("", boxStyle);
				myTarget.profile.showPlayerSetup = EditorGUILayout.BeginToggleGroup ("Player & Camera Setup", myTarget.profile.showPlayerSetup);
			if (myTarget.profile.showPlayerSetup) {
				GUILayout.Space (20);
				EditorGUILayout.PropertyField (Player, true, null);
				EditorGUILayout.PropertyField (Camera, true, null);
				GUILayout.Space (20);
				AssignOnRuntime.boolValue = EditorGUILayout.BeginToggleGroup ("Assign On Runtime", AssignOnRuntime.boolValue);
				PlayerTag.stringValue = EditorGUILayout.TagField ("Player Tag", PlayerTag.stringValue);
				CameraTag.stringValue = EditorGUILayout.TagField ("Camera Tag", CameraTag.stringValue);
				EditorGUILayout.EndToggleGroup ();
			}
			EditorGUILayout.EndToggleGroup ();
			GUILayout.EndVertical ();

			/// Render Setup
			GUILayout.BeginVertical ("", boxStyle);
			myTarget.profile.showRenderingSetup = EditorGUILayout.BeginToggleGroup ("Rendering Setup", myTarget.profile.showRenderingSetup);
			if (myTarget.profile.showRenderingSetup) {
				EditorGUILayout.PropertyField (HDR, true, null);
				EditorGUILayout.PropertyField (setCameraClearFlags, true, null);
				GUILayout.Space (10);
				EditorGUILayout.LabelField ("Layer Setup",headerStyle,null);
				SatTag.intValue = EditorGUILayout.LayerField ("Sky Layer", SatTag.intValue);
				SkyTag.intValue = EditorGUILayout.LayerField ("Satellite Layer", SkyTag.intValue);
				EditorGUILayout.PropertyField (singlePassVR, true, null);
			}
			EditorGUILayout.EndToggleGroup ();
			GUILayout.EndVertical ();

			/// Components Setup
			GUILayout.BeginVertical ("", boxStyle);
			myTarget.profile.showComponentsSetup = EditorGUILayout.BeginToggleGroup ("Component Setup", myTarget.profile.showComponentsSetup);
			if (myTarget.profile.showComponentsSetup) 
			{
				EditorGUILayout.PropertyField (Sun, true, null);
				EditorGUILayout.PropertyField (Moon, true, null);
				EditorGUILayout.PropertyField (Clouds, true, null);
				EditorGUILayout.PropertyField (windZone, true, null);
				EditorGUILayout.PropertyField (GlobalReflectionProbe, true, null);
				EditorGUILayout.PropertyField (satellites, true, null);
				EditorGUILayout.PropertyField (starsRotation, true, null);
			}
			EditorGUILayout.EndToggleGroup ();
			GUILayout.EndVertical ();
			GUILayout.EndVertical ();

	

			////////////
			// Begin Controls
			GUILayout.BeginVertical ("", boxStyle);
			// Time Control
			GUILayout.BeginVertical ("", boxStyle);
			myTarget.profile.showTimeUI = EditorGUILayout.BeginToggleGroup ("Time and Location Controls", myTarget.profile.showTimeUI);
			if (myTarget.profile.showTimeUI) {
				GUILayout.Space (20);
				GUILayout.BeginVertical ("Time", boxStyle);
				GUILayout.Space (20);
				EditorGUILayout.PropertyField (ProgressMode, true, null);
				GUILayout.Space (20);
				EditorGUILayout.PropertyField (Seconds, true, null);
				EditorGUILayout.PropertyField (Minutes, true, null);
				EditorGUILayout.PropertyField (Hours, true, null);
				EditorGUILayout.PropertyField (Days, true, null);
				EditorGUILayout.PropertyField (Years, true, null);
				GUILayout.Space (10);
				EditorGUILayout.PropertyField (DayLength, true, null);
				EditorGUILayout.PropertyField (NightLength, true, null);
				GUILayout.EndVertical ();
				GUILayout.BeginVertical ("Season", boxStyle);
				GUILayout.Space (20);
				EditorGUILayout.PropertyField (UpdateSeason, true, null);
				EditorGUILayout.PropertyField (CurrentSeason, true, null);
				GUILayout.EndVertical ();
				GUILayout.BeginVertical ("Location", boxStyle);
				GUILayout.Space (20);
				EditorGUILayout.PropertyField (UTC, true, null);
				GUILayout.Space (10);
				EditorGUILayout.PropertyField (Latitude, true, null);
				EditorGUILayout.PropertyField (Longitude, true, null);
				GUILayout.EndVertical ();
			}
			EditorGUILayout.EndToggleGroup ();
			GUILayout.EndVertical ();
			// Time End
			// Weather Control
			GUILayout.BeginVertical ("", boxStyle);
			myTarget.profile.showWeatherUI = EditorGUILayout.BeginToggleGroup ("Weather Controls", myTarget.profile.showWeatherUI);
			if (myTarget.profile.showWeatherUI) {
				EditorGUILayout.PropertyField (UpdateWeather, true, null);
				GUILayout.BeginVertical ("Weather", boxStyle);
				GUILayout.Space (20);
				EditorGUILayout.PropertyField (StartWeather, true, null);
				GUILayout.Space (15);
				if (Application.isPlaying) {
					if (myTarget.Weather.weatherPresets.Count > 0) {
						GUIContent[] zonePrefabs = new GUIContent[myTarget.Weather.weatherPresets.Count];
						for (int idx = 0; idx < zonePrefabs.Length; idx++) {
							zonePrefabs [idx] = new GUIContent (myTarget.Weather.weatherPresets [idx].Name);
						}
						int weatherID = EditorGUILayout.Popup (new GUIContent ("Current Weather"), myTarget.GetActiveWeatherID (), zonePrefabs);
						myTarget.ChangeWeather (weatherID);
					}
				} else
					EditorGUILayout.LabelField ("Weather can only be changed in runtime!");
			
				if (GUILayout.Button ("Edit current Weather Preset")) {
					if(myTarget.Weather.currentActiveWeatherPreset != null)
						Selection.activeObject = myTarget.Weather.currentActiveWeatherPreset;
					else if(myTarget.Weather.startWeatherPreset != null)
						Selection.activeObject = myTarget.Weather.startWeatherPreset;
				}
				GUILayout.EndVertical ();
				GUILayout.BeginVertical ("Zones", boxStyle);
				GUILayout.Space (20);
				myTarget.Weather.currentActiveZone = (EnviroZone)EditorGUILayout.ObjectField ("Current Zone", myTarget.Weather.currentActiveZone, typeof(EnviroZone), true);
				GUILayout.EndVertical ();
			}
			EditorGUILayout.EndToggleGroup ();
			GUILayout.EndVertical ();
			// Weather End
			// Effects Control
			GUILayout.BeginVertical ("", boxStyle);
			myTarget.profile.showEffectsUI = EditorGUILayout.BeginToggleGroup ("Feature Controls", myTarget.profile.showEffectsUI);
			if (myTarget.profile.showEffectsUI) {
				EditorGUILayout.PropertyField (EnableFog, true, null);
				EditorGUILayout.PropertyField (EnableSunShafts, true, null);
				EditorGUILayout.PropertyField (EnableMoonShafts, true, null);
			}
			EditorGUILayout.EndToggleGroup ();
			GUILayout.EndVertical ();
			// Effects End
			// Audio Control
			GUILayout.BeginVertical ("", boxStyle);
			myTarget.profile.showAudioUI = EditorGUILayout.BeginToggleGroup ("Audio Controls", myTarget.profile.showAudioUI);
			if (myTarget.profile.showAudioUI) {
				GUILayout.BeginVertical ("", boxStyle);
				EditorGUILayout.PropertyField (AmbientVolume, true, null);
				EditorGUILayout.PropertyField (WeatherVolume, true, null);
				GUILayout.EndVertical ();
			}
			EditorGUILayout.EndToggleGroup ();
			GUILayout.EndVertical ();
			// Audio End
			/////////////
			if (EditorGUI.EndChangeCheck ())
				serializedObj.ApplyModifiedProperties ();
			EditorGUILayout.EndVertical ();
		} else {
			GUILayout.BeginVertical ("", boxStyle);
			EditorGUILayout.LabelField ("No profile assigned!");
			if (GUILayout.Button ("Create and assign new profile!")) {
				myTarget.profile = EnviroProfileCreation.CreateNewEnviroProfile ();
				myTarget.ApplyProfile (myTarget.profile);
				myTarget.ReInit ();
			}
			GUILayout.EndVertical ();
		}
		EditorUtility.SetDirty (target);
	}
}
