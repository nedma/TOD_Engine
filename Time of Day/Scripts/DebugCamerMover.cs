using UnityEngine;
using System.Collections;

public class DebugCamerMover : MonoBehaviour
{
    public float Speed = 1.0f;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    //transform.position += (Vector3.back + Vector3.left + Vector3.down) * Speed;
	    transform.position += (Vector3.back) * Speed;
	}
}
