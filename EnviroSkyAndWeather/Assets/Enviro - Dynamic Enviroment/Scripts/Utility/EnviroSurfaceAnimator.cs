using UnityEngine;
using System.Collections;

[AddComponentMenu("Enviro/Utility/Surface Animator")]
public class EnviroSurfaceAnimator : MonoBehaviour
{
	[HideInInspector]
	public bool isRaining; 

	public MeshRenderer mRenderer;
	public bool useRipples;
	public Texture2D[] RippleFrames;
	public float RippleAnimationSpeed = 30f;
	public bool useWaveAnimation;
	public Vector2 waveSpeed;
	
	private Material[] mats;
	private float curWaveSpeedx;
	private float curWaveSpeedy;

	private float curWetness = 0f;
	private float curSnow = 0f;

    public void Start()
    {
		if (mRenderer == null)
			mRenderer.GetComponentInChildren<MeshRenderer> ();

		mats = mRenderer.materials;
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
		mats = mRenderer.materials;

		GetParameters ();

		if (useRipples && isRaining) {
			int a = (int)(Time.time * RippleAnimationSpeed);
			a = a % RippleFrames.Length;
			for (int i=0; i<mats.Length; i++) {
				mats [i].SetTexture ("_Ripple", RippleFrames [a]);
			}
		} else {
			for (int i=0; i<mats.Length; i++) {
				mats [i].SetTexture ("_Ripple", null);
			}

		}
	
		for (int i=0; i<mats.Length; i++) {
			mats [i].SetFloat ("_SnowStrenght", curSnow);
		}

		if (isRaining) {
			if (useWaveAnimation) 
			{
				Vector2 waveSpeed1;
				waveSpeed1.x = Time.time * waveSpeed.x;
				waveSpeed1.y = Time.time * waveSpeed.y;
			
				for (int i=0; i<mats.Length; i++) {
					mats [i].SetTextureOffset ("_WetNormal", waveSpeed1);}

			}
			for (int i=0; i<mats.Length; i++) {
				mats [i].SetFloat ("_Raining", curWetness);}
		}
		else 
		{
			for (int i=0; i<mats.Length; i++) {
				mats [i].SetFloat ("_Raining", -0.01f);}
		}
	}
}

