using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Enviro/Utility/Surface Animator Multiple")]
public class EnviroSurfaceAnimatorMultiple : MonoBehaviour
{
	[HideInInspector]
	public bool isRaining;

	[HideInInspector]
    public MeshRenderer[] mRenderer;
	public bool useRipples;
	public Texture2D[] RippleFrames;
	public float RippleAnimationSpeed = 30f;
	public bool useWaveAnimation;
	public Vector2 waveSpeed;
	 
	private List<Material> mats = new List<Material>();
	private float curWaveSpeedx;
	private float curWaveSpeedy;

	private float curWetness = 0f;
	private float curSnow = 0f;
    bool gotcha = false;

    public void Start()
    {
            mRenderer = GetComponentsInChildren<MeshRenderer> ();
    }
	
	void GetParameters ()
	{
		curWetness = EnviroSky.instance.Weather.wetness;
		curSnow = EnviroSky.instance.Weather.snowStrength;

		if (curWetness <= 0.1f)
			isRaining = false;
		else 
			isRaining = true;

	}
    public void Update()
    { 
    
        if (gotcha == false)
        {
        for (int i = 0; i < mRenderer.Length; i++)
        {
            mats.Add(mRenderer[i].material);
        }
        gotcha = true;
        }

        GetParameters ();

		if (useRipples && isRaining) {
			int a = (int)(Time.time * RippleAnimationSpeed);
			a = a % RippleFrames.Length;
			for (int i=0; i<mats.Count; i++) {
				mats [i].SetTexture ("_Ripple", RippleFrames [a]);
			}
		} else {
			for (int i=0; i<mats.Count; i++) {
				mats [i].SetTexture ("_Ripple", null);
			}
		}
	
		for (int i=0; i<mats.Count; i++) {
			mats [i].SetFloat ("_SnowStrenght", curSnow);
		}

		if (isRaining) {
			if (useWaveAnimation) 
			{
				Vector2 waveSpeed1;
				waveSpeed1.x = Time.time * waveSpeed.x;
				waveSpeed1.y = Time.time * waveSpeed.y;
			
				for (int i=0; i<mats.Count; i++) {
					mats [i].SetTextureOffset ("_WetNormal", waveSpeed1);}

			}
			for (int i=0; i<mats.Count; i++) {
				mats [i].SetFloat ("_Raining", curWetness);}
		}
		else 
		{
			for (int i=0; i<mats.Count; i++) {
				mats [i].SetFloat ("_Raining", -0.01f);}
		}
	}
}

