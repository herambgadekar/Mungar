using UnityEngine;
using System.Collections;

public class camera : MonoBehaviour {
	public GameObject person;
	// Use this for initialization
	void Start () {
		this.transform.position=person.transform.position;
		this.transform.parent = person.transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
