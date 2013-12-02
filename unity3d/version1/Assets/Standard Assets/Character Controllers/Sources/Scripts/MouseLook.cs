using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {
	private int init=0;
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationY = 0F;
		public void camerarotateup(float degree)
	{
		transform.localEulerAngles = new Vector3((0-degree),0,0 );
		Debug.Log("look upward");
	}
	public void camerarotateright(float degree)
	{
	  transform.localEulerAngles = new Vector3(0, 0,degree );

	}
	public void camerarotateleft(float degree)
	{
			//-7f;
		 transform.localEulerAngles = new Vector3(0, 0,(0-degree) );

	}
	public void cameraback()
	{
		Vector3  desire=Vector3.zero;
		//desire.x=0-transform.localEulerAngles.x;
		//desire.z=0-transform.localEulerAngles.z;
		 Debug.Log(transform.localEulerAngles );
		Debug.Log("global"+GameObject.Find("First Person Controller").transform.localEulerAngles);
		 transform.localEulerAngles =Vector3.zero;// GameObject.Find("First Person Controller").transform.localEulerAngles;
		//transform.RotateAround(transform.position,transform.forward,-30);
	}
	void Update ()
	{
		
		 /*
		init++;
		if(init<10){
		if(init<=-1)
		{
			Debug.Log("first time");
			//transform.localEulerAngles=(new Vector3(0,0,30));
			transform.RotateAround(transform.position,transform.forward,30);
		}
		
		  Debug.Log(GameObject.Find("First Person Controller").transform.localEulerAngles);
		 Debug.Log("After:"+transform.localEulerAngles );
		
		}*/
		/*
		
		if (axes == RotationAxes.MouseXAndY)
		{
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
			
		}
		else if (axes == RotationAxes.MouseX)
		{
		  	 

			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}
		 */
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
}