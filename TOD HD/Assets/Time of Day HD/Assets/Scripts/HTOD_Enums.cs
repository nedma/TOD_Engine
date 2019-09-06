using UnityEngine;

/// Moon position types.
public enum HTOD_MoonPositionType
{
	OppositeToSun,
	Realistic
}

/// Stars position types.
public enum HTOD_StarsPositionType
{
	Static,
	Rotating
}

/// Fog adjustment types.
public enum HTOD_FogType
{
	None,
	Atmosphere,
	Directional,
	Gradient
}

/// Ambient light types.
public enum HTOD_AmbientType
{
	None,
	Color,
	Gradient,
	Spherical
}

/// Reflection cubemap types.
public enum HTOD_ReflectionType
{
	None,
	Cubemap
}

/// Color space types.
public enum HTOD_ColorSpaceType
{
	Auto,
	Linear,
	Gamma
}

/// Dynamic color range types.
public enum HTOD_ColorRangeType
{
	Auto,
	HDR,
	LDR
}

/// Color output types.
public enum HTOD_ColorOutputType
{
	Raw,
	Dithered
}

/// Cloud rendering qualities.
public enum HTOD_CloudQualityType
{
	Low,
	Medium,
	High
}

/// Mesh vertex count levels.
public enum HTOD_MeshQualityType
{
	Low,
	Medium,
	High
}

/// Star count qualities.
public enum HTOD_StarQualityType
{
	Low,
	Medium,
	High
}

/// Sky rendering qualities.
public enum HTOD_SkyQualityType
{
	PerVertex,
	PerPixel
}
