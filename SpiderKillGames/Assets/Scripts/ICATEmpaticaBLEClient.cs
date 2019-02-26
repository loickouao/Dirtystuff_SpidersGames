/* -- ICAT's Empatica Bluetooth Low Energy(BLE) Comm Client -- *
 * ----------------------------------------------------------- *
 * 0. Attach this to main camera or any empty game object
 * 1. On launch, it tries to connect to the localhost/port20 
 * 	  (You have to change it to your own ip/port combination).
 * 2. Enter the Device ID and connect to device.
 * 3. Select the data streams to log and hit "Log Data"
 * 4. Hit Ctrl+Shift+Z to disconnect at anytime.
 * 
 * Written By: Deba Saha (dpsaha@vt.edu)
 * Virginia Tech, USA.  */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Diagnostics;

using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using System.Threading;

public class ICATEmpaticaBLEClient : MonoBehaviour {	
	//variables	
	private TCPConnection myTCP;	
	private string streamSelected;
	public string msgToServer;
	public string connectToServer;
	
	private string savefilename = "name" + DateTime.UtcNow.ToString("dd_mm_yyyy_hh_mm_ss") + ".txt";

	//flag to indicate device conection status
	private bool deviceConnected = false;

	//flag to indicate if data to be logged to file
	private bool logToFile = false;

	public static string EmpaticaDeviceID = "484C5C";//"7DBBA7"; // Hardcoded but can be changed.

    // Keep a reference if the stream is active in case we want to stop it mid game or something.
    private bool gsrActive = false;

    private bool subscribeToAll = false;
    private bool waitingForResponse = false;
    private Text empaticafeedbackText;
    private Text empaticaGSRStream;

    // The Stream Starts Paused so we can control when to start it
    private static bool pauseStream = true; 
    public static bool streamIsPaused = false;

    public bool empaticaReady = false;

    private Thread socketStreamming;
    public static Queue<string> streamResponse;

    public static bool isStreaming;
    public static bool threadRunning;

    private float checkrate = 2f;
    private float nextTimeToCheck = 0f;
    public float gsr; 
    public float initgsr;
    public ArrayList gsrData;
    private int nspiders;

    void Awake() {
        //add a copy of TCPConnection to this game object		
        myTCP = new TCPConnection();
	}

	void Start () {

        empaticafeedbackText = GameObject.Find("EmpaticaFeedback").GetComponent<Text>();
        empaticaGSRStream = GameObject.Find("EmpaticaGSR").GetComponent<Text>();
        initgsr = 0f; gsr = 0f;
        nspiders = HoldData.numberSelected;
        gsrData = new ArrayList();
        if (nspiders == 99)
            return;

        //DisplayTimerProperties ();
        streamSelected = String.Empty; // Important need to Initialize the Stream Before using it!

		if (myTCP.socketReady == false) {			
			Debug.Log("Attempting to connect..");
			//Establish TCP connection to server
			myTCP.setupSocket();
		}

        if (empaticafeedbackText != null)
            empaticafeedbackText.text += "Connected To Server\n";

        streamResponse = new Queue<string>();
        connectToDevice();        
    }

	void Update ()
    {		
		//keep checking the server for messages, if a message is received from server, 
		//it gets logged in the Debug console (see function below)
        if (Time.time >= nextTimeToCheck) {
            nextTimeToCheck = Time.time + 1f / checkrate;
            if(!isStreaming)
            SocketResponse();
        
            if (!isStreaming)
            {
                if (!waitingForResponse && deviceConnected)
                {
                    if (subscribeToAll)
                    {
                        if (!gsrActive)
                            connectToStream(EmpaticaStreamType.StreamType.Galvanic_Skin_Response);
                        Thread.Sleep(10);
                    }
                }
            }
        }
	}

	void OnApplicationQuit()
    {
        if(socketStreamming != null)
        {
            Debug.Log("Stopping Thread...");
            isStreaming = false;
            SendToServer("device_disconnect");
            while (threadRunning)
            {
                Debug.Log("Waiting for thread to close...");
            }
            Debug.Log("Thread Has Stopped!");
        }

        if (EmpaticaStreamLogger.threadIsRunning)
        {
            Debug.Log("Stopping Writter Thread...");
            EmpaticaStreamLogger.stopThread = true;
            while (EmpaticaStreamLogger.threadIsRunning)
            {
                Debug.Log("Waiting for writer to finish writing...");
                Thread.Sleep(100);
            }
            Debug.Log("Writter thread has finished..");
        }

    }

