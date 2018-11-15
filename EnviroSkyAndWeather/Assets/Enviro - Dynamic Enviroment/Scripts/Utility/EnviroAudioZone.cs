using UnityEngine;
using System.Collections;

[AddComponentMenu("Enviro/Utility/Audio Zone")]
public class EnviroAudioZone : MonoBehaviour {

	public bool ambientSFX;
	public bool weatherSFX;

	[Header("Volume Modifications")]
	public float ambientVolume = 0f;
	public float weatherVolume = 0f;

	[Header("Gizmo:")]
	public Color zoneGizmoColor = Color.green;

	private Collider zoneCollider;

	void Start () 
	{

		zoneCollider = GetComponent<Collider> ();

		if(zoneCollider == null)
			zoneCollider = gameObject.AddComponent<BoxCollider>();

		zoneCollider.isTrigger = true;
	}
		
		
	void OnTriggerEnter (Collider col)
	{
		if (EnviroSky.instance.profile == null)
			return;
		
		if (EnviroSky.instance.profile.weatherSettings.useTag) {
			if (col.gameObject.tag == EnviroSky.instance.gameObject.tag) {
				EnviroSky.instance.Audio.ambientSFXVolumeMod += ambientVolume;
				EnviroSky.instance.Audio.weatherSFXVolumeMod += weatherVolume;
			}
		} else {
			if (col.gameObject.GetComponent<EnviroSky> ()) {
				EnviroSky.instance.Audio.ambientSFXVolumeMod += ambientVolume;
				EnviroSky.instance.Audio.weatherSFXVolumeMod += weatherVolume;
			}
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (EnviroSky.instance.profile == null)
			return;
		
		if (EnviroSky.instance.profile.weatherSettings.useTag) {
			if (col.gameObject.tag == EnviroSky.instance.gameObject.tag) {
				EnviroSky.instance.Audio.ambientSFXVolumeMod -= ambientVolume;
				EnviroSky.instance.Audio.weatherSFXVolumeMod -= weatherVolume;
			}
		} else {
			if (col.gameObject.GetComponent<EnviroSky> ()) {
				EnviroSky.instance.Audio.ambientSFXVolumeMod -= ambientVolume;
				EnviroSky.instance.Audio.weatherSFXVolumeMod -= weatherVolume;
			}
		}
	}


	void OnDrawGizmos () 
	{
		Gizmos.color = zoneGizmoColor;
		Gizmos.DrawCube (transform.position, new Vector3(transform.localScale.x,transform.localScale.y,transform.localScale.z));
	}
}
