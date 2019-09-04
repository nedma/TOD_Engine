using UnityEngine;
using System;

/// Parameters of the day and night cycle.
[Serializable] public class TOD_CycleParameters
{
    /// [0, 24]
    /// Time of the day in hours.
    /// \n = 0 at the start of the day.
    /// \n = 12 at noon.
    /// \n = 24 at the end of the day.
    public float Hour = 12;

    /// [1, 28-31]
    /// Current day of the month.
    public int Day = 1;

    /// [1, 12]
    /// Current month of the year.
    public int Month = 3;

    /// [1, 9999]
    /// Current year.
    public int Year = 2000;

    /// [-1, 1]
    /// Phase of the moon.
    /// \n = 0 full moon.
    /// \n &plusmn; 1 no moon.
    public float MoonPhase = 0.0f;

    /// [-90, 90]
    /// Latitude of your position in degrees.
    /// \n = -90 at the south pole.
    /// \n = 0 at the equator.
    /// \n = 90 at the north pole.
    public float Latitude = 0f;

    /// [-180, 180]
    /// Longitude of your position in degrees.
    /// \n = -180 at 180 degrees in the west of Greenwich, England.
    /// \n = 0 at Greenwich, England.
    /// \n = 180 at 180 degrees in the east of Greenwich, England.
    public float Longitude = 0f;

    /// UTC/GMT time zone of the current location.
    /// \n = 0 for Greenwich, England.
    public float UTC = 0f;

    /// Assures that all parameters are within a reasonable range.
    public void CheckRange()
    {
        Year  = Mathf.Clamp(Year, 1, 9999);
        Month = Mathf.Clamp(Month, 1, 12);
        Day   = Mathf.Clamp(Day, 1, DateTime.DaysInMonth(Year, Month));
        Hour  = Mathf.Repeat(Hour, 24);

        Longitude = Mathf.Clamp(Longitude, -180, 180);
        Latitude  = Mathf.Clamp(Latitude, -90, 90);
        MoonPhase = Mathf.Clamp(MoonPhase, -1, +1);
    }
}

/// Parameters of the atmosphere.
[Serializable] public class TOD_AtmosphereParameters
{
    /// Artistic value to shift the scattering color of the atmosphere.
    /// Can be used to easily simulate alien worlds.
    public Color ScatteringColor = Color.white;

    public Texture2D ScatteringColorChart;

    /// [0, &infin;]
    /// Intensity of the atmospheric Rayleigh scattering.
    /// Generally speaking this resembles the static scattering.
    public float RayleighMultiplier = 1.0f;

    /// [0, &infin;]
    /// Intensity of the atmospheric Mie scattering.
    /// Generally speaking this resembles the angular scattering.
    public float MieMultiplier = 1.0f;

    /// [0, &infin;]
    /// Brightness of the atmosphere.
    /// This is being applied as a simple multiplier to the output color.
    public float Brightness = 1.0f;

    /// [0, &infin;]
    /// Contrast of the atmosphere.
    /// This is being applied as a power of the output color.
    public float Contrast = 1.0f;

    /// [0, 1]
    /// Directionality factor that determines the size and sharpness of the glow around the light source.
    public float Directionality = 0.5f;

    /// [0, 1]
    /// Intensity of the haziness of the sky at the horizon.
    public float Haziness = 0.5f;

    /// [0, 1]
    /// Density of the fog covering the sky.
    /// This does not affect the RenderSettings fog that is being applied to other objects in the scene.
    public float Fogginess = 0.0f;

    public float CloudBrightness = 1.0f;

    /// <summary>
    ///  fog color to multiply the basic one
    /// </summary>
    public Color FogColorMultiplier = Color.white;


    /// Assures that all parameters are within a reasonable range.
    public void CheckRange()
    {
        MieMultiplier      = Mathf.Max(0, MieMultiplier);
        RayleighMultiplier = Mathf.Max(0, RayleighMultiplier);
        Brightness         = Mathf.Max(0, Brightness);
        Contrast           = Mathf.Max(0, Contrast);
        Directionality     = Mathf.Clamp01(Directionality);
        Haziness           = Mathf.Clamp01(Haziness);
        Fogginess          = Mathf.Clamp01(Fogginess);
    }
}

/// Parameters that are unique to the day.
[Serializable] public class TOD_DayParameters
{
    /// Artistic value for an additive color at day.
    public Color AdditiveColor = Color.black;

    /// Color of the sun material.
    public Color SunMeshColor = new Color32(255, 233, 180, 255);

    /// Color of the light emitted by the sun.
    public Color SunLightColor = new Color32(255, 243, 234, 255);

    /// [0, &infin;]
    /// Size of the sun mesh in degrees.
    public float SunMeshSize = 1.0f;

    /// [0, &infin;]
    /// Intensity of the sun light source.
    public float SunLightIntensity = 0.75f;

    /// [0, 1]
    /// Intensity of the ambient light.
    /// TOD_WorldParameters.SetAmbientLight has to be set for this to have any effect.
    public float AmbientIntensity = 0.75f;

    /// [0, 1]
    /// Opacity of the object shadows dropped by the sun light source
    public float ShadowStrength = 1.0f;

    /// [0, 1]
    /// Sky opacity multiplier at day.
    public float SkyMultiplier = 1.0f;

    /// [0, 1]
    /// Cloud tone multiplier at day.
    public float CloudMultiplier = 1.0f;

    // kyleyi, save a sunpos for MC use
    public Vector3 SunPos;
    public Vector3 MoonPos;

    /// Assures that all parameters are within a reasonable range.
    public void CheckRange()
    {
        SunLightIntensity = Mathf.Max(0, SunLightIntensity);
        SunMeshSize       = Mathf.Max(0, SunMeshSize);
        AmbientIntensity  = Mathf.Clamp01(AmbientIntensity);
        ShadowStrength    = Mathf.Clamp01(ShadowStrength);
        SkyMultiplier     = Mathf.Clamp01(SkyMultiplier);
        CloudMultiplier   = Mathf.Clamp01(CloudMultiplier);
    }
}

