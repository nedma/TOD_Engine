using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class HTOD_MinAttribute : PropertyAttribute
{
	public float min;

	public HTOD_MinAttribute(float min)
	{
		this.min = min;
	}
}

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class HTOD_MaxAttribute : PropertyAttribute
{
	public float max;

	public HTOD_MaxAttribute(float max)
	{
		this.max = max;
	}
}

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class HTOD_RangeAttribute : PropertyAttribute
{
	public float min;
	public float max;

	public HTOD_RangeAttribute(float min, float max)
	{
		this.min = min;
		this.max = max;
	}
}
