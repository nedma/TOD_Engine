using UnityEngine;
using System.Collections;

[AddComponentMenu("Enviro/Utility/Interior Fog Zone")]
public class EnviroInteriorFog : MonoBehaviour {

	public bool distanceFog;
	public bool heightFog;

	[Header("Interior Fog Modifications")]
	public float distanceFogIntensity = 1f;
	public float heightFogIntensity = 0f;
	public float speed = 10f;


	[Header("Gizmo:")]
	public Color zoneGizmoColor = Color.blue;

	private Collider zoneCollider;
	private float defaultDistanceFogIntensity = 0f;
	private float defaultHeightFogIntensity = 0f;
	private bool inside;

	void Start () 
	{

		zoneCollider = GetComponent<Collider> ();

		if(zoneCollider == null)
			zoneCollider = gameObject.AddComponent<BoxCollider>();

		zoneCollider.isTrigger = true;

		if (EnviroSky.instance.profile != null) {
			defaultDistanceFogIntensity = EnviroSky.instance.profile.fogSettings.distanceFogIntensity;
			defaultHeightFogIntensity = EnviroSky.instance.profile.fogSettings.heightFogIntensity;
		}
	}
		
		
	void OnTriggerEnter (Collider col)
	{
		if (EnviroSky.instance.profile == null)
			return;

		if (EnviroSky.instance.profile.weatherSettings.useTag) {
			if (col.gameObject.tag == EnviroSky.instance.gameObject.tag) {
				inside = true;
			}
		} else {
			if (col.gameObject.GetComponent<EnviroSky> ()) {
				inside = true;
			}
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (EnviroSky.instance.profile == null)
			return;

		if (EnviroSky.instance.profile.weatherSettings.useTag) {
			if (col.gameObject.tag == EnviroSky.instance.gameObject.tag) {
				inside = false;
			}
		} else {
			if (col.gameObject.GetComponent<EnviroSky> ()) {
				inside = false;
			}
		}
	}


	void Update ()
	{
		if (EnviroSky.instance.started) 
		{
			if (inside) {
				if(distanceFog)
					EnviroSky.instance.profile.fogSettings.distanceFogIntensity = Mathf.Lerp (EnviroSky.instance.profile.fogSettings.distanceFogIntensity, distanceFogIntensity, speed * Time.deltaTime);
				if(heightFog)
					EnviroSky.instance.profile.fogSettings.heightFogIntensity = Mathf.Lerp (EnviroSky.instance.profile.fogSettings.heightFogIntensity, heightFogIntensity, speed * Time.deltaTime);
			} else {
				if(distanceFog)
					EnviroSky.instance.profile.fogSettings.distanceFogIntensity = Mathf.Lerp (EnviroSky.instance.profile.fogSettings.distanceFogIntensity, defaultDistanceFogIntensity, speed * Time.deltaTime);
				if(heightFog)
					EnviroSky.instance.profile.fogSettings.heightFogIntensity = Mathf.Lerp (EnviroSky.instance.profile.fogSettings.heightFogIntensity, defaultHeightFogIntensity, speed * Time.deltaTime);
			}
		}
	}

	void OnDrawGizmos () 
	{
		Gizmos.color = zoneGizmoColor;
		Gizmos.DrawCube (transform.position, new Vector3(transform.localScale.x,transform.localScale.y,transform.localScale.z));
	}
}
