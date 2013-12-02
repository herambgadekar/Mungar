using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
public class firstperson : MonoBehaviour {
	//public GameObject  mushmodel1;
	//public GameObject  mushmodel2;
	private int quit=-1 ;
	private float countdownexit=2f;
	private float distance=0f;
	private int SW=0;
	private int delay=40;
	public GUIText TEXT;
	public int punching=0;
	public int kills=0;
	private int displayt;
	private Socket S;
 	private int port=4800;
	private float liftforce= 19.9f;
	private float resistance=0f;//0.1f;//1.00f;
	private int lastcom=-1;
	private CharacterMotor Motor;
	private float startspeed=10.0f;
	private int length=40;
	private int framenumber=0;
	private int mushroomcounter=0;
	private int longlength=50;
	private float step=2.0f;
	private float climbspeed=8.0f;
	private int smooth=-1;
	private float range= 10f;
	private float uprange=10f;
	private float upsteep=2.5f;
	private float attenuation=0.65f;
	private int freeze=0;
	private int changez=0;
	private int changex=0;
	private MouseLook Look;
	private int []commandist={0,5,-1,-1,4,4,-1,-1,3,3,3,3,3,3,0,-1,2,2,-1,-1,-1,-1};
	private int commandindex=-1;
	private float rotatez=0;
	private float rotatex=0;
	// Use this for initialization
	void Start () {
		
		port=4800;
		connect ();
		Vector3 liftvelocity = Vector3.zero;
		Look =this.GetComponent<MouseLook>();
		Motor=this.GetComponent<CharacterMotor>();		 
		liftvelocity.y = liftforce * Time.deltaTime;
		 Vector3 CurrentV=  this.GetComponent<CharacterMotor>().movement.velocity;
		 this.GetComponent<CharacterMotor>().movement.velocity=CurrentV+liftvelocity;
			TEXT.text="";
	}
	
	// Update is called once per frame
	void placemushrooms(){
		float marginmush=5.0f;
		if(this.mushroomcounter<=0)
		{
			mushroomcounter=60;
				for(int j=0;j<4;j++)
				{
					
					GameObject mushm=GameObject.Find("Mushroom_06_a");
					int r=Random.Range(0,3);
				 
					bool place=true;
					switch (r)
					{
					case 0: mushm=GameObject.Find("Mushroom_06_a") ;break;
					case 1:mushm=GameObject.Find("Mushroom_07_c");break;
					default: place=false; break;
					}
					if(place&(mushm!=null)){
						Terrain T=Terrain.activeTerrain;
						float xm=Random.Range(marginmush-10,10+marginmush)+this.transform.position.x;
						float zm=Random.Range(marginmush-5,30+marginmush)+this.transform.position.z;
						float ym=T.SampleHeight(new Vector3(xm,0,zm))+T.GetPosition().y+Random.Range(0f,20f);
						GameObject newm=(GameObject) Instantiate(mushm,new Vector3(xm,ym,zm),Quaternion.Euler(0,0,0) );
						if(r==0)
						   newm.GetComponent<mushroom_06>().timer();
						if(r==1)
						   newm.GetComponent<mushroom_07>().timer();
					}
				}
		}
		else{
			mushroomcounter--;
			
		}
	}
	void Update(){}
	
