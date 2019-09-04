using UnityEngine;
using System;

/// Time iteration class.
///
/// Component of the sky dome parent game object.
[ExecuteInEditMode]
public class TOD_Time : MonoBehaviour
{
    /// Day length inspector variable.
    /// Length of one day in minutes.
    public float DayLengthInMinutes = 30;
    /// Date progression inspector variable.
    /// Automatically updates Cycle.Day if enabled.
    public bool ProgressDate = true;

    /// Moon phase progression inspector variable.
    /// Automatically updates Moon.Phase if enabled.
    public bool ProgressMoonPhase = true;

    public bool ProgressTime = true;

    [Range(0, 24)] public float CurrTime = 12.5f;

    private bool isDay = true;

    public bool IsDay
    {
        get { return isDay; }
    }
    private TOD_Sky sky;

    private static TOD_Time instance;
    public static TOD_Time GetInstance()
    {
        if(instance==null)
            instance = FindObjectOfType<TOD_Time>();
        return instance;
    }

    public void SetTime(uint timeInSecond)
    {
        Debug.LogWarning("OnSynTime: " + timeInSecond);

        uint oneDay = (uint)DayLengthInMinutes * 60;
        uint morningTime = (uint)(oneDay * 0.4f);

        //float oneHour = oneDay / 24;
        uint currentTimeInSecond = (timeInSecond + morningTime) % oneDay;
        CurrTime = 24.0f * currentTimeInSecond / oneDay;
    }

	public void SetHour(float _hour)
	{
		CurrTime = _hour;
	}

    public byte MapSkyLight(byte skyLight)
    {
        if (CurrTime >= 0.0f && CurrTime < 4.9f)
        {
            return 0;
        }
        else if (CurrTime >= 4.9f && CurrTime < 20.0f)
        {
            return skyLight;
        }
        else
        {
            return 0;
        }
    }


    protected void Start()
    {
        sky = GetComponent<TOD_Sky>();
    }

    protected void Update()
    {
        float oneDay = DayLengthInMinutes * 60;
        float oneHour = oneDay / 24;

        float hourIter = Time.deltaTime / oneHour;
        float moonIter = Time.deltaTime / (30*oneDay) * 2;

        if (ProgressTime)
        {
            if (CurrTime > 6.0f && CurrTime < 18.0f)
            {
                CurrTime += hourIter;
                isDay = true;
            }
            else
            {
                CurrTime += hourIter;
                isDay = false;
            }
        }

        if (CurrTime >= 24)
        {
            CurrTime = 0;
        }

        sky.Cycle.Hour = CurrTime;

        if (ProgressMoonPhase)
        {
            sky.Cycle.MoonPhase += moonIter;
            if (sky.Cycle.MoonPhase < -1) sky.Cycle.MoonPhase += 2;
            else if (sky.Cycle.MoonPhase > 1) sky.Cycle.MoonPhase -= 2;
        }

        if (sky.Cycle.Hour >= 24)
        {
            sky.Cycle.Hour = 0;

            if (ProgressDate)
            {
                int daysInMonth = DateTime.DaysInMonth(sky.Cycle.Year, sky.Cycle.Month);
                if (++sky.Cycle.Day > daysInMonth)
                {
                    sky.Cycle.Day = 1;
                    if (++sky.Cycle.Month > 12)
                    {
                        sky.Cycle.Month = 1;
                        sky.Cycle.Year++;
                    }
                }
            }
        }
    }
}
