using UnityEngine;
using System.Collections;

public class EnviroPuddle : MonoBehaviour {
	
	public GameObject puddleObject;
	[HideInInspector]
	public bool isRaining; 
	
	public bool useWaveAnimation;
	public Vector2 waveSpeed;
	public bool useRipples;
	public Texture2D[] RippleFrames;
	public float RippleAnimationSpeed = 30f;


	private MeshRenderer mRenderer;
	private Material[] mats;
	private float curWaveSpeedx;
	private float curWaveSpeedy;


	public float maxFloodHeight = 0.1f;
	public float minFloodHeight = -0.1f;

	[HideInInspector]
	public float floodVal = 0.3f;

	private float curFlood;
	private float lastUpdate;
	// Use this for initialization
	void Start ()
	{
		mRenderer = puddleObject.GetComponent<MeshRenderer> ();
		mats = mRenderer.materials;
	}
	

	public float Remap (float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}



	// Update is called once per frame
	void Update ()
	{
		mats = mRenderer.materials;
		floodVal = EnviroSky.instance.Weather.wetness;

		if (floodVal <= 0.1f)
			isRaining = false;
		else 
			isRaining = true;

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

		if (isRaining) {
			puddleObject.SetActive(true);
			if (useWaveAnimation) 
			{
				Vector2 waveSpeed1;
				waveSpeed1.x = Time.time * waveSpeed.x;
				waveSpeed1.y = Time.time * waveSpeed.y;
				
				for (int i=0; i<mats.Length; i++) {
					mats [i].SetTextureOffset ("_WetNormal", waveSpeed1);}
				
			}
			for (int i=0; i<mats.Length; i++) {
				mats [i].SetFloat ("_Raining", floodVal);}
		}
		else 
		{
			puddleObject.SetActive(false);
			for (int i=0; i<mats.Length; i++) {
				mats [i].SetFloat ("_Raining", -0.01f);}
		}


		float calcHeight = Remap (floodVal,0f, 1f, minFloodHeight, maxFloodHeight);
		puddleObject.transform.localPosition = new Vector3 (puddleObject.transform.localPosition.x, calcHeight, puddleObject.transform.localPosition.z);

	}
}