	void updatedisplaytext()
	{
		if(displayt>0) 
			displayt--;
		if(displayt<=0)
		{
			TEXT.text="";
		}
	}
	 public void displaytext()
	{
		this.displayt=80;
	}
	void smoothmodify(){
		//Debug.Log("last com:"+lastcom+":|freeze:"+freeze+" "+Look.transform.eulerAngles);
		switch (lastcom)
		{
		   case 1: //left
			
			 
				 
					if(rotatez==0)
					
				{   
					 Look.cameraback();
					 Vector3 rotate=Vector3.zero;
					 rotate.z=this.range;
					 Look.transform.Rotate(rotate);
					 rotatez=1f;
					 freeze=this.length;
				}
		 
			break;
		   case 2: //right
			 		if(rotatez==0)
					
				{   
					 Look.cameraback();
					 Vector3 rotate=Vector3.zero;
					 rotate.z=0-this.range;
					 Look.transform.Rotate(rotate);
					 rotatez=-1f;
				   freeze=this.length;
				}
			break;
		   case 3: 
				if(rotatex==0)
			{
				 Look.cameraback();
				 
					 
					Vector3 rotate=Vector3.zero;
					rotate.x=(0-this.uprange);
					Look.transform.Rotate(rotate);
					rotatex=1f;
				    freeze=this.length;
				 
			}
			break;
		   default:
				//freeze=0;
			break;
			
		}
	 
		if(freeze>0)
		{
			freeze--;
			 
		}else{
			if(freeze==0)
			{
				rotatez=rotatex=0f;
				Look.cameraback();
			}
			
		}		
	
	
		
	}
	public void speedup(){
		Vector3 jumpDirection=this.transform.forward;
						jumpDirection.y=0;
						jumpDirection=jumpDirection.normalized;
					  
					  Motor.movement.velocity=jumpDirection*this.startspeed;
	}
	void FixedUpdate () {
				// Debug.Log ("current:"+Motor.movement.velocity+": normal"+Motor.groundNormal+"groundis"+Motor.grounded);
		//Debug.Log("collision"+this.collider);
		          Vector3 CurrentV=  this.GetComponent<CharacterMotor>().movement.velocity;
				  distance=distance+CurrentV.magnitude*Time.deltaTime;
		          CharacterController controller = GetComponent<CharacterController>();
		           Vector3 liftvelocity = Vector3.zero;
					 if(Motor.grounded)
			{
								TEXT.text="Score: "+(int)this.distance;
								quit=20;
			 					if(quit>0)
								{
									countdownexit=countdownexit-Time.deltaTime;
								}
								if(countdownexit<0)
									Application.Quit();
								this.displaytext();
			}
		 
		 liftvelocity.y = liftforce * Time.deltaTime;
	  //  smoothmodify();
		//Debug.Log("transfom euler"+this.transform.localEulerAngles+":rotation"+this.transform.rotation);
		placemushrooms();
		updatedisplaytext();
		if(CurrentV.y<0.1)
		{ 
			float vupward=liftvelocity.y+CurrentV.y;
			if(vupward>0)
				liftvelocity.y=0-CurrentV.y-0.001f;
			this.GetComponent<CharacterMotor>().movement.velocity=CurrentV+liftvelocity;
		}
		  if (!this.Motor.grounded)
		 {	 
				//Debug.Log("ground normal:"+Motor.groundNormal);
						 
							CurrentV=Motor.movement.velocity;
							 Vector3 forwardvelocity = Vector3.zero;				 
					         forwardvelocity.z = resistance * Time.deltaTime;
							
							 if(CurrentV.z <forwardvelocity.z)
								CurrentV.z=0;
							else 
								CurrentV=CurrentV-forwardvelocity;  
							 Motor.movement.velocity=CurrentV;
				 			
						 
								 if(Motor.thetree!=null){ 
										if(Motor.thetree.collider.isTrigger)
											{	float x=Motor.thetree.transform.position.x;
												 x=x-this.transform.position.x;
												 float z=Motor.thetree.transform.position.z;
												 z=z-this.transform.position.z;
												 if(x*x+z*z>9) 
												  Motor.thetree.collider.isTrigger=false;
											}
								}				
							 
						//update punching;
					 punching--;
		 }else{
			punching=0;
			Debug.Log("land on game over");
				 Motor.movement.velocity=Vector3.zero;
				//Debug.Log(transform.position);
		}
		
		if(this.Motor.onthetree)
		{
			//on the tree add some mocali
			CurrentV= this.GetComponent<CharacterMotor>().movement.velocity;
			
			
		}
			
       			// controller.Move(moveDirection );
		 
				int command=-1;
				byte[] message = new byte[256];
				byte[] buffer=new byte[1];
				int offset=0;
				if(S.Available>2){
					        while(S.Available>0)
							{ 	
								int messagel =S.Receive(buffer); 
 								if( System.Convert.ToChar(buffer[0])=='\n')
							 		break;
								else
									{	
										message[offset]=buffer[0];
										offset++;
									}
								
							}
						 

				}
			if(offset>0){
					string result = System.Text.Encoding.UTF8.GetString(message,0,offset);
				    
					result.Trim();
				   command= System.Convert.ToInt32(result);
					lastcom=command;
					 Debug.Log("command:"+command);	
			        
				 
					
			 		 
					//parse command
				}
		//command=-1;
	    if(SW>0)
			SW--;
		if(true)
		{
			
		if(Input.GetKey("7"))
			{
				command=7;
			}
		if(Input.GetKey("0"))
		{
			command=0;
		}
		if(Input.GetKey("1"))
		{
			command=1;
		}
		if(Input.GetKey("2"))
		{
			command=2;
			
		}
		if(Input.GetKey("3"))
		{
			command=3;
			
		}
		if(Input.GetKey("4"))
		{
			command=4;
			
		}
		if(Input.GetKey("5"))
		{
			command=5;
		}
		lastcom=command;
		}
		Vector3 movedire=Vector3.zero;
		Vector3 newspeed=Vector3.zero;
			switch(command)
			{
				case 0://jump-flying;
				{
					Debug.Log("press 0");
			         bool on=this.Motor.onthetree;
					if(on)
					{ 			Debug.Log("Jumping");
					  
					//CharacterController controller = GetComponent<CharacterController>();
					Vector3 jumpDirection=this.transform.forward;
						jumpDirection.y=0;
						jumpDirection=jumpDirection.normalized;
					  Debug.Log("direction forward:"+this.transform.forward);
					  Motor.movement.velocity=jumpDirection*this.startspeed;
					  if(Motor.thetree==null)
							Debug.Log("no tree");
				      else{
						Motor.thetree.collider.isTrigger=true;
						}
			    	  Motor.onthetree=false;
					  //Motor.thetree.collider.isTrigger=true;
					}
          			  //this.rigidbody.velocity = this.transform.forward*10;
			
					

				}
					break;
				case 1://turn left
					{	
					 if(lastcom!=1)
						Look.cameraback();
					if(lastcom==1 && SW>0)
					{
				
					}else{
				           // GameObject.Find("Main Camera").transform.localEulerAngles=new Vector3(0,0,10f);
							SW=this.delay;
							Debug.Log("left");
							if(freeze==0)
								freeze= -1;
							if(freeze>0)
								freeze=this.length;
							smooth=0;
							   movedire=this.transform.forward;
					 
						 
							transform.Rotate(0, (0-this.range), 0);
					 
							Vector3 d=transform.forward;
							float oldspeed=Motor.movement.velocity.magnitude;
							newspeed=Vector3.Project( Motor.movement.velocity,d);
					        Motor.movement.velocity=newspeed.normalized*oldspeed*0.99f;
							//Motor.movement.velocity=newspeed;
							Debug.Log("direction is "+d);
							}
					}
					break;
				case 2://turn right
					 {
					 
					 if(lastcom!=2)
						Look.cameraback();
					if(lastcom==2 && SW>0)
					{
				
					}else{
							SW=delay;
							Debug.Log("right");
							if(freeze==0)
								freeze= -1;
							if(freeze>0)
								freeze=this.length;
							smooth=0;
							movedire=this.transform.forward;
							//this.framenumber=this.length+range/step+2;
							transform.Rotate(0, this.range, 0);
							Vector3 d=transform.forward;
							float oldspeed=Motor.movement.velocity.magnitude;
							newspeed=Vector3.Project( Motor.movement.velocity,d);
							Motor.movement.velocity=newspeed.normalized*oldspeed*0.99f;
							//this.GetComponent<MouseLook>().camerarotateright();
							 }
				}
					break;
				case 3: //climbing tree
					{
						if(lastcom!=3)
			            Look.cameraback();
						if(lastcom==3 && SW>0)
					{
				
					}else{
								SW=delay;
								Debug.Log(" climbing tree");
							     bool on=this.Motor.onthetree;
								 if(on)
								 {			
											 
												
											if(freeze==0)
											freeze= -1;
											if(freeze>0)
												freeze=this.length; 
											Vector3 climbing=Vector3.up*this.climbspeed;
											Motor.movement.velocity=climbing;
											smooth=0;
											freeze=-1;
											//Motor.movement.velocity.y=20f;
											//this.GetComponent<MouseLook>().camerarotateup();
											//framenumber=this.longlength;
											Debug.Log(" to up");
								 }else{
						
								}
							}
					}break;
				case 4: //stoping
				{	
					 
					if(lastcom!=4)
			            Look.cameraback();
						if(lastcom==4 && SW>0)
					{
				
					}else{
							SW=delay;
						 Debug.Log("press4 "+Motor.movement.velocity);
						if(!Motor.onthetree && !Motor.grounded)
						{
							 
							Vector3 d=transform.forward;
							 Vector3 reduce =(attenuation-1)*Vector3.Project( Motor.movement.velocity,d);
							
							Motor.movement.velocity=Motor.movement.velocity+reduce;
						}
					}
				}break;
				case 5://punching
				{	
					if(lastcom!=5)
			            Look.cameraback();
						if(lastcom==5 && SW>0)
					{
				
					}else{
					SW=delay;
							if(!Motor.onthetree && !Motor.grounded)
							{
								
								punching=80;
							}
					}
				}break;
		case 6:{//gliding
			
		}break;
		case 7:  this.transform.localEulerAngles=Vector3.zero;
				//GameObject.Find("Main Camera").transform.localEulerAngles=Vector3.zero;
				
				break;
				default: break;
			}
				/* if (BitConverter.IsLittleEndian)
    				Array.Reverse(bytes);

				int i = BitConverter.ToInt32(bytes, 0);*/
		
	}
	
	void connect(){
		 
		 S = new System.Net.Sockets.Socket( System.Net.Sockets.AddressFamily.InterNetwork, 
   		 System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
		 IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress,port);
		  S.Connect(remoteEP);
		 
		Debug.Log("Socket connected to"+S.RemoteEndPoint.ToString());
		 //s.Available;
	}
}
