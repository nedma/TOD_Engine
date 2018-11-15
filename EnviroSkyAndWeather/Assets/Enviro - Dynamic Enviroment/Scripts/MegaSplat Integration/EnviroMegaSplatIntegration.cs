using UnityEngine;
using System.Collections;

[AddComponentMenu("Enviro/Integration/MegaSplat Integration")]
public class EnviroMegaSplatIntegration : MonoBehaviour {

	public Material MegaSplatMaterial;

	[Header("Synchronize Weather")]
	public bool UpdateWetness = true;
	public bool UpdateSnow = true;
	public bool UpdatePuddles = true;
	public bool UpdateRainRipples = true;

	private Vector4 MegaSplatWetness;
	private float puddleBlend;

	void Start () 
	{
		if (MegaSplatMaterial != null) 
		{
			MegaSplatWetness = MegaSplatMaterial.GetVector ("_GlobalPorosityWetness");
		}
	}

	void Update () 
	{
		if (MegaSplatMaterial == null)
			return;

		if (UpdateSnow) {
			MegaSplatMaterial.SetFloat ("_SnowAmount", EnviroSky.instance.Weather.curSnowStrength);
		}

		if (UpdateWetness) {
			MegaSplatMaterial.SetVector ("_GlobalPorosityWetness", new Vector4(MegaSplatWetness.x,EnviroSky.instance.Weather.curWetness,MegaSplatWetness.z,MegaSplatWetness.w));
		}
			
		if (UpdatePuddles) {
			puddleBlend = EnviroSky.instance.Weather.curWetness * 60f;
			puddleBlend = Mathf.Clamp (puddleBlend, 1f, 60f);
			MegaSplatMaterial.SetFloat ("_PuddleBlend",puddleBlend);
		}

		if (UpdateRainRipples) {
			MegaSplatMaterial.SetVector ("_RainIntensity", new Vector4(MegaSplatWetness.x,EnviroSky.instance.Weather.curWetness,MegaSplatWetness.z,MegaSplatWetness.w));
		}

	}
}