/// Parameters that are unique to the night.
[Serializable] public class TOD_NightParameters
{
    /// Artistic value for an additive color at night.
    public Color AdditiveColor = Color.black;

    /// Color of the moon material.
    public Color MoonMeshColor = new Color32(255, 233, 200, 255);

    /// Color of the light emitted by the moon.
    public Color MoonLightColor = new Color32(181, 204, 255, 255);

    /// Color of the moon halo.
    public Color MoonHaloColor = new Color32(81, 104, 155, 255);

    /// [0, &infin;]
    /// Size of the moon mesh in degrees.
    public float MoonMeshSize = 1.0f;

    /// [0, &infin;]
    /// Intensity of the moon light source.
    public float MoonLightIntensity = 0.1f;

    /// [0, 1]
    /// Intensity of the ambient light.
    /// TOD_WorldParameters.SetAmbientLight has to be set for this to have any effect.
    public float AmbientIntensity = 0.2f;

    /// [0, 1]
    /// Opacity of the object shadows dropped by the moon light source
    public float ShadowStrength = 1.0f;

    /// [0, 1]
    /// Sky opacity multiplier at night.
    public float SkyMultiplier = 0.1f;

    /// [0, 1]
    /// Cloud tone multiplier at night.
    public float CloudMultiplier = 0.2f;

    /// Assures that all parameters are within a reasonable range.
    public void CheckRange()
    {
        MoonLightIntensity = Mathf.Max(0, MoonLightIntensity);
        MoonMeshSize       = Mathf.Max(0, MoonMeshSize);
        AmbientIntensity   = Mathf.Clamp01(AmbientIntensity);
        ShadowStrength     = Mathf.Clamp01(ShadowStrength);
        SkyMultiplier      = Mathf.Clamp01(SkyMultiplier);
        CloudMultiplier    = Mathf.Clamp01(CloudMultiplier);
    }
}

/// Parameters of the light source.
[Serializable] public class TOD_LightParameters
{
    /// [0, 1]
    /// Controls how fast the sun color falls off.
    /// This is especially visible during sunset and sunrise.
    public float Falloff = 0.7f;

    /// [0, 1]
    /// Controls how strongly the light color is being affected by sunset and sunrise.
    public float Coloring = 0.5f;

    /// [0, 1]
    /// Controls how strongly the sun color affects the atmosphere color.
    /// This is especially visible during sunset and sunrise.
    public float SkyColoring = 0.5f;

    /// [0, 1]
    /// Controls how strongly the sun color affects the cloud color.
    /// This is especially visible during sunset and sunrise.
    public float CloudColoring = 0.9f;

    /// Assures that all parameters are within a reasonable range.
    public void CheckRange()
    {
        Falloff       = Mathf.Clamp01(Falloff);
        Coloring      = Mathf.Clamp01(Coloring);
        SkyColoring   = Mathf.Clamp01(SkyColoring);
        CloudColoring = Mathf.Clamp01(CloudColoring);
    }
}

[Serializable]
public class TOD_CloudPBRParameters
{
    public float Scale = 3.0f;

    public float UvPow = 0.5f;

    public float Speed = 0.09f;

    public float CloudCover = 0.5f;

    public float Brightness = 1.3f;

    [HideInInspector]
    public float EarthRadius = 21600000.0f;

    [HideInInspector]
    public float CloudHeight = 600.0f;

    [HideInInspector]
    public float AtomosHeight = 30000.0f;

    /// Assures that all parameters are within a reasonable range.
    public void CheckRange()
    {
    }
}

/// Parameters affecting other objects of the world.
[Serializable] public class TOD_WorldParameters
{
    /// Automatically adjust the ambient light color of your render settings.
    public bool SetAmbientLight = false;

    /// Automatically adjust the fog color of your render settingstings
    public bool SetFogColor = false;

    /// [0, 1]
    /// Fog color sampling height.
    /// TOD_WorldParameters.SetFogColor has to be set for this to have any effect.
    /// \n = 0 fog is atmosphere color at horizon.
    /// \n = 1 fog is atmosphere color at zenith.
    public float FogColorBias = 0.0f;

    /// [0, 1]
    /// Relative viewer height in the atmosphere.
    /// \n = 0 on the ground.
    /// \n = 1 at the border of the atmosphere.
    public float ViewerHeight = 0.0f;

    /// [0, 1]
    /// Relative horizon offset.
    /// \n = 0 horizon exactly in the middle of the sky dome sphere.
    /// \n = 1 horizon exactly at the bottom of the sky dome sphere.
    public float HorizonOffset = 0.0f;

    /// Assures that all parameters are within a reasonable range.
    public void CheckRange()
    {
        FogColorBias  = Mathf.Clamp01(FogColorBias);
        ViewerHeight  = Mathf.Clamp01(ViewerHeight);
        HorizonOffset = Mathf.Clamp01(HorizonOffset);
    }
}
