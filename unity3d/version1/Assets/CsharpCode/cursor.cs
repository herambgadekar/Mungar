using UnityEngine;
using System.Collections;

public class cursor : MonoBehaviour {
	private GameObject FPS;
	// Use this for initialization
	void Start () {
		FPS=GameObject.Find("First Person Controller");
	}
	
	// Update is called once per frame
	void Update () {
			Vector3 offset=Vector3.zero;
			
			offset.y=1.0f;
			//Vector3.Project(offset)
			Vector3 direction= FPS.transform.forward.normalized*5;
		    this.transform.position=FPS.transform.position+direction+offset;
			this.transform.localEulerAngles=FPS.transform.localEulerAngles;
			
	}
}
