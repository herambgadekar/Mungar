using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	public Transform sometrees;

	void Start() {
		GameObject Gobject=gameObject;
		Debug.Log("empty name is: "+Gobject.name);
		    
			for (int z = 848; z < 848+7; z=z+2) {
				for (int x = 2520; x < 2520+6; x=x+2) {
					//Instantiate(sometrees, new Vector3(x, 82,z), Quaternion.identity);
				}
			}
		}
	
	// Update is called once per frame
	void Update () {
	
	}
}
