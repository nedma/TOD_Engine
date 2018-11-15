using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LateMouse : MonoBehaviour {


	public float sensitivity = 2f;

	float rotX;
	float rotY;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnPreCull () {

		rotX = Input.GetAxis ("Mouse X") * sensitivity;
		rotY -= Input.GetAxis ("Mouse Y") * sensitivity;

		rotY = Mathf.Clamp (rotY, -60f, 60f);

		transform.Rotate (0, rotX, 0);
		transform.localRotation = Quaternion.Euler(rotY, 0, 0);
	}
}
