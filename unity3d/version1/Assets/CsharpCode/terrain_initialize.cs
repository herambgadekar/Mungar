using UnityEngine;
using System.Collections;

public class terrain_initialize : MonoBehaviour {
	public GameObject  treemodel1;
	public GameObject  treemodel2;
	public GameObject  treemodel3;
	
	public Vector3 position;
	// Use this for initialization
	void placetrees(int[]models){
		Terrain T= Terrain.activeTerrain;
	   	float xstart=T.GetPosition().x;
		float zstart=T.GetPosition().z;
		float ystart=T.GetPosition().y;
		TerrainData terrain_data=T.terrainData;
		float xsize=terrain_data.size.x;
		float zsize=terrain_data.size.z;
		
		float margin=15.0f;
		 
		for(int m=0;m<models.Length && m<3;m++)
		{	
			int t1=models[m];
			GameObject tree;
			switch (m)
			{
			case 0:tree=treemodel1;break;
			case 1:tree=treemodel2;break;
			case 2:tree=treemodel3;break;
			default:tree=treemodel1;break;
			}
			for (int i=0;i<t1;i++)
			{
				float x1=Random.Range(margin, xsize-margin)+xstart;
				float z1=Random.Range(margin, zsize-margin)+zstart;
				float y=T.SampleHeight(new Vector3(x1,0,z1))+ystart;
				float rx=Random.Range(-5,5);
				float rz=Random.Range(-45,45);
				float ry=Random.Range(-45,45);
				GameObject newt=(GameObject) Instantiate(tree, new Vector3(x1,y ,z1), Quaternion.Euler(270, 0, 0));
			
			}
		}


	}
	void Start() {
		  int []inp={1000,1000,800};
		  
		  placetrees(inp);
		  
		  Terrain T= Terrain.activeTerrain;
		  Debug.Log(T.GetPosition());
		  
		//  Debug.Log (T.SampleHeight(new Vector3(2519.154f,0,847.0358f)));
		  GameObject Gobject=gameObject;
		int numbers=Gobject.transform.childCount;
 		int i = 0;
 
		    Debug.Log("name is "+this.transform.position); 	 

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
