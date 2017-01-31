using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class door : MonoBehaviour {
	Quaternion start_rotation;
	bool returning=false;
	bool rest=true;
	public float rotation_speed=30;
	public float force_value=10;
	Rigidbody rb;

	void Start() {
		start_rotation=transform.rotation;
		rb=gameObject.GetComponent<Rigidbody>();
	}

	public void Activate(Vector3 pos) {
		if (transform.rotation!=start_rotation) rest=false; else rest=true;
		if (rest)	{
			rb.AddForceAtPosition((transform.position-pos)*force_value, pos);
			rest=false;
		}
		else {
			rb.angularVelocity=Vector3.zero;
			returning=true;

		}
	}

	void Update () 
	{
		if (returning)
		{
			transform.rotation=Quaternion.RotateTowards(transform.rotation,start_rotation,rotation_speed*Time.deltaTime);
			if (transform.rotation==start_rotation) {returning=false;rest=true;}
		}
	}


}
