using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {
	public Transform target;
	Camera mainCam;

	void Start(){
		mainCam = GetComponent<Camera> ();
	}

	void Update ()
	{
		if (target) {
			transform.position = Vector3.Lerp(transform.position, target.position, .1f) + new Vector3(0,0,-10);
		}
	}
}