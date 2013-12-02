//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using System.Collections.Generic;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class  gesture
    {
        private int type = 0;
        private string  name = "";
        public const   int glide =1;
        public const   int lande = 2;
        public const   int punching = 3;
        public const   int climbing = 4;
        public const   int jump = 5;
        public const   int lean = 6;

        public const int nogesture = 0;
    }
    public class path
    {
        public   const   int handraising = 2;
        public   const   int handfalling = 3;
        public   const int jumping_initial = 4;
        public const int jumping_launch = 5;
        public const int jumping_falling = 6;
        public const int jumping = 7;
        public  const int processing = 0;
        public  const   int notpath = -1;
        
        private List<Skeleton> skelsinpath = new List<Skeleton>();
        public static bool haspath(int type)
        {
            if (type != processing && type != notpath)
                return true;
            else
                return false;
         }
        public static bool isclimbing(int type)
        {
            if (type == path.handraising || type == path.handfalling)
                return true;
            return false;
        }
        public static string getname(int type)
        {
            string name="";
          switch (type)
            {
                case path.handraising: name= "Climbing (hand rising)";break;
                case path.handfalling: name = "Climbing (hand falling)"; break;
                case path.notpath: name = "no path"; break;
                case path.jumping: name = "jumping"; break;
                default: name = ""; break;
            } 
            return name;
        }
        public List<Skeleton> getskels()
        {
            return skelsinpath;
        }
        public int gtype()
        {
            return type;
        }
        
        
        private int type = 0;
        
        public path(int a)
        {
           
            type = a;
        }
        public path()
        {

            type = 0;
        }

    }
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        ///
        private int []last2gesture=new int[2];
        private const int maxpathnumber = 6;
        private const int skelbuffsize = 5;
        private List<Skeleton> recordedskels=new List<Skeleton>();
        private List<path> recordedpaths = new List<path>();
        private System.Collections.Concurrent.BlockingCollection<int> commandlist = new System.Collections.Concurrent.BlockingCollection<int>();
        private  Socket socketunity=null;

        public int onesidepath(List<Skeleton> skelsinpath,JointType LOR)
        {
            int type = 0;

            double sx = 0;
            double sy = 0;
            double sz = 0;
            for (int i = 0; i <= 1; i++)
            {
                SkeletonPoint point = skelsinpath[i].Joints[LOR].Position;
                sx = sx + point.X;
                sy = sy + point.Y;
                sz = sz + point.Z;

            }
            double ex = 0;
            double ey = 0;
            double ez = 0;
            double shouldercy = 0;
            double hipcy = 0;
            for (int i = 1; i <= 2; i++)
            {
                int k = skelsinpath.Count - i;
                SkeletonPoint point = skelsinpath[k].Joints[LOR].Position;
                ex = ex + point.X;
                ey = ey + point.Y;
                ez = ez = +point.Z;
                shouldercy = shouldercy + skelsinpath[k].Joints[JointType.ShoulderCenter].Position.Y;
                hipcy = hipcy + skelsinpath[k].Joints[JointType.HipCenter].Position.Y;


            }
            shouldercy = shouldercy / 2;
            hipcy = hipcy / 2;
            // System.Console.WriteLine("distance:" + (ey - sy));
            //System.Console.WriteLine("body height:" + (shouldercy-hipcy));
            double bodyheight = System.Math.Abs(shouldercy - hipcy);
            if (System.Math.Abs(ey - sy) <= 0.8 * bodyheight) // change later on
            {

                return path.notpath;
            }
            if (sy < ey)
            {
                type = path.handraising;
            }
            else
            {
                type = path.handfalling;
            }
            return type;
        }
        public int bonepath(List<Skeleton> skelsinpath)
        {

            double headd = 0;
            double spined = 0;
            double hipd = 0;
           Skeleton skeleton=skelsinpath[0];
            double bonelength=skeleton.Joints[JointType.Head].Position.Y - skeleton.Joints[JointType.HipCenter].Position.Y;
            for (int i = 0; i <= 1; i++)
            {
                SkeletonPoint pointhead = skelsinpath[i].Joints[JointType.Head].Position;
                SkeletonPoint pointspine = skelsinpath[i].Joints[JointType.Spine].Position;
                SkeletonPoint pointhip = skelsinpath[i].Joints[JointType.HipCenter].Position;
                headd =headd+ pointhead.Y;
                spined = spined + pointspine.Y;
                hipd = hipd + pointhip.Y;

            }
            for (int i = 1; i <= 2; i++)
            {
                int k = skelsinpath.Count - i;
                SkeletonPoint pointhead = skelsinpath[k].Joints[JointType.Head].Position;
                SkeletonPoint pointspine = skelsinpath[k].Joints[JointType.Spine].Position;
                SkeletonPoint pointhip = skelsinpath[k].Joints[JointType.HipCenter].Position;
                headd = headd - pointhead.Y;
                spined = spined - pointspine.Y;
                hipd = hipd - pointhip.Y;
            }
            int numberofoffset = 0;
            if (headd > 0)
                numberofoffset++;
            if(spined>0)
                numberofoffset++;
            if (hipd > 0)
                numberofoffset++;
            if (numberofoffset % 3 == 0)
            {
                //same direction;
             
                headd = Math.Abs(headd);
                hipd = Math.Abs(hipd);
             //  Console.WriteLine(headd+"jump  length"+bonelength);
              // Console.WriteLine(hipd + "jump  length" + bonelength);
               if (headd>0.15&& hipd>0.15 &&(headd / bonelength > 0.35 || hipd / bonelength > 0.35))
                {
                    return path.jumping;
                }
                else
                    return path.notpath;
                
            }

            return path.notpath;
        }
        public int checkpathtype(List<Skeleton> skelsinpath)
        {

            int boner = bonepath(skelsinpath);
            if (boner != path.notpath)
                return boner;
            int R = onesidepath(skelsinpath, JointType.WristRight);
          
          int L = onesidepath(skelsinpath, JointType.WristLeft);
          
            if (L == R)
                return L;
            else
                return path.notpath;
        }
        public MainWindow()
        {
            InitializeComponent();
            last2gesture[0]=last2gesture[1]=gesture.nogesture ;
           // connecttounity ;
            sonthread SON = new sonthread(this);
            Console.WriteLine("build server");
            Thread workerThread = new Thread(SON.connecttounity);
            workerThread.Start();
        }
        class sonthread
        {
            // This method will be called when the thread is started. 
            MainWindow parent=null;
            public sonthread(MainWindow Mw) {
                parent=Mw;
            }
            public void connecttounity()
            {

                // Data buffer for incoming data.
                byte[] bytes = new Byte[1024];
                string data = null;
                int lastindex = 0;
                // Establish the local endpoint for the socket.
                // Dns.GetHostName returns the name of the 
                // host running the application.
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 4800);
                
                // Create a TCP/IP socket.
                Socket listener = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(100);
                // Bind the socket to the local endpoint and 
                // listen for incoming connections.
                //Console.WriteLine("starting server...");
                while (true)
                {
                    try
                    {
                        

                        // Start listening for connections.

                        Console.WriteLine("Waiting for a connection...");
                        // Program is suspended while waiting for an incoming connection.
                        Socket handler = listener.Accept();
                        parent.socketunity = handler;
                        parent.commandlist = new System.Collections.Concurrent.BlockingCollection<int>();
                        Console.WriteLine("Accept new one");
                        // An incoming connection needs to be processed.
                        while (true)
                        {
                            int command = -1;

                            if (parent.commandlist.Count > 0)
                            {
                                int i = parent.commandlist.Count;

                                int j = 0;
                                while (j < i - 1)
                                {

                                    parent.commandlist.Take();
                                    j++;
                                }
                                command = parent.commandlist.Take();
                                lastindex = parent.commandlist.Count;
                                byte[] msg = System.Text.Encoding.UTF8.GetBytes(" " + command + " \n");
                                handler.Send(msg);
                                Console.Write(msg.ToString());
                                System.Threading.Thread.Sleep(150);
                               
                                
                            }

                        }

                        // Show the data on the console.
                        // Console.WriteLine("Text received : {0}", data);

                        // Echo the data back to the client.




                    }
                    catch (Exception e)
                    {
                        // handler.Shutdown(SocketShutdown.Both);
                        //  handler.Close();
                        parent.socketunity = null;
                        Console.WriteLine(e.ToString());
                        Console.WriteLine("continue wait...");
                        continue;
                    }
                }
                
                // Console.Read();
            }
         
            public void RequestStop()
            {
               
            }
            // Volatile is used as hint to the compiler that this data 
            // member will be accessed by multiple threads. 
           // private volatile bool _shouldStop;
        }

     

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// 
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            
                            this.DrawBonesAndJoints(skel, dc);
                            this.updategesture(skel);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }


        /*
         * process the frame buffer which stored several frames. 10-20 frames
         * *
         */
        private double[] leanleftright(Skeleton skeleton)
        { 
            double elbowleftx= skeleton.Joints[JointType.ElbowLeft].Position.X;
            double elbowlefty = skeleton.Joints[JointType.ElbowLeft].Position.Y;
            double elbowrightx = skeleton.Joints[JointType.ElbowRight].Position.X;
            double  elbowrighty = skeleton.Joints[JointType.ElbowRight].Position.Y;
            double wristleftx = skeleton.Joints[JointType.WristLeft].Position.X;
            double wristlefty = skeleton.Joints[JointType.WristLeft].Position.Y;
            double wristrightx = skeleton.Joints[JointType.WristRight].Position.X;
            double wristrighty = skeleton.Joints[JointType.WristRight].Position.Y;
            double k = (elbowrighty - elbowlefty) / (elbowrightx - elbowleftx);
            double k2 = (wristrighty - wristlefty) / (wristrightx - wristleftx);
            //if(k>0.5 &&k2>0.5 ||)
             double []r=new double [2];
                r[0]=k;
                r[1]=k2;
                return r;
            //Console.WriteLine(k+":"+k2);
         //   if()
        }
        private void updategesture(Skeleton skeleton)
        {
            int COM = -1;
            float LHX, RHX, handsdistance;
            LHX = skeleton.Joints[JointType.WristLeft].Position.X;
            RHX = skeleton.Joints[JointType.WristRight].Position.X;
            double RHZ = skeleton.Joints[JointType.WristRight].Position.Z; // z coordinate of right wrist
            double RSZ = skeleton.Joints[JointType.ShoulderRight].Position.Z; // z coordinate of right shoulder
            double LHZ = skeleton.Joints[JointType.WristLeft].Position.Z;
            double LSZ = skeleton.Joints[JointType.ShoulderLeft].Position.Z;
            //label2.Content = skeleton.Joints[JointType.WristRight].Position.X + ":" + skeleton.Joints[JointType.ShoulderRight].Position.X;
            //label3.Content = skeleton.Joints[JointType.WristLeft].Position.X + ":" + skeleton.Joints[JointType.ShoulderLeft].Position.X;
            // System.Console.WriteLine(label2.Content+"     "+label3.Content);
           // Console.WriteLine("head"+skeleton.Joints[JointType.Head].Position.Y);
           // Console.WriteLine("length" + (skeleton.Joints[JointType.Head].Position.Y - skeleton.Joints[JointType.HipCenter].Position.Y));
           // Console.WriteLine("spine" + skeleton.Joints[JointType.Spine].Position.Y);
           // Console.WriteLine("foot" + skeleton.Joints[JointType.FootLeft].Position.Y);
            double shoulderdistance = (skeleton.Joints[JointType.ShoulderRight].Position.X - skeleton.Joints[JointType.ShoulderLeft].Position.X);
            
            if (shoulderdistance < 0)
                shoulderdistance = 0 - shoulderdistance;
            handsdistance = (LHX - RHX);
            double handepthR = (RSZ - RHZ);
            double handepthL = (LSZ - LHZ);
            int processresult = Processskels(skeleton);

            //System.Console.WriteLine(processresult);
            if (handsdistance < 0)
                handsdistance = (float)(0 - handsdistance);
            string TEXT = "";
            double[] angle=leanleftright(skeleton);
          //  Console.WriteLine(handepthL + "l - r" + handepthR);
            if (handsdistance / shoulderdistance > 2.5)//45
            {
                COM = -1;
                if (angle[0] > 0.3 )
                {
                    COM = 1;
                    TEXT = "turn left";
                }
                if (angle[0] < -0.3 )
                {
                    COM = 2;
                    TEXT = "turn right";

                }
                if (Math.Abs(angle[0]) < 0.15 && Math.Abs(angle[1]) < 0.15)
                {
                    TEXT = "Gliding";
                    last2gesture[0] = last2gesture[1];
                    last2gesture[1] = gesture.glide;
                    COM = 6;
                }
                
            }
            else
            {
               // if()

                // System.Console.WriteLine(path.haspath(processresult));


                if (path.haspath(processresult))
                {
                      // capture all the gestures  flying landing climbing and jumping here
                        TEXT = path.getname(processresult);
                        last2gesture[0] = last2gesture[1];
                        if (path.isclimbing(processresult))
                        {
                            last2gesture[1] = gesture.climbing;
                            COM = 3;
                        }
                        if(processresult==path.jumping) 
                            COM = 0;
                        //  if(processresult==path.handfalling|)
                   
                }
                else
                {
                    if ((handepthR > 0.35 && handepthL < 0.25) || (handepthL > 0.35 && handepthR < 0.25))
                    {
                        //Console.WriteLine("punching:"+handepthL + "l - r" + handepthR);
                        
                            if (handepthR > 0.35)
                                TEXT = "Punching (Right Hand)";
                            else
                                TEXT = "Punching (Left Hand)";
                            last2gesture[0] = last2gesture[1];
                            last2gesture[1] = gesture.punching;
                            COM = 5;
                        
                    }
                    else
                    {
                        double rhy = skeleton.Joints[JointType.WristRight].Position.Y;
                        double lhy = skeleton.Joints[JointType.WristLeft].Position.Y;
                        double bodyheight = skeleton.Joints[JointType.ShoulderCenter].Position.Y - skeleton.Joints[JointType.HipCenter].Position.Y;
                        double handdis = System.Math.Abs(rhy - lhy);
                      if ((handsdistance / shoulderdistance < 1.5) && (handdis < 0.3 * bodyheight))
                        {
                             
                            double rz = System.Math.Abs(skeleton.Joints[JointType.ShoulderRight].Position.Z- skeleton.Joints[JointType.ElbowRight].Position.Z);
                            rz = rz+System.Math.Abs(skeleton.Joints[JointType.ElbowRight].Position.Z - skeleton.Joints[JointType.WristRight].Position.Z);
                            double lz = System.Math.Abs(skeleton.Joints[JointType.ShoulderLeft].Position.Z - skeleton.Joints[JointType.ElbowLeft].Position.Z);
                            lz =lz + System.Math.Abs(skeleton.Joints[JointType.ElbowLeft].Position.Z - skeleton.Joints[JointType.WristLeft].Position.Z);
                            if (rz < 0.13 && lz < 0.13)
                            {
                                if (last2gesture[1] != gesture.climbing)
                                {
                                    TEXT = "Landing";
                                    last2gesture[0] = last2gesture[1];
                                    last2gesture[1] = gesture.lande;
                                    COM = 4;
                                }
                            }
                            else { 
                                
                            }
                         
                        }
                        else
                        {
                            COM = -1;
                            TEXT = "No Gesture";
                        }
                    }

                }

            }
            if (TEXT.Length > 2)
            {
                if (this.socketunity != null)
                { 
                    if(COM>-1)
                    commandlist.Add(COM); 
                }
                gesturelabel.Content = TEXT; 
            }
          if(COM>-1)
            Console.WriteLine(COM+" is "+TEXT);
        }
        private int Processskels( Skeleton newskeleton)//(ReplaySkeletonFrame frame)
        {
           
             
            if (recordedskels.Count < skelbuffsize) // if the buffer is too small no operation
            {
                recordedskels.Add(newskeleton);
                return path.processing;
            }
            else
            {
                //recordedskels.RemoveAt(0);
                recordedskels.Add(newskeleton);
            }
            double DR = 0;
            
            double dx,dy,dz;
            Skeleton skelo = recordedskels[0];
            for (int i = 0; i < recordedskels.Count;i++ )
            {
                Skeleton skel = recordedskels[i];
                SkeletonPoint oldpoint = skelo.Joints[JointType.WristRight].Position;
                SkeletonPoint point=skel.Joints[JointType.WristRight].Position;
                dx = System.Math.Abs(oldpoint.X-point.X);
                dy= System.Math.Abs(oldpoint.Y-point.Y);
                dz = System.Math.Abs(oldpoint.Z - point.Z);
                DR = DR + dx + dy+ dz;
                
                 
            }
            if (DR < 1)
            {

                if (recordedpaths.Count < 1)
                {
                    path currentpath = new path(path.notpath);
                    currentpath.getskels().AddRange(recordedskels);
                    recordedpaths.Add(currentpath);

                }
                else
                {
                    path previouspath = recordedpaths[recordedpaths.Count - 1];
                    if (!path.haspath(previouspath.gtype()))
                    {

                    }
                    else
                    {
                        path currentpath = new path(path.notpath);
                        currentpath.getskels().AddRange(recordedskels);
                        recordedpaths.Add(currentpath);
                    }
                }
                recordedskels.Clear();
                
                
                return path.notpath;

            }
            else
            {
                 
               // System.Console.WriteLine(System.DateTime.Now.TimeOfDay + ":Has path");
  
                int currenttype= checkpathtype(recordedskels);
                //System.Console.WriteLine("Currenttype:" + currenttype);
                if(recordedpaths.Count>0)
                {
                     

                    path previouspath = recordedpaths[recordedpaths.Count - 1];
                      int previoustype = previouspath.gtype();
                      if (previoustype == currenttype )
                      {
                          List<Skeleton> S = previouspath.getskels();
                          
                          for (int i = 0; i < recordedskels.Count; i++)
                              S.Add(recordedskels[i]);
                           
                      }
                      else
                      {
                          path currentpath = new path(currenttype);
                          List<Skeleton> S = currentpath.getskels();
                          for (int i = 0; i < recordedskels.Count; i++)
                              S.Add(recordedskels[i]);
                          recordedpaths.Add(currentpath);

                      }
                     
                     
                }else{
                    if (!path.haspath(currenttype))
                    {

                      

                    }
                    else
                    {
                       
                        path currentpath = new path(currenttype);
                        List<Skeleton> S = currentpath.getskels();
                        for (int i = 0; i < recordedskels.Count; i++)
                            S.Add(recordedskels[i]);
                        recordedpaths.Add(currentpath);
                    }
                }
                recordedskels.Clear();
                if (recordedpaths.Count > maxpathnumber)
                    recordedpaths.RemoveAt(0);
               /* if(currenttype== path.handraising)
                    System.Console.WriteLine("rasing");
                if (currenttype == path.handfalling)
                    System.Console.WriteLine("falling");
               */
                return currenttype;
            }
            //System.Console.WriteLine(D);
            
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {

        
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);
 
            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;                    
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;                    
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

   
    }
}