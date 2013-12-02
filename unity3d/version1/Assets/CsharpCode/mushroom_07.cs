using UnityEngine;
using System.Collections;

public class mushroom_07 : MonoBehaviour {
	private float life;
	private int lifecycle;
	public GUIText TEXT;
	private GameObject FPS;
	private bool die=false;
	// Use this for initialization
	void Start () {
		 life=0f;
		lifecycle=20;
		 FPS=GameObject.Find("First Person Controller");
	}
	public void timer()
	{
		die=true;
		 
	}
	// Update is called once per frame
	void Update () {
		if(die)
		{
			life=life+Time.deltaTime;
			if(life>lifecycle)
			{
				Destroy(gameObject);
				 
			}
		}
	}
	 void OnTriggerEnter(Collider other) {
		
		
		if(other.gameObject.name.ToLower().Contains("First"))
				     FPS=other.gameObject;
		if(FPS!=null)
		{
			int a=FPS.GetComponent<firstperson>().punching;
		if(a>0)
		{	
			FPS.GetComponent<firstperson>().speedup();
			int k=FPS.GetComponent<firstperson>().kills+1;
			Destroy(gameObject);
			string s="HITS";
			if(k==1)
				s="HIT";
			TEXT.text="GOOD JOB!\n"+k+s;
			FPS.GetComponent<firstperson>().displaytext();
			FPS.GetComponent<firstperson>().kills=k;
				FPS.GetComponent<firstperson>().punching=0;
		}
		}
    }
}