    public void connectToDevice()
    {
        if (myTCP.socketReady == true && deviceConnected == false)
        {
            SendToServer("device_connect " + EmpaticaDeviceID);
            Debug.Log("Connected to Empatica. Press Ctrl+Shift+Z to disconnect Empatica at any time");
        }

        if (empaticafeedbackText != null)
            empaticafeedbackText.text += "Sucessfully Connected to Device " + EmpaticaDeviceID + "\n";

        subscribeToAllStreams();
    }

    private void connectToStream(EmpaticaStreamType.StreamType type)
    {
        if (myTCP.socketReady == true && deviceConnected == true && logToFile == false)
        {
            // Subscribe to the GSR Stream
            if (type == EmpaticaStreamType.StreamType.Galvanic_Skin_Response)
            {
                if (gsrActive)
                {
                    SendToServer("device_subscribe gsr OFF");
                }
                else
                {
                    SendToServer("device_subscribe gsr ON");
                    streamSelected += "GSR ";
                }
                gsrActive = !gsrActive;
            }
        }
    }

    // Subscribe to All Streams
    public void subscribeToAllStreams()
    {
        subscribeToAll = true;
    }

	//socket reading script	
	void SocketResponse() {		
		string serverSays = myTCP.readSocket();	
		if (serverSays != "") {		
			if (myTCP.socketReady == true && deviceConnected == true && logToFile == true){
				//streamwriter for writing to file
				using(StreamWriter sw = File.AppendText(savefilename)){
					sw.WriteLine(serverSays);
				}
			}
            else
            {
                // Check the Server Response Here
				string serverConnectOK = @"R device_connect OK";

                //Check if server response was device_connect OK
                waitingForResponse = false;
                if (string.CompareOrdinal(Regex.Replace(serverConnectOK, @"\s", ""), Regex.Replace(serverSays.Substring(0, serverConnectOK.Length), @"\s", "")) == 0) {
                    deviceConnected = true;
                }
                else
                {
                    string[] serverSaySplit = serverSays.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                    foreach(string s in serverSaySplit)
                    {
                        string[] parsedResponse = s.Split(null);
                        if (parsedResponse[0] == "R")
                        {
                            // Pause Response
                            if (parsedResponse[1] == "pause")
                            {
                                if (parsedResponse[2] == "ON")
                                {
                                    streamIsPaused = true;
                                }
                                else
                                {
                                    streamIsPaused = false;
                                }
                            }
                        }
                        else
                        {
                            string[] parseLine = s.Split(null);
                            if (parseLine[0] == "E4_Gsr")
                            {
                                empaticaGSRStream.text = parseLine[0] + " = " + parseLine[2];
                                if (initgsr == 0f && gsrData.Count < 4)
                                    gsrData.Add(float.Parse(parseLine[2]));
                                else if (initgsr == 0f && gsrData.Count == 4) {
                                    foreach(float i in gsrData) {
                                        initgsr += i;
                                    }
                                    initgsr /= 4;
                                    gsrData.Clear();
                                }
                                if (initgsr != 0 && gsrData.Count < 4) {
                                    gsrData.Add(float.Parse(parseLine[2]));
                                } else if (initgsr != 0 && gsrData.Count == 4) {
                                    gsr = 0f;
                                    foreach(float i in gsrData) {
                                        gsr += i;
                                    }
                                    gsr /= 4;
                                    gsrData.Clear();
                                }
                            }
                            compute();
                        }
                    }
                }
            }
        }
	}

	//send message to the server	
	public void SendToServer(string str) {		 
		myTCP.writeSocket(str);		
	}

    public void compute() {
        if (initgsr != 0f) {
            int k = 0;
            if (gsr-initgsr > 0f) {
                k = new System.Random().Next(3);
            }
            else {
                k = - (new System.Random().Next(3));
            }
            PlayerController control = gameObject.GetComponent<PlayerController>();
            if (control != null) {
                control.setnspiders(nspiders + k);
            }
            
        }
    }
}

